using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineManager : MonoBehaviour
{
    public Guid MyId, OtherId, MatchId;
    public bool Accepted = false;
    //status 0 = waiting for player, 1 = waiting for inventories, 2 = playing

    private void Awake()
    {
        bool flag = PlayerPrefsExtentions.GetBool("online");
        ChessManager.Local = !flag;

        if(flag)
        {
            MyId = Guid.NewGuid();
            Ref.LoadingScreen.Toggle(true);
            LookForRoom();
        }
    }

    public async void SendCommand(string command)
    {
        Debug.Log("[OnlineManager] Sending command: " + command);
        var key = DateTime.UtcNow.ToString("yyyy-MM-ddTHH-mm-ss");
        await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("moves").Child(key).SetValueAsync(
            new Dictionary<string, object>()
            {
                { "id" , MyId.ToString() },
                { "command", command }
            });
        Debug.Log("[OnlineManager] Command sent: " + command);
    }

    public void ReceiveCommand(DataSnapshot dataSnapshot)
    {
        Debug.LogError($"[OnlineManager] Received command: {dataSnapshot}");
        if (dataSnapshot == null)
            return;

        var id = dataSnapshot.Child("id").GetValue<string>();
        if(id == null) return;

        if(id != MyId.ToString())
        {
            var command = dataSnapshot.Child("command").GetValue<string>();
            Ref.CommandManager.AddCommandExternal(command);
        }
    }

    async void LookForRoom()
    {
        var matches = await FirebaseDatabase.DefaultInstance
            .GetReference("matches")
            .GetValueAsync();
        bool tryFound = false;

        foreach (var match in matches.Children)
        {
            var state = match.Child("state").GetValue<int>();
            if (state == 0)
            {
                Debug.Log($"[OnlineManager] Found match: {match.Key}");

                FirebaseDatabase.DefaultInstance.GetReference("matches").Child(match.Key).Child("room").ChildAdded += ManageAddedToState;

                MatchId = Guid.Parse(match.Key);
                OtherId = Guid.Parse(match.Child("host").Child("id").GetValue<string>());
                Ref.LoadingScreen.SetInfo("Found match, trying to join...");

                tryFound = true;

                await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(match.Key).Child("room").Child("trying").SetValueAsync(MyId.ToString());
                break;
            }
        }
        if (!tryFound)
        {
            Ref.LoadingScreen.SetInfo("No matches found, creating room...");
            CreateRoom();
        }
    }

    async void CreateRoom()
    {
        Ref.LoadingScreen.SetInfo("Creating room...");
        var matchGuid = Guid.NewGuid().ToString();
        MatchId = Guid.Parse(matchGuid);

        await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(matchGuid).Child("state").SetValueAsync(0);
        await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(matchGuid).Child("room").SetValueAsync(0);

        await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(matchGuid).Child("host").Child("id").SetValueAsync(MyId.ToString());

        FirebaseDatabase.DefaultInstance.GetReference("matches").Child(matchGuid).Child("room").ChildAdded += HostManageRoom;

        Ref.LoadingScreen.SetInfo("Room created, waiting for player...");
    }

    private async void HostManageRoom(DataSnapshot dataSnapshot)
    {
        if(Accepted)
        {
            return;
        }
        if(dataSnapshot.Key == "trying")
        {
            Ref.LoadingScreen.HideCancel();
            Ref.LoadingScreen.SetInfo("Player is trying to join, accepting...");
            Accepted = true;
            Debug.Log($"[OnlineManager] Accepting Player to join: {dataSnapshot}");
            OtherId = Guid.Parse(dataSnapshot.GetValue<string>());

            FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("room").ChildAdded -= HostManageRoom;

            await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("room").Child("accepted").SetValueAsync(dataSnapshot.GetValue<string>());
            await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("state").SetValueAsync(1);
            await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("host").Child("pieces").SetValueAsync(Ref.ChessManager.GetMyInventoryData());
            FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("guest").Child("pieces").ValueChanged += ManageGuestSendsInventory;
        }
    }

    public void ManageAddedToState(DataSnapshot dataSnapshot)
    {
        if(dataSnapshot.Key == "accepted" && MyId == Guid.Parse(dataSnapshot.GetValue<string>()))
        {
            Ref.LoadingScreen.HideCancel();
            Ref.LoadingScreen.SetInfo("Player accepted, preparing match...");
            Debug.Log($"[OnlineManager] Match accepted by other player: {dataSnapshot}");
            Accepted = true;

            FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("room").ChildAdded -= ManageAddedToState;
            FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("state").ValueChanged += ManageStateChangedFromGuest;
            EnterAsGuest();
        }
        if (dataSnapshot.Key == "accepted" && MyId != Guid.Parse(dataSnapshot.GetValue<string>()))
        {
            Ref.LoadingScreen.SetInfo("Player rejected, going back...");
            Debug.Log($"[OnlineManager] Match rejected by other player: {dataSnapshot}");

            FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("room").ChildAdded -= ManageAddedToState;

            CreateRoom();
        }
    }

    public async void ManageStateChangedFromGuest(DataSnapshot dataSnapshot)
    {
        if(dataSnapshot.Key == "state" && dataSnapshot.GetValue<int>() == 2)
        {
            var hostPieces = await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("host").Child("pieces").GetValueAsync();
            Debug.Log($"[OnlineManager] Match is starting: {dataSnapshot}");
            FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("state").ValueChanged -= ManageStateChangedFromGuest;

            Ref.ChessManager.PreparePieces(hostPieces.GetValue<string>(), false);
            Ref.LoadingScreen.Toggle(false);

            FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("moves").ChildAdded += ReceiveCommand;
        }
    }

    public async void ManageGuestSendsInventory(DataSnapshot dataSnapshot)
    {
        if (dataSnapshot.GetValue<string>() == null)
            return;

        Ref.LoadingScreen.SetInfo("Starting match...");

        FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("guest").Child("pieces").ValueChanged -= ManageGuestSendsInventory;
        await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("state").SetValueAsync(2);

        Ref.ChessManager.PreparePieces(dataSnapshot.GetValue<string>(), true);
        Ref.LoadingScreen.Toggle(false);

        FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("moves").ChildAdded += ReceiveCommand;

        Debug.Log($"[OnlineManager] Guest sent inventory: {dataSnapshot}");
    }

    public async void EnterAsGuest()
    {
        await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("moves").SetValueAsync(0);
        await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("guest").Child("id").SetValueAsync(MyId.ToString());
        await FirebaseDatabase.DefaultInstance.GetReference("matches").Child(MatchId.ToString()).Child("guest").Child("pieces").SetValueAsync(Ref.ChessManager.GetMyInventoryData());
        Ref.LoadingScreen.SetInfo("Joining match...");
    }
}
