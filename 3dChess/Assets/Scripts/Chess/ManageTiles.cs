using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ManageTiles : MonoBehaviour
{
    public Tile OriginalTile;
    public List<List<Tile>> Tiles = new();

    public Material Empty, Preview, Move;

    private float originalSize;
    private Vector3 originalPos;

    public enum TileState { empty, preview, move }

    private void Awake()
    {
        originalSize = OriginalTile.transform.localScale.x;
        originalPos = OriginalTile.transform.localPosition;
        MakeTiles();
    }

    public Tile GetFenTile(string fen)
    {
        string alph = "abcdefgh";
        char letter = fen[0];
        int num = int.Parse(fen[1].ToString());

        return GetTile(num - 1, alph.IndexOf(letter));
    }

    public Tile GetTile(int index)
    {
        try
        {
            return Tiles[index / 8][index % 8];
        }
        catch
        {
            return null;
        }
    }

    public Tile GetTile(int x, int y)
    {
        try
        {
            return Tiles[x][y];
        }
        catch
        {
            return null;
        }
    }
    public Tile GetTile((int x, int y) tile)
    {
        try
        {
            return Tiles[tile.x][tile.y];
        }
        catch
        {
            return null;
        }
    }

    public (int x, int y) GetUnderTile(Vector3 position)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                //Debug.Log(i + " " + j);
                if(IsInsideSquare(new Vector2(position.x, position.z), new Vector2(Tiles[i][j].transform.position.x, Tiles[i][j].transform.position.z), originalSize/10))
                {
                    return (i, j);
                }
            }
        }
        return (-1, -1);
    }

    bool IsInsideSquare(Vector2 point, Vector2 center, float sideLength)
    {
        //Debug.Log(point + " " + center);
        float halfSide = sideLength / 2f;

        return
            point.x >= center.x - halfSide &&
            point.x <= center.x + halfSide &&
            point.y >= center.y - halfSide &&
            point.y <= center.y + halfSide;
    }

    public void SetTileState(TileState state, int x, int y)
    {
        if (x == -1 || y == -1) return;
        Material newState = state switch
        {
            TileState.empty => Empty,
            TileState.preview => Preview,
            TileState.move => Move,
            _ => null
        };
        Tiles[x][y].state = state;
        Tiles[x][y].GetComponent<MeshRenderer>().material = newState;
    }

    public void SetTileState(TileState state, (int x, int y) tile)
    {
        if (tile.x == -1 || tile.y == -1) return;
        Material newState = state switch
        {
            TileState.empty => Empty,
            TileState.preview => Preview,
            TileState.move => Move,
            _ => null
        };
        Tiles[tile.x][tile.y].state = state;
        Tiles[tile.x][tile.y].GetComponent<MeshRenderer>().material = newState;
    }
    public void ResetAllTileStates()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Tiles[i][j].state = TileState.empty;
                Tiles[i][j].GetComponent<MeshRenderer>().material = Empty;
            }
        }
    }

    public void ResetSpecificTileStates(TileState state)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (Tiles[i][j].state == state)
                {
                    Tiles[i][j].state = TileState.empty;
                    Tiles[i][j].GetComponent<MeshRenderer>().material = Empty;
                }                
            }
        }
    }

    public void MakeTiles()
    {
        for(int i = 0; i < 8; i++)
        {
            Tiles.Add(new List<Tile>());
            for (int j = 0; j < 8; j++)
            {
                Tile tile = Instantiate(OriginalTile, OriginalTile.transform.parent);
                tile.transform.localPosition = new Vector3(originalPos.x + originalSize * j, originalPos.y + originalSize * i, originalPos.z);
                tile.gameObject.SetActive(true);
                tile.Create(i, j);
                Tiles[i].Add(tile);
            }
        }
    }

    public string GetFenTable()
    {
        string table = "";
        for (int i = 7; i >= 0; i--)
        {
            int emptyCnt = 0;
            for (int j = 0; j < 8; j++)
            {
                bool empty = Tiles[i][j].currentPiece == null;
                if(empty) emptyCnt++;

                if(!empty)
                {
                    if(emptyCnt > 0)
                    {
                        table += emptyCnt.ToString();
                        emptyCnt = 0;
                    }
                    table += Tiles[i][j].currentPiece.GetFenChar();
                }
            }
            if(emptyCnt > 0)
            {
                table += emptyCnt.ToString();
            }
            table += "/";
        }
        table = table[..^1];
        return table;
    }
}
