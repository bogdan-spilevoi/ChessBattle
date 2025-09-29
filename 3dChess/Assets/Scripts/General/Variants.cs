using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Variants : MonoBehaviour
{
    public static List<EntityData> PiecesVariants = new()
    {
        new EntityData("Pawn", EntityData.Type.Pawn,     -1, 250, 250, 2, 5, 4, 0, "basic", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 275, 275, 3, 4, 3, 0, "basic", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 300, 300, 3, 3, 4, 0, "basic", null),
        new EntityData("Rook", EntityData.Type.Rook,     -1, 450, 450, 2, 2, 4, 0, "basic", null),
        new EntityData("Queen", EntityData.Type.Queen,   -1, 300, 300, 5, 1, 2, 0, "basic", null),
        new EntityData("King", EntityData.Type.King,     -1, 300, 300, 4, 2, 3, 0, "basic", null),


        new EntityData("Pawn",   EntityData.Type.Pawn,   -1, 180, 180, 2, 4, 1, 0, "rust", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 250, 250, 3, 4, 2, 0, "rust", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 280, 280, 3, 3, 2, 0, "rust", null),
        new EntityData("Rook",   EntityData.Type.Rook,   -1, 350, 350, 4, 2, 2, 0, "rust", null),
        new EntityData("Queen",  EntityData.Type.Queen,  -1, 300, 300, 4, 3, 1, 0, "rust", null),
        new EntityData("King",   EntityData.Type.King,   -1, 320, 320, 3, 2, 2, 0, "rust", null),


        new EntityData("Pawn",   EntityData.Type.Pawn,   -1, 260, 260, 3, 4, 3, 0, "stone", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 380, 380, 4, 4, 4, 0, "stone", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 420, 420, 4, 3, 4, 0, "stone", null),
        new EntityData("Rook",   EntityData.Type.Rook,   -1, 500, 500, 5, 2, 4, 0, "stone", null),
        new EntityData("Queen",  EntityData.Type.Queen,  -1, 480, 480, 5, 3, 4, 0, "stone", null),
        new EntityData("King",   EntityData.Type.King,   -1, 520, 520, 4, 2, 4, 0, "stone", null),


        new EntityData("Pawn",   EntityData.Type.Pawn,   -1, 280, 280, 3, 5, 5, 0, "bronze", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 360, 360, 4, 5, 5, 0, "bronze", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 400, 400, 4, 4, 6, 0, "bronze", null),
        new EntityData("Rook",   EntityData.Type.Rook,   -1, 520, 520, 5, 3, 5, 0, "bronze", null),
        new EntityData("Queen",  EntityData.Type.Queen,  -1, 460, 460, 5, 4, 5, 0, "bronze", null),
        new EntityData("King",   EntityData.Type.King,   -1, 500, 500, 4, 3, 5, 0, "bronze", null),


        new EntityData("Pawn",   EntityData.Type.Pawn,   -1, 320, 320, 4, 4, 8, 0, "frost", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 520, 520, 5, 4, 6, 0, "frost", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 650, 650, 5, 3, 8, 0, "frost", null), 
        new EntityData("Rook",   EntityData.Type.Rook,   -1, 850, 850, 4, 2, 5, 0, "frost", null),
        new EntityData("Queen",  EntityData.Type.Queen,  -1, 700, 700, 5, 3, 6, 0, "frost", null),
        new EntityData("King",   EntityData.Type.King,   -1, 900, 900, 3, 2, 5, 0, "frost", null),


        new EntityData("Pawn",   EntityData.Type.Pawn,   -1, 260, 260, 5,10, 6, 0, "storm", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 320, 320, 6,10, 5, 0, "storm", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 360, 360, 5, 9, 8, 0, "storm", null),
        new EntityData("Rook",   EntityData.Type.Rook,   -1, 420, 420, 6, 7, 5, 0, "storm", null),
        new EntityData("Queen",  EntityData.Type.Queen,  -1, 500, 500, 7, 8, 5, 0, "storm", null),
        new EntityData("King",   EntityData.Type.King,   -1, 480, 480, 6, 6, 5, 0, "storm", null),


        new EntityData("Pawn",   EntityData.Type.Pawn,   -1, 380, 380, 6, 5, 8, 0, "terra", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 540, 540, 6, 5, 6, 0, "terra", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 620, 620, 6, 4, 7, 0, "terra", null),
        new EntityData("Rook",   EntityData.Type.Rook,   -1, 900, 900, 5, 3, 5, 0, "terra", null),
        new EntityData("Queen",  EntityData.Type.Queen,  -1, 680, 680, 6, 4, 6, 0, "terra", null),
        new EntityData("King",   EntityData.Type.King,   -1, 800, 800, 4, 3, 6, 0, "terra", null),


        new EntityData("Pawn",   EntityData.Type.Pawn,   -1, 300, 300, 4, 6,10, 0, "aqua", null), 
        new EntityData("Knight", EntityData.Type.Knight, -1, 380, 380, 5, 6, 9, 0, "aqua", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 420, 420, 5, 6,10, 0, "aqua", null),
        new EntityData("Rook",   EntityData.Type.Rook,   -1, 500, 500, 6, 4, 8, 0, "aqua", null),
        new EntityData("Queen",  EntityData.Type.Queen,  -1, 560, 560, 6, 6, 8, 0, "aqua", null),
        new EntityData("King",   EntityData.Type.King,   -1, 600, 600, 5, 5, 8, 0, "aqua", null),


        new EntityData("Pawn",   EntityData.Type.Pawn,   -1, 240, 240, 7, 9, 3, 0, "inferno", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 320, 320, 8, 8, 4, 0, "inferno", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 360, 360, 7, 7, 5, 0, "inferno", null),
        new EntityData("Rook",   EntityData.Type.Rook,   -1, 420, 420, 9, 4, 3, 0, "inferno", null),
        new EntityData("Queen",  EntityData.Type.Queen,  -1, 520, 520, 8, 6, 4, 0, "inferno", null),
        new EntityData("King",   EntityData.Type.King,   -1, 500, 500, 7, 3, 4, 0, "inferno", null),


        new EntityData("Pawn",   EntityData.Type.Pawn,   -1, 240, 240, 6, 7, 8, 0, "void", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 320, 320, 7, 7, 8, 0, "void", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 360, 360, 7, 6, 9, 0, "void", null),
        new EntityData("Rook",   EntityData.Type.Rook,   -1, 420, 420, 8, 5, 7, 0, "void", null),
        new EntityData("Queen",  EntityData.Type.Queen,  -1, 520, 520, 8, 6, 7, 0, "void", null),
        new EntityData("King",   EntityData.Type.King,   -1, 480, 480, 7, 5, 7, 0, "void", null),


        new EntityData("Pawn",   EntityData.Type.Pawn,   -1, 260, 260, 3,10, 9, 0, "radiant", null),
        new EntityData("Knight", EntityData.Type.Knight, -1, 340, 340, 4, 9, 8, 0, "radiant", null),
        new EntityData("Bishop", EntityData.Type.Bishop, -1, 380, 380, 4, 8,10, 0, "radiant", null),
        new EntityData("Rook",   EntityData.Type.Rook,   -1, 460, 460, 5, 7, 8, 0, "radiant", null),
        new EntityData("Queen",  EntityData.Type.Queen,  -1, 520, 520, 6, 8, 8, 0, "radiant", null),
        new EntityData("King",   EntityData.Type.King,   -1, 520, 520, 5, 7, 8, 0, "radiant", null),
    };

    public static EntityData GetRandomOfType(EntityData.Type type)
    {
        var candidates = PiecesVariants.Where(p => p.PieceType == type).ToList();

        if (candidates.Count == 0)
            return null;

        var chosen = candidates[Random.Range(0, candidates.Count)];
        for(int i =0; i< 4; i++)
        {
            chosen.Moves.Add(MovePool.GetRandomMove(chosen.Variant, chosen.PieceType));
        }
        
        return chosen;
    }

    public static EntityData GetRandom()
    {
        var chosen = PiecesVariants[Random.Range(0, PiecesVariants.Count)];
        for (int i = 0; i < 4; i++)
        {
            chosen.Moves.Add(MovePool.GetRandomMove(chosen.Variant, chosen.PieceType));
        }
        return chosen;
    }
    private void Awake()
    {
        VisualizeVariants = PiecesVariants;
    }
    public List<EntityData> VisualizeVariants = new();
}
