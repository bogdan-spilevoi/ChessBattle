using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChessManager : MonoBehaviour
{    
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
        string white = PlayerPrefs.GetString("pieces");
        string black = PlayerPrefs.GetString("trainer");

        print(white);
        print(black);

        InventoryData whiteData = JsonConvert.DeserializeObject<InventoryData>(white);
        TrainerData blackData = JsonConvert.DeserializeObject<TrainerData>(black);

        foreach(var piece in whiteData.Inventory)
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
        InventoryData whiteData = new()
        {
            Inventory = AllWhites
        };

        string json = JsonConvert.SerializeObject(whiteData, Formatting.Indented);
        PlayerPrefs.SetString("pieces", json);
        SceneManager.LoadScene("Game");
    }

    public void EndMatch(bool state)
    {
        if(state)
        {
            ChessUI.WinUI();
        }
        else
        {
            ChessUI.LoseUI();
        }
    }
}
