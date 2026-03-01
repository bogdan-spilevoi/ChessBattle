using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed class DatabaseReference
{
    private readonly FirebaseDatabase _db;
    internal FirebaseDatabase Db => _db;
    public string Path { get; }

    internal DatabaseReference(FirebaseDatabase db, string path)
    {
        _db = db;
        Path = path ?? "";
    }

    public DatabaseReference Child(string childPath)
    {
        childPath = (childPath ?? "").Trim('/');

        if (string.IsNullOrEmpty(Path))
            return new DatabaseReference(_db, childPath);

        return new DatabaseReference(_db, $"{Path}/{childPath}");
    }

    public DatabaseReference Push()
    {
        var id = Guid.NewGuid().ToString("N");
        return Child(id);
    }

    public async Task<DataSnapshot> GetValueAsync()
    {
        var url = _db.BuildUrl(Path);
        var (code, body) = await HttpHelper.SendAsync("GET", url, null);

        if (code < 200 || code >= 300)
            throw new Exception($"Firebase GET failed: {code} {body}");

        return new DataSnapshot(this, body);
    }

    public async Task SetValueAsync(object value)
    {
        var json = JsonConvert.SerializeObject(value);
        var url = _db.BuildUrl(Path);

        var (code, body) = await HttpHelper.SendAsync("PUT", url, json);

        if (code < 200 || code >= 300)
            throw new Exception($"Firebase PUT failed: {code} {body}");
    }

    public async Task UpdateChildrenAsync(Dictionary<string, object> updates)
    {
        var json = JsonConvert.SerializeObject(updates);
        var url = _db.BuildUrl(Path);

        var (code, body) = await HttpHelper.SendAsync("PATCH", url, json);

        if (code < 200 || code >= 300)
            throw new Exception($"Firebase PATCH failed: {code} {body}");
    }

    public async Task RemoveValueAsync()
    {
        var url = _db.BuildUrl(Path);
        var (code, body) = await HttpHelper.SendAsync("DELETE", url, null);

        if (code < 200 || code >= 300)
            throw new Exception($"Firebase DELETE failed: {code} {body}");
    }

    public event Action<DataSnapshot> ChildAdded
    {
        add => RtdbStreamManager.Instance.Subscribe(this, StreamEvent.ChildAdded, value);
        remove => RtdbStreamManager.Instance.Unsubscribe(this, StreamEvent.ChildAdded, value);
    }

    public event Action<DataSnapshot> ChildChanged
    {
        add => RtdbStreamManager.Instance.Subscribe(this, StreamEvent.ChildChanged, value);
        remove => RtdbStreamManager.Instance.Unsubscribe(this, StreamEvent.ChildChanged, value);
    }

    public event Action<DataSnapshot> ChildRemoved
    {
        add => RtdbStreamManager.Instance.Subscribe(this, StreamEvent.ChildRemoved, value);
        remove => RtdbStreamManager.Instance.Unsubscribe(this, StreamEvent.ChildRemoved, value);
    }

    public event Action<DataSnapshot> ValueChanged
    {
        add => RtdbStreamManager.Instance.Subscribe(this, StreamEvent.ValueChanged, value);
        remove => RtdbStreamManager.Instance.Unsubscribe(this, StreamEvent.ValueChanged, value);
    }

    public override string ToString() => "/" + Path;
}