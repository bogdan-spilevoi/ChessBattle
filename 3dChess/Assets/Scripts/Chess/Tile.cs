using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x, y;
    public ManageTiles.TileState state = ManageTiles.TileState.empty;
    public Piece currentPiece;

    public void Create(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public int GetIndex()
    {
        return x * 8 + y;
    }
    public override string ToString()
    {
        return "[" + x + ", " + y + "]";
    }
}
