using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Move 
{
    public List<string> Variants = new();
    public string Name;
    public string Description;
    public MoveType Type;
    public MoveRarity Rarity;
    public float Action;
    public int Count;

    public Move(string name, string description, MoveType type, MoveRarity rarity, float action, params string[] variants)
    {
        Name = name;
        Description = description;
        Type = type;
        Action = action;
        Rarity = rarity;
        Variants = variants == null ? new() : variants.ToList();
        Count = RarityToMoveCount(rarity);
    }

    public int MaxCount {  get {  return RarityToMoveCount(Rarity); } }
    public static int MoveIndToLvlRequired(int i) => i switch { 0 => 0, 1 => 7, 2 => 15, 3 => 20, _ => 0 };
    public static int RarityToMoveCount(MoveRarity rarity) => rarity switch { MoveRarity.Common => 10, MoveRarity.Rare => 8, MoveRarity.Epic => 6, MoveRarity.Legendary => 4, _ => 10 };

}


public enum MoveType
{
    Attack,
    Heal,
    Poison,
    Defense,
    Weaken,
    Slow,
    Evasion
}

public enum MoveRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}