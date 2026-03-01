using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

internal sealed class StreamSubscription
{
    private readonly MonoBehaviour _runner;
    private readonly FirebaseDatabase _db;
    private readonly string _path;

    private UnityWebRequest _req;
    private StreamingDownloadHandler _handler;

    private readonly ConcurrentQueue<Action> _mainThread = new();

    // child inference state
    private readonly Dictionary<string, JToken> _children = new();
    private readonly HashSet<string> _knownKeys = new();
    private readonly HashSet<string> _pendingAdded = new();

    private bool _running;
    private bool _receivedInitial;

    private readonly DatabaseReference _baseRef;

    // handlers
    private readonly HashSet<Action<DataSnapshot>> _onAdded = new();
    private readonly HashSet<Action<DataSnapshot>> _onChanged = new();
    private readonly HashSet<Action<DataSnapshot>> _onRemoved = new();
    private readonly HashSet<Action<DataSnapshot>> _onValue = new();

    public bool IsEmpty =>
        _onAdded.Count == 0 && _onChanged.Count == 0 && _onRemoved.Count == 0 && _onValue.Count == 0;

    public StreamSubscription(MonoBehaviour runner, FirebaseDatabase db, string path)
    {
        _runner = runner;
        _db = db;
        _path = path ?? "";
        _baseRef = _db.GetReference(_path);
    }

    public void AddHandler(StreamEvent evt, Action<DataSnapshot> h)
    {
        switch (evt)
        {
            case StreamEvent.ChildAdded: _onAdded.Add(h); break;
            case StreamEvent.ChildChanged: _onChanged.Add(h); break;
            case StreamEvent.ChildRemoved: _onRemoved.Add(h); break;
            case StreamEvent.ValueChanged: _onValue.Add(h); break;
        }
    }

    public void RemoveHandler(StreamEvent evt, Action<DataSnapshot> h)
    {
        switch (evt)
        {
            case StreamEvent.ChildAdded: _onAdded.Remove(h); break;
            case StreamEvent.ChildChanged: _onChanged.Remove(h); break;
            case StreamEvent.ChildRemoved: _onRemoved.Remove(h); break;
            case StreamEvent.ValueChanged: _onValue.Remove(h); break;
        }
    }

    public void Start()
    {
        if (_running) return;
        _running = true;

        _receivedInitial = false;
        _children.Clear();
        _knownKeys.Clear();
        _pendingAdded.Clear();

        _runner.StartCoroutine(ListenLoop());
        _runner.StartCoroutine(DispatchLoop());
    }

    public void Stop()
    {
        _running = false;
        try { _req?.Abort(); } catch { }
        _req?.Dispose();
        _req = null;
        _handler = null;
    }

    private System.Collections.IEnumerator DispatchLoop()
    {
        while (_running)
        {
            while (_mainThread.TryDequeue(out var a))
                a?.Invoke();
            yield return null;
        }
    }

    private System.Collections.IEnumerator ListenLoop()
    {
        int backoffMs = 500;

        while (_running)
        {
            var url = BuildStreamUrl(_db.DatabaseUrl, _path, _db.AuthToken);

            _handler = new StreamingDownloadHandler(OnSseEvent);
            _req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET)
            {
                downloadHandler = _handler
            };
            _req.SetRequestHeader("Accept", "text/event-stream");

            var op = _req.SendWebRequest();

            while (!op.isDone && _running)
                yield return null;

            if (!_running) break;

            yield return new WaitForSeconds(backoffMs / 1000f);
            backoffMs = Math.Min(backoffMs * 2, 8000);
        }
    }

    private static string BuildStreamUrl(string baseUrl, string path, string authToken)
    {
        baseUrl = (baseUrl ?? "").Trim().TrimEnd('/');
        path = (path ?? "").Trim().Trim('/');

        var url = $"{baseUrl}/{path}.json";

        var q = new List<string>();
        if (!string.IsNullOrEmpty(authToken))
            q.Add("auth=" + UnityWebRequest.EscapeURL(authToken));

        return q.Count > 0 ? (url + "?" + string.Join("&", q)) : url;
    }

    private void OnSseEvent(string evt, string dataJson)
    {
        if (string.IsNullOrWhiteSpace(evt) || string.IsNullOrWhiteSpace(dataJson))
            return;

        JObject payload;
        try { payload = JObject.Parse(dataJson); }
        catch { return; }

        var relPath = payload["path"]?.Value<string>();
        var data = payload["data"];

        if (relPath == null) return;

        _mainThread.Enqueue(() => ApplyEvent(evt, relPath, data));
    }

    private DataSnapshot Snap(string key, JToken token)
    {
        var childRef = string.IsNullOrEmpty(key) ? _baseRef : _baseRef.Child(key);
        return new DataSnapshot(childRef, key, token);
    }

    private void EmitAdded(string key, JToken token)
    {
        var s = Snap(key, token);
        var handlers = _onAdded.ToArray();
        foreach (var h in handlers) h?.Invoke(s);
    }

    private void EmitChanged(string key, JToken token)
    {
        var s = Snap(key, token);
        var handlers = _onChanged.ToArray();
        foreach (var h in handlers) h?.Invoke(s);
    }

    private void EmitRemoved(string key)
    {
        var s = Snap(key, null);
        var handlers = _onRemoved.ToArray();
        foreach (var h in handlers) h?.Invoke(s);
    }

    private void ApplyEvent(string evt, string relativePath, JToken data)
    {
        relativePath = (relativePath ?? "").Trim();
        if (!relativePath.StartsWith("/")) relativePath = "/" + relativePath;

        if (evt == "put")
        {
            if (relativePath == "/")
            {
                HandleFullSnapshot(data);
                EmitValueChanged(data);
                _receivedInitial = true;
                return;
            }

            var parts = relativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            var key = parts[0];

            if (parts.Length == 1) HandleChildReplace(key, data);
            else HandleChildFieldUpdate(key, parts, data);

            EmitValueChanged(BuildCurrentRoot());
        }
        else if (evt == "patch")
        {
            HandlePatch(relativePath, data);
        }
    }

    private void HandleFullSnapshot(JToken data)
    {
        var obj = data as JObject;

        var newKeys = new HashSet<string>();
        if (obj != null)
            foreach (var prop in obj.Properties())
                newKeys.Add(prop.Name);

        foreach (var oldKey in new List<string>(_knownKeys))
        {
            if (!newKeys.Contains(oldKey))
            {
                _knownKeys.Remove(oldKey);
                _children.Remove(oldKey);
                _pendingAdded.Remove(oldKey);
                EmitRemoved(oldKey);
            }
        }

        if (obj == null) return;

        foreach (var prop in obj.Properties())
        {
            var key = prop.Name;
            var val = prop.Value;

            if (_knownKeys.Add(key))
            {
                _children[key] = val;

                // delay added if empty object
                if (val is JObject o && !o.HasValues)
                    _pendingAdded.Add(key);
                else
                    EmitAdded(key, val);
            }
            else
            {
                _children[key] = val;
                EmitChanged(key, val);
            }
        }
    }

    private void HandleChildReplace(string key, JToken data)
    {
        if (data == null || data.Type == JTokenType.Null)
        {
            if (_knownKeys.Remove(key))
            {
                _children.Remove(key);
                _pendingAdded.Remove(key);
                EmitRemoved(key);
            }
            return;
        }

        var isNew = _knownKeys.Add(key);
        _children[key] = data;

        if (isNew)
        {
            if (data is JObject o && !o.HasValues) _pendingAdded.Add(key);
            else EmitAdded(key, data);
            return;
        }

        if (_pendingAdded.Remove(key))
        {
            EmitAdded(key, data);
            return;
        }

        EmitChanged(key, data);
    }

    private void HandleChildFieldUpdate(string key, string[] parts, JToken leafData)
    {
        if (!_knownKeys.Contains(key))
        {
            _knownKeys.Add(key);
            _children[key] = new JObject();
            _pendingAdded.Add(key);
        }

        if (_children[key] is not JObject childObj)
        {
            EmitChanged(key, _children[key]);
            return;
        }

        JObject cursor = childObj;
        for (int i = 1; i < parts.Length - 1; i++)
        {
            var seg = parts[i];
            if (cursor[seg] is not JObject next)
            {
                next = new JObject();
                cursor[seg] = next;
            }
            cursor = next;
        }

        var leafKey = parts[^1];
        cursor[leafKey] = leafData;

        if (_pendingAdded.Remove(key))
            EmitAdded(key, childObj);
        else
            EmitChanged(key, childObj);
    }

    private void HandlePatch(string relativePath, JToken data)
    {
        if (data is not JObject obj) return;

        if (relativePath != "/")
        {
            var parts = relativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1)
            {
                var key = parts[0];
                foreach (var prop in obj.Properties())
                    ApplyEvent("put", "/" + key + "/" + prop.Name, prop.Value);
            }
            return;
        }

        foreach (var prop in obj.Properties())
            HandleChildReplace(prop.Name, prop.Value);
    }

    private void EmitValueChanged(JToken token)
    {
        var key = string.IsNullOrEmpty(_path) ? "" : _path[(_path.LastIndexOf('/') + 1)..];
        var s = new DataSnapshot(_baseRef, key, token);
        var handlers = _onValue.ToArray();
        foreach (var h in handlers) h?.Invoke(s);
    }

    private JObject BuildCurrentRoot()
    {
        var root = new JObject();
        foreach (var kv in _children)
            root[kv.Key] = kv.Value;
        return root;
    }

    // SSE parser
    private class StreamingDownloadHandler : DownloadHandlerScript
    {
        private readonly Action<string, string> _onEvent;
        private readonly StringBuilder _buffer = new();

        private string _currentEvent;
        private readonly StringBuilder _currentData = new();

        public StreamingDownloadHandler(Action<string, string> onEvent) : base(new byte[16 * 1024])
        {
            _onEvent = onEvent;
        }

        protected override bool ReceiveData(byte[] data, int dataLength)
        {
            if (data == null || dataLength <= 0) return true;

            _buffer.Append(Encoding.UTF8.GetString(data, 0, dataLength));
            ParseBuffer();
            return true;
        }

        private void ParseBuffer()
        {
            while (true)
            {
                var buf = _buffer.ToString();
                var nl = buf.IndexOf('\n');
                if (nl < 0) return;

                var line = buf.Substring(0, nl).TrimEnd('\r');
                _buffer.Remove(0, nl + 1);

                if (line.Length == 0)
                {
                    if (!string.IsNullOrEmpty(_currentEvent))
                    {
                        _onEvent?.Invoke(_currentEvent, _currentData.ToString());
                        _currentEvent = null;
                        _currentData.Clear();
                    }
                    continue;
                }

                if (line.StartsWith("event:"))
                    _currentEvent = line.Substring("event:".Length).Trim();
                else if (line.StartsWith("data:"))
                {
                    var d = line.Substring("data:".Length).Trim();
                    if (_currentData.Length > 0) _currentData.Append('\n');
                    _currentData.Append(d);
                }
            }
        }
    }
}