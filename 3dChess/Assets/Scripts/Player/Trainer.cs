using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    public string Name;
    public List<TrainerPieceInfo> Pieces;
    public List<int> Potions;
    [TextArea(10, 10)]
    public string ChallengeText, DefeatedText;
    public bool Defeated;

    private Quaternion InitialRotation;

    private void Awake()
    {
        InitialRotation = transform.rotation;
    }

    public void Create(TrainerData data)
    {
        Name = data.Name;
        Defeated = data.Defeated;
    }

    public void Speak()
    {
        Vector3 direction = (GameRef.PlayerBehaviour.transform.position - transform.position);
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Tween.Rotation(transform, targetRotation, 0.5f, 0, Tween.EaseInOut);
    }

    public void EndSpeak()
    {
        Tween.Rotation(transform, InitialRotation, 0.5f, 0, Tween.EaseInOut);
    }

    public InventoryData GetInventory()
    {
        List<EntityData> pieces = new();

        return new InventoryData() 
        { 
            Pieces = new(Pieces.Select((pieceInfo) => {
                var p = Variants.GetPieceByIndex(pieceInfo.PieceIndex);
                p.Moves = pieceInfo.MoveIndexes.Select(i => MovePool.GetMoveByIndex(p.Variant, i)).ToList();
                p.Position = pieceInfo.Position;
                p.Level = pieceInfo.Level;
                return p;
            })), 
            Potions = new(Potions.Select(p => Variants.GetPotionByIndex(p)))
        };
    }
}

[Serializable]
public struct TrainerPieceInfo
{
    public int PieceIndex;
    public int Position;
    public int Level;
    public List<int> MoveIndexes;
}
