using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChessManager : MonoBehaviour
{    
    public SaveData saveData;
    public TrainerData trainerData;
    public List<Piece> OrgPieces = new();
    public static int Turn = 0;
    public List<EntityData> AllWhites = new();
    public List<Piece> WhitePieces = new(), BlackPieces = new();
    public ChessUI ChessUI;

    private void Start()
    {
        PreparePieces();
        Turn = 0;
    }

    public void PreparePieces()
    {
        string white = PlayerPrefs.GetString("save" + PlayerPrefs.GetString("currentSave"));
        string black = PlayerPrefs.GetString("trainer");

        print(white);
        print(black);

        SaveData whiteData = JsonConvert.DeserializeObject<SaveData>(white);
        saveData = whiteData;
        TrainerData blackData = JsonConvert.DeserializeObject<TrainerData>(black);
        trainerData = blackData;
        Debug.LogWarning(black);

        foreach(var piece in whiteData.InventoryData.Inventory)
        {
            AllWhites.Add(piece);
            if (piece.Position == -1) continue;

            var p = Instantiate(OrgPieces[(int)piece.PieceType]);
            p.gameObject.SetActive(true);
            p.Create(piece.Position, piece);
            
            WhitePieces.Add(p);
        }

        foreach (var piece in blackData.Inventory)
        {
            if (piece.Position == -1) continue;

            var p = Instantiate(OrgPieces[(int)piece.PieceType]);
            p.gameObject.SetActive(true);
            p.side = false;
            p.Create(64 - piece.Position, piece); 
            
            BlackPieces.Add(p);
        }
    }

    public void EndGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void EndMatch(bool state)
    {
        saveData.InventoryData.Inventory = AllWhites;
        saveData.TrainerData.Find(t => t.Name == trainerData.Name).Defeated = state;
        print(trainerData.Name + " " + state);
        string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        PlayerPrefs.SetString("save" + PlayerPrefs.GetString("currentSave"), json);
        if (state)
        {
            ChessUI.WinUI();
        }
        else
        {
            ChessUI.LoseUI();
        }
    }
}
