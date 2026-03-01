using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseTest : MonoBehaviour
{
    [Header("Fill these from Firebase Console")]
    public string DatabaseUrl = "https://YOUR_PROJECT-default-rtdb.europe-west1.firebasedatabase.app";
    public string AuthToken = ""; // Leave empty if rules allow public read

    private RtdbRestListener _listener;

    private DatabaseReference _eventsRef;

    // Keep delegates so we can unsubscribe cleanly
    private System.Action<DataSnapshot> _onAdded;
    private System.Action<DataSnapshot> _onChanged;
    private System.Action<DataSnapshot> _onRemoved;

    async void Start()
    {
        _eventsRef = FirebaseDatabase.DefaultInstance
            .GetReference("matches")
            .Child("match1")
            .Child("events");

        _onAdded = snap =>
        {
            Debug.Log($"[SDK-STYLE] ChildAdded key={snap.Key} path={snap.Reference.Path}");
            Debug.Log(snap);
        };

        _onChanged = snap =>
        {
            Debug.Log($"[SDK-STYLE] ChildChanged key={snap.Key} path={snap.Reference.Path}");
            Debug.Log(snap);
        };

        _onRemoved = snap =>
        {
            Debug.Log($"[SDK-STYLE] ChildRemoved key={snap.Key} path={snap.Reference.Path}");
        };

        // Subscribe (this should start streaming internally)
        _eventsRef.ChildAdded += _onAdded;
        _eventsRef.ChildChanged += _onChanged;
        _eventsRef.ChildRemoved += _onRemoved;

        Debug.Log("[SDK-STYLE] Subscribed to matches/match1/events");

        // Write a test event (so you can immediately see ChildAdded fire)
        var newEventRef = _eventsRef.Push();
        await newEventRef.SetValueAsync(new
        {
            type = "MovePlayed",
            playerId = "p1",
            move = "A2-A4",
            ts = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });

        Debug.Log($"[SDK-STYLE] Wrote test event: {newEventRef.Path}");
    }

    private void OnDestroy()
    {
        if (_eventsRef == null) return;

        // Unsubscribe to avoid duplicate handlers if you reload scenes / enter play mode multiple times
        if (_onAdded != null) _eventsRef.ChildAdded -= _onAdded;
        if (_onChanged != null) _eventsRef.ChildChanged -= _onChanged;
        if (_onRemoved != null) _eventsRef.ChildRemoved -= _onRemoved;

        Debug.Log("[SDK-STYLE] Unsubscribed");
    }
}
