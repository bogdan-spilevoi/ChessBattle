using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public PlayerBehaviour player;
    

    private void Awake()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        SaveData sv = new();
        var allTrainers = FindObjectsOfType<Trainer>().ToList();
        sv.TrainerData = allTrainers.ConvertAll(t => new TrainerData(t));
        sv.InventoryData = new InventoryData() { Inventory = player.PiecesInventory };
        sv.Position = player.transform.position;
        sv.PieceFoundData = player.pieceFoundData;

        string json = JsonConvert.SerializeObject(sv, Formatting.Indented);
        print(json);
        PlayerPrefs.SetString("save" + PlayerPrefs.GetString("currentSave"), json);
    }

    public void LoadGame()
    {
        print(PlayerPrefs.GetString("save" + PlayerPrefs.GetString("currentSave")));
        SaveData sv = JsonConvert.DeserializeObject<SaveData>(PlayerPrefs.GetString("save" + PlayerPrefs.GetString("currentSave")));
        if(sv != null)
        {
            player.ChangePlayerPos(sv.Position);
            player.pieceFoundData = sv.PieceFoundData ?? new();
            var allTrainers = FindObjectsOfType<Trainer>().ToList();

            foreach (var t in allTrainers)
            {
                var trainer = sv.TrainerData.Find(td => td.Name == t.Name);
                if (trainer == null) continue;
                t.Create(trainer);
            }
        }
        player.PiecesInventory = sv == null ? new() : sv.InventoryData.Inventory;
        player.PiecesInventory ??= new();
        player.pieceFoundData ??= new();
    }
}
