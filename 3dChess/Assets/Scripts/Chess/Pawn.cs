using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    public List<(int, int)> pos = new() { (1, 0), (2, 0) };
    public override List<Tile> GetCurrentPreviewTiles(Tile tile)
    {
        //orgTile = tile;
        Preview.Clear();
        foreach (var newPos in pos)
        {
            int x = tile.x + newPos.Item1;
            int y = tile.y + newPos.Item2;

            if (newPos.Item1 == 2 && movesCnt >= 1) continue;
            if (x < 0 || y < 0 || x > 7 || y > 7) continue;

            if (Ref.ManageTiles.GetTile(x, y).currentPiece == null)
                Preview.Add(Ref.ManageTiles.GetTile(x, y));
            else
                break;
        }

        List<(int, int)> attack = new() { (1, -1), (1, 1) };
        foreach(var attackPos in attack)
        {
            int x = tile.x + attackPos.Item1;
            int y = tile.y + attackPos.Item2;

            if (x < 0 || y < 0 || x > 7 || y > 7) continue;
            if(Ref.ManageTiles.GetTile(x, y).currentPiece != null && Ref.ManageTiles.GetTile(x, y).currentPiece.side != side)
                Preview.Add(Ref.ManageTiles.GetTile(x, y));
        }

        return Preview;
    }

    public override List<Tile> GetCurrentAttackTiles(Tile tile)
    {
        //orgTile = tile;
        Preview.Clear();

        List<(int, int)> attack = new() { (side ? 1 : -1, -1), (side ? 1 : -1, 1) };
        foreach (var attackPos in attack)
        {
            int x = tile.x + attackPos.Item1;
            int y = tile.y + attackPos.Item2;

            if (x < 0 || y < 0 || x > 7 || y > 7) continue;
            Preview.Add(Ref.ManageTiles.GetTile(x, y));
        }

        return Preview;
    }

    public override List<Tile> GetCurrentHelpingTiles(Tile tile)
    {
        Preview.Clear();
        foreach (var newPos in pos)
        {
            int x = tile.x + newPos.Item1;
            int y = tile.y + newPos.Item2;

            if (newPos.Item1 == 2 && movesCnt >= 1) continue;
            if (x < 0 || y < 0 || x > 7 || y > 7) continue;

            if (Ref.ManageTiles.GetTile(x, y).currentPiece == null)
                Preview.Add(Ref.ManageTiles.GetTile(x, y));
            else
                break;
        }

        List<(int, int)> attack = new() { (1, -1), (1, 1) };
        foreach (var attackPos in attack)
        {
            int x = tile.x + attackPos.Item1;
            int y = tile.y + attackPos.Item2;

            if (x < 0 || y < 0 || x > 7 || y > 7) continue;
            Preview.Add(Ref.ManageTiles.GetTile(x, y));
        }

        return Preview;
    }
}
