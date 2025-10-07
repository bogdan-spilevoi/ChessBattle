using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class King : Piece
{
    public List<(int, int)> pos = new() { (1, -1), (1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1) };
    public override List<Tile> GetCurrentPreviewTiles(Tile tile)
    {
        //orgTile = tile;
        Preview.Clear();

        var allOppositePiecesAttacks = FindObjectsOfType<Piece>().Where(p => p.side != side && p.gameObject.activeInHierarchy && p.GetType() != typeof(King)).ToList();
        List<Tile> attackTiles = new();
        foreach(var p in allOppositePiecesAttacks)
        {
            attackTiles.AddRange(p.GetCurrentAttackTiles(Ref.ManageTiles.GetTile(Ref.ManageTiles.GetUnderTile(p.transform.position))));
        }

        foreach (var newPos in pos)
        {
            int x = tile.x + newPos.Item1;
            int y = tile.y + newPos.Item2;

            if (x < 0 || y < 0 || x > 7 || y > 7) continue;
            if (Ref.ManageTiles.GetTile(x, y).currentPiece != null && Ref.ManageTiles.GetTile(x, y).currentPiece.side == side) continue;
            //if (attackTiles.Contains(Ref.ManageTiles.GetTile(x, y))) continue;

            Preview.Add(Ref.ManageTiles.GetTile(x, y));
        }

        return Preview;
    }

    public override List<Tile> GetCurrentAttackTiles(Tile tile)
    {
        return GetCurrentPreviewTiles(tile);
    }

    public override List<Tile> GetCurrentHelpingTiles(Tile tile)
    {
        Preview.Clear();

        var allOppositePiecesAttacks = FindObjectsOfType<Piece>().Where(p => p.side != side && p.gameObject.activeInHierarchy && p.GetType() != typeof(King)).ToList();
        List<Tile> attackTiles = new();
        foreach (var p in allOppositePiecesAttacks)
        {
            attackTiles.AddRange(p.GetCurrentAttackTiles(Ref.ManageTiles.GetTile(Ref.ManageTiles.GetUnderTile(p.transform.position))));
        }

        foreach (var newPos in pos)
        {
            int x = tile.x + newPos.Item1;
            int y = tile.y + newPos.Item2;

            if (x < 0 || y < 0 || x > 7 || y > 7) continue;
            if (attackTiles.Contains(Ref.ManageTiles.GetTile(x, y))) continue;

            Preview.Add(Ref.ManageTiles.GetTile(x, y));
        }
        //Debug.Log("King helping: " + string.Join(", ", Preview));
        return Preview;
    }
}
