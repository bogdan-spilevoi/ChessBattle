using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class RtdbRestListener : MonoBehaviour
{
    [Tooltip("Full RTDB base URL")]
    public string DatabaseUrl;

    [Tooltip("Path to listen to, e.g. matches/ABC/events")]
    public string Path;

    [Tooltip("Optional Firebase Auth ID token if your rules require it")]
    public string AuthToken;

    [Tooltip("If true, emits ChildAdded for existing children on first connect (like Firebase SDK).")]
    public bool EmitExistingAsChildAdded = true;

    // key, payload (usually JObject/JValue/etc). You can convert to your type with token.ToObject<T>()
    public event Action<DataSnapshot> ChildAdded;
    public event Action<DataSnapshot> ChildChanged;
    public event Action<DataSnapshot> ChildRemoved;

    // "connecting", "open", "error", "reconnecting", "closed"
    public event Action<string> ConnectionState;

    private UnityWebRequest _req;
    private StreamingDownloadHandler _handler;

    // Local state to infer child events
    private readonly Dictionary<string, JToken> _children = new();
    private readonly HashSet<string> _knownKeys = new();
    private readonly HashSet<string> _pendingAdded = new();

    private readonly ConcurrentQueue<Action> _mainThread = new();
    private bool _running;
    private bool _receivedInitial;
    private DatabaseReference _baseRef;

    private void EnsureBaseRef()
    {
        // assumes your REST FirebaseDatabase wrapper exists
        _baseRef ??= FirebaseDatabase.DefaultInstance.GetReference(Path);
    }

    private DataSnapshot MakeSnapshot(string key, JToken token)
    {
        EnsureBaseRef();
        var childRef = string.IsNullOrEmpty(key) ? _baseRef : _baseRef.Child(key);
        return new DataSnapshot(childRef, key, token);
    }

    public void Setup(string DbUrl, string path)
    {
        DatabaseUrl = DbUrl;
        Path = path;
        EmitExistingAsChildAdded = true;
    }

    public void StartListening()
    {
        DatabaseUrl = FirebaseConfigLoader.DatabaseUrl();
        if (_running) return;

        _running = true;
        _receivedInitial = false;
        _children.Clear();
        _knownKeys.Clear();
        _pendingAdded.Clear();

        StartCoroutine(ListenLoop());
    }

    public void StopListening()
    {
        _running = false;
        try { _req?.Abort(); } catch { /* ignore */ }
        _req?.Dispose();
        _req = null;
        _handler = null;

        ConnectionState?.Invoke("closed");
    }

    private void Update()
    {
        while (_mainThread.TryDequeue(out var a))
            a?.Invoke();
    }

    private System.Collections.IEnumerator ListenLoop()
    {
        int backoffMs = 500;

        while (_running)
        {
            ConnectionState?.Invoke(_receivedInitial ? "reconnecting" : "connecting");

            var url = BuildStreamUrl(DatabaseUrl, Path, AuthToken);
            _handler = new StreamingDownloadHandler(OnSseEvent);
            _req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET)
            {
                downloadHandler = _handler
            };

            _req.SetRequestHeader("Accept", "text/event-stream");

            var op = _req.SendWebRequest();
            ConnectionState?.Invoke("open");

            while (!op.isDone && _running)
                yield return null;

            if (!_running) break;

            ConnectionState?.Invoke("error");

            yield return new WaitForSeconds(backoffMs / 1000f);
            backoffMs = Math.Min(backoffMs * 2, 8000);
        }
    }

    private static string BuildStreamUrl(string baseUrl, string path, string authToken)
    {
        baseUrl = (baseUrl ?? "").Trim().TrimEnd('/');
        path = (path ?? "").Trim().Trim('/');

        // REST streaming endpoint: /path.json
        var url = $"{baseUrl}/{path}.json";

        // print=silent reduces chatter; you can remove it if you want full payload prints
        var q = new List<string>();// { "print=silent" };
        if (!string.IsNullOrEmpty(authToken))
            q.Add("auth=" + UnityWebRequest.EscapeURL(authToken));

        return q.Count > 0 ? (url + "?" + string.Join("&", q)) : url;
    }

    // SSE callback: evt is "put" or "patch", dataJson is the JSON object with { path, data }
    private void OnSseEvent(string evt, string dataJson)
    {
        if (string.IsNullOrWhiteSpace(evt) || string.IsNullOrWhiteSpace(dataJson))
            return;

        JObject payload;
        try
        {
            payload = JObject.Parse(dataJson);
        }
        catch
        {
            return;
        }

        var relPath = payload["path"]?.Value<string>();
        var data = payload["data"]; // can be null

        if (relPath == null) return;

        _mainThread.Enqueue(() => ApplyEvent(evt, relPath, data));
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
                _receivedInitial = true;
                return;
            }

            var parts = relativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            var key = parts[0];

            if (parts.Length == 1)
            {
                HandleChildReplace(key, data);
            }
            else
            {
                HandleChildFieldUpdate(key, parts, data);
            }
        }
        else if (evt == "patch")
        {
            HandlePatch(relativePath, data);
        }
    }

    private void HandleFullSnapshot(JToken data)
    {
        // data can be null or an object { childKey: childValue, ... }
        var obj = data as JObject;

        var newKeys = new HashSet<string>();
        if (obj != null)
        {
            foreach (var prop in obj.Properties())
                newKeys.Add(prop.Name);
        }

        // removals
        foreach (var oldKey in new List<string>(_knownKeys))
        {
            if (!newKeys.Contains(oldKey))
            {
                _knownKeys.Remove(oldKey);
                _children.Remove(oldKey);
                _pendingAdded.Remove(oldKey);
                ChildRemoved?.Invoke(MakeSnapshot(oldKey, null));
            }
        }

        // adds/changes
        if (obj != null)
        {
            foreach (var prop in obj.Properties())
            {
                var key = prop.Name;
                var val = prop.Value;

                if (_knownKeys.Add(key))
                {
                    _children[key] = val;

                    if (EmitExistingAsChildAdded)
                    {
                        // If it's empty object, delay ChildAdded until it gets real data
                        if (val is JObject o && !o.HasValues)
                            _pendingAdded.Add(key);
                        else
                            ChildAdded?.Invoke(MakeSnapshot(key, val));
                    }
                }
                else
                {
                    _children[key] = val;
                    ChildChanged?.Invoke(MakeSnapshot(key, val));

                }
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
                ChildRemoved?.Invoke(MakeSnapshot(key, null));
            }
            return;
        }

        var isNew = _knownKeys.Add(key);
        _children[key] = data;

        if (isNew)
        {
            // new child
            if (EmitExistingAsChildAdded)
            {
                if (data is JObject o && !o.HasValues) _pendingAdded.Add(key);
                else ChildAdded?.Invoke(MakeSnapshot(key, data));
            }
            return;
        }

        // existing child
        if (_pendingAdded.Remove(key))
        {
            // first real payload after being empty
            ChildChanged?.Invoke(MakeSnapshot(key, data));
            return;
        }

        ChildChanged?.Invoke(MakeSnapshot(key, data));
    }

    private void HandleChildFieldUpdate(string key, string[] parts, JToken leafData)
    {
        if (!_knownKeys.Contains(key))
        {
            _knownKeys.Add(key);
            _children[key] = new JObject();
            _pendingAdded.Add(key);
        }

        // if we have a JObject, we can apply deep patches
        if (_children[key] is not JObject childObj)
        {
            // emit snapshot for the root child as-is
            ChildChanged?.Invoke(MakeSnapshot(key, _children[key]));
            return;
        }

        // Navigate/create objects for parts[1..n-1]
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
        cursor[leafKey] = leafData; // can be null
        if (_pendingAdded.Remove(key))
            ChildAdded?.Invoke(MakeSnapshot(key, childObj));
        else
            ChildChanged?.Invoke(MakeSnapshot(key, childObj));
    }

    private void HandlePatch(string relativePath, JToken data)
    {
        // Usually: patch at "/" with object { childKey: valueOrNull, ... }
        if (!(data is JObject obj)) return;

        if (relativePath != "/")
        {
            // Best-effort: apply patch as deep updates beneath relativePath
            // Example: relativePath="/childA", data={ "score": 2 }
            var parts = relativePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1)
            {
                var key = parts[0];
                foreach (var prop in obj.Properties())
                {
                    ApplyEvent("put", "/" + key + "/" + prop.Name, prop.Value);
                }
            }
            return;
        }

        foreach (var prop in obj.Properties())
        {
            HandleChildReplace(prop.Name, prop.Value);
        }
    }

    // --------------------------------------------
    // StreamingDownloadHandler: parses SSE frames
    // --------------------------------------------
    private class StreamingDownloadHandler : DownloadHandlerScript
    {
        private readonly Action<string, string> _onEvent;
        private readonly StringBuilder _buffer = new();

        private string _currentEvent;
        private readonly StringBuilder _currentData = new();

        public StreamingDownloadHandler(Action<string, string> onEvent)
            : base(new byte[16 * 1024])
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
                {
                    _currentEvent = line.Substring("event:".Length).Trim();
                }
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