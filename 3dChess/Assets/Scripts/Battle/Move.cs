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

    public Move(string name, string description, MoveType type, MoveRarity rarity, float action, params string[] variants)
    {
        Name = name;
        Description = description;
        Type = type;
        Action = action;
        Rarity = rarity;
        Variants = variants == null ? new() : variants.ToList();
    }

    public static int MoveIndToLvlRequired(int i) => i switch { 0 => 0, 1 => 7, 2 => 15, 3 => 20, _ => 0 };

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