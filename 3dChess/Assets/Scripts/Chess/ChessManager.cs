using Newtonsoft.Json;
using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChessManager : MonoBehaviour
{
    public InventoryData WhiteData, BlackData;
    public SaveData saveData;
    public string OpponentName;

    public List<Piece> OrgPieces = new();

    public static int Turn = 0;

    public List<EntityData> AllWhites = new();
    public List<Piece> WhitePieces = new(), BlackPieces = new();
    public ChessUI ChessUI;
    public static bool Local = true;

    public static bool Side = true;

    private void Start()
    {
        if(Local)
        {
            PrepareLocalMatch();
            PreparePieces(WhiteData, BlackData);
            Turn = 0;
            Ref.AI.CreateChess();
        }
    }

    //Match preparation
    public void PreparePieces(string incoming, bool side)
    {
        print("Starting chess match with side:\n" + side);
        string mine = GetMyInventoryData();
        Side = side;

        if(!side)
        {
            Ref.ManageTiles.SwitchBoard();
            PreparePieces(JsonConvert.DeserializeObject<InventoryData>(incoming), JsonConvert.DeserializeObject<InventoryData>(mine));
        }
        else
        {
            PreparePieces(JsonConvert.DeserializeObject<InventoryData>(mine), JsonConvert.DeserializeObject<InventoryData>(incoming));
        }

        
    }
    public void PreparePieces(InventoryData white, InventoryData black)
    {
        WhiteData = white;
        BlackData = black;

        WhiteData.Pieces = WhiteData.Pieces.Where(p => p.Position > -1).ToList();
        WhiteData.Potions = WhiteData.Potions.Where(p => p.Position > -1).ToList();

        BlackData.Pieces = BlackData.Pieces.Where(p => p.Position > -1).ToList();
        BlackData.Potions = BlackData.Potions.Where(p => p.Position > -1).ToList();

        foreach (var piece in white.Pieces)
        {
            AllWhites.Add(piece);
            if (piece.Position == -1) continue;

            var p = Instantiate(OrgPieces[(int)piece.PieceType]);
            p.gameObject.SetActive(true);
            p.Create(piece.Position, piece);

            foreach(var move in piece.Moves)
            {
                move.Count = move.MaxCount;
            }
            
            WhitePieces.Add(p);
        }

        foreach (var piece in black.Pieces)
        {
            if (piece.Position == -1) continue;

            var p = Instantiate(OrgPieces[(int)piece.PieceType]);
            p.gameObject.SetActive(true);
            p.side = false;
            p.Create(64 - piece.Position, piece);

            foreach (var move in piece.Moves)
            {
                move.Count = move.MaxCount;
            }

            BlackPieces.Add(p);
        }
    }

    public void EndGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void EndMatch(bool state)
    {
        saveData.InventoryData.Pieces = AllWhites;
        saveData.InventoryData.Potions = WhiteData.Potions;
        if (Local)
            saveData.TrainerData.Find(t => t.Name == OpponentName).Defeated = state;

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

    public void PrepareLocalMatch()
    {
        string white = PlayerPrefs.GetString("save" + PlayerPrefs.GetString("currentSave"));
        string black = PlayerPrefs.GetString("trainer");
        OpponentName = PlayerPrefs.GetString("trainerName");

        SaveData whiteData = JsonConvert.DeserializeObject<SaveData>(white);
        saveData = whiteData;

        InventoryData whiteInventory = whiteData.InventoryData;
        whiteInventory.Pieces = whiteInventory.Pieces.Where(p => p.Position > -1).ToList();
        whiteInventory.Potions = whiteInventory.Potions.Where(p => p.Position > -1).ToList();

        InventoryData blackInventory = JsonConvert.DeserializeObject<InventoryData>(black);

        WhiteData = whiteInventory;
        BlackData = blackInventory;
    }

    public string GetMyInventoryData()
    {
        string white = PlayerPrefs.GetString("save" + PlayerPrefs.GetString("currentSave"));
        string black = PlayerPrefs.GetString("trainer");

        SaveData whiteData = JsonConvert.DeserializeObject<SaveData>(white);
        saveData = whiteData;

        InventoryData whiteInventory = whiteData.InventoryData;
        whiteInventory.Pieces = whiteInventory.Pieces.Where(p => p.Position > -1).ToList();
        return JsonConvert.SerializeObject(whiteInventory);
    }

    


    //Match logic
    public void MovePiece(bool side, int pieceInd, int tileInd)
    {
        var piece = side ? WhitePieces[pieceInd] : BlackPieces[pieceInd];
        var tile = Ref.ManageTiles.GetTile(tileInd);
        MovePiece(side, piece, tile);
    }
    public void MovePiece(bool side, Piece piece, Tile tile)
    {     
        if (tile.currentPiece != null)
        {
            BattleManager.Ongoing = true;
            Ref.VersusUI.Create(piece, tile.currentPiece, () =>
            {
                Ref.BattleManager.StartBattle(piece, tile.currentPiece, tile, side);
            });      
        }
        else
        {
            piece.GoToTile(tile);
            IncreaseTurn();
        }
    }



    public void PrepareMove(bool side, Piece piece, Tile tile)
    {
        var pieceInd = GetPieceIndex(piece);
        var tileInd = tile.GetIndex();

        Ref.CommandManager.AddCommandLocal(new MoveCommand(side, pieceInd, tileInd));
    }

    //Helpers
    public int GetPieceIndex(Piece piece)
    {
        var pieces = piece.side ? WhitePieces : BlackPieces;
        return pieces.IndexOf(piece);
    }
    public int GetPieceIndex(bool side, Piece piece)
    {
        var pieces = side ? WhitePieces : BlackPieces;
        return pieces.IndexOf(piece);
    }

    public Piece GetPieceByIndex(bool side, int pieceInd)
    {
        var pieces = side ? WhitePieces : BlackPieces;
        return pieces[pieceInd];
    }

    public PotionData GetPotionByIndex(bool side, int potionInd)
    {
        var inventory = side ? WhiteData : BlackData;
        return inventory.Potions.Where(p => p.Position == potionInd).First();
    }
    public void RemovePotionAtIndex(bool side, int potionInd)
    {
        var inventory = side ? WhiteData : BlackData;
        inventory.Potions.Remove(GetPotionByIndex(side, potionInd));
    }

    public static bool IsMyTurn()
    {
        return Side == (Turn % 2 == 0);
    }

    public static void IncreaseTurn()
    {
        Debug.LogWarning("Chess turn increased");
        Turn++;
    }
}
