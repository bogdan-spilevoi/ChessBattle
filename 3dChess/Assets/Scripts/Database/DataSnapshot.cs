using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public sealed class DataSnapshot
{
    public DatabaseReference Reference { get; }
    public string Key { get; }
    public bool Exists => _token != null && _token.Type != JTokenType.Null;

    private readonly JToken _token;

    // Used when you already have a parsed token (listener)
    public DataSnapshot(DatabaseReference reference, string key, JToken token)
    {
        Reference = reference;
        Key = key ?? GetKeyFromRef(reference);
        _token = token;
    }

    // Used by GetValueAsync (REST)
    public DataSnapshot(DatabaseReference reference, string jsonBody)
    {
        Reference = reference;
        Key = GetKeyFromRef(reference);

        if (string.IsNullOrWhiteSpace(jsonBody) || jsonBody == "null")
            _token = null;
        else
            _token = JToken.Parse(jsonBody);
    }

    public T GetValue<T>() => Exists ? _token.ToObject<T>() : default;

    public JToken Raw => _token;

    /// <summary>
    /// Firebase-SDK-like: navigate to a child snapshot by relative path (supports "a/b/c").
    /// If missing, returns a snapshot with Exists=false.
    /// </summary>
    public DataSnapshot Child(string childPath)
    {
        if (string.IsNullOrWhiteSpace(childPath))
            return this;

        var normalized = childPath.Trim().Trim('/');
        if (normalized.Length == 0)
            return this;

        // Move reference
        var childRef = Reference?.Child(normalized);

        // Move token
        JToken cur = _token;
        foreach (var seg in normalized.Split('/'))
        {
            if (cur == null || cur.Type == JTokenType.Null)
            {
                cur = null;
                break;
            }

            if (cur is JObject obj)
            {
                cur = obj.TryGetValue(seg, out var next) ? next : null;
            }
            else
            {
                // If current is not an object, it can't have named children
                cur = null;
                break;
            }
        }

        var key = GetLastSegment(normalized);
        return new DataSnapshot(childRef, key, cur);
    }

    public bool HasChild(string childPath) => Child(childPath).Exists;

    /// <summary>
    /// Enumerates direct children of this snapshot (like snapshot.Children in SDK-ish style).
    /// Only works when Raw is an object (JObject).
    /// </summary>
    public IEnumerable<DataSnapshot> Children
    {
        get
        {
            if (_token is not JObject obj) yield break;

            foreach (var prop in obj.Properties())
            {
                var childRef = Reference?.Child(prop.Name);
                yield return new DataSnapshot(childRef, prop.Name, prop.Value);
            }
        }
    }

    private static string GetKeyFromRef(DatabaseReference reference)
    {
        var path = reference?.Path ?? "";
        if (string.IsNullOrEmpty(path)) return ""; // root
        var idx = path.LastIndexOf('/');
        return idx >= 0 ? path[(idx + 1)..] : path;
    }

    private static string GetLastSegment(string path)
    {
        if (string.IsNullOrEmpty(path)) return "";
        var idx = path.LastIndexOf('/');
        return idx >= 0 ? path[(idx + 1)..] : path;
    }

    public override string ToString()
    {
        if (!Exists)
            return "null";

        return _token.ToString(Newtonsoft.Json.Formatting.None);
    }
}