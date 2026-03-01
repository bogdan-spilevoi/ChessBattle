using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public enum StreamEvent { ChildAdded, ChildChanged, ChildRemoved, ValueChanged }

public sealed class RtdbStreamManager : MonoBehaviour
{
    public static RtdbStreamManager Instance
    {
        get
        {
            if (_instance != null) return _instance;

            var go = new GameObject("RtdbStreamManager");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<RtdbStreamManager>();
            return _instance;
        }
    }
    private static RtdbStreamManager _instance;

    private readonly Dictionary<string, StreamSubscription> _subs = new(); // path -> subscription

    public void Subscribe(DatabaseReference reference, StreamEvent evt, Action<DataSnapshot> handler)
    {
        if (reference == null || handler == null) return;

        var path = Normalize(reference.Path);

        if (!_subs.TryGetValue(path, out var sub))
        {
            sub = new StreamSubscription(this, reference.Db, path);
            _subs[path] = sub;
            sub.Start(); // starts streaming coroutine
        }

        sub.AddHandler(evt, handler);
    }

    public void Unsubscribe(DatabaseReference reference, StreamEvent evt, Action<DataSnapshot> handler)
    {
        if (reference == null || handler == null) return;

        var path = Normalize(reference.Path);
        if (!_subs.TryGetValue(path, out var sub)) return;

        sub.RemoveHandler(evt, handler);

        if (sub.IsEmpty)
        {
            sub.Stop();
            _subs.Remove(path);
        }
    }

    private static string Normalize(string p) => string.IsNullOrWhiteSpace(p) ? "" : p.Trim().Trim('/');
}