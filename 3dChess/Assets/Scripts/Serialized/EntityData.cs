using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class EntityData
{
    public string Name;
    public Type PieceType;
    public int Position;
    public int Health;
    public int MaxHealth;// max 1000
    
    public int Attack; // max 10
    public int Speed; // max 10
    public int Luck; // max 10

    public string Variant;
    public int HiddenStat; // max 10

    public int Level {
        get
        {
            return GetLevel();
        }
        set
        {

        }
    }
    
    public float Exp;

    public List<Move> Moves = new();

    public enum Type
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }

    public EntityData()
    {

    }

    public EntityData(Entity entity)
    {
        Name = entity.Name;
        Health = entity.Health;
        MaxHealth = entity.MaxHealth;
        Level = entity.Level;
        Moves = new List<Move>();

        foreach (var move in entity.Moves) 
        {
            Moves.Add(move);
        }
    }

    public EntityData Copy()
    {
        return new EntityData(Name, PieceType, Position, Health, MaxHealth, Attack, Speed, Luck, Exp, Variant, Moves.ToArray());
    }

    public EntityData(string Name, Type PieceType, int Position, int Health, int MaxHealth, int Attack, int Speed, int Luck, float Exp, string Variant, params Move[] Moves)
    {
        this.Name = Name;
        this.PieceType = PieceType;
        this.Position = Position;
        this.Health = Health;
        this.MaxHealth = MaxHealth;
        this.Attack = Attack;
        this.Speed = Speed;
        this.Luck = Luck;
        this.Exp = Exp;
        this.Variant = Variant;
        if(Moves != null)
            this.Moves.AddRange(Moves);
    }

    public int GetLevel()
    {
        int i = 0;
        for (i = 0; i < Entity.Tresholds.Count; i++)
        {
            if (Entity.Tresholds[i] > Exp)
            {
                break;
            }
        }
        return i;
    }

    public (int, int) GetExpTresholdBounds()
    {
        int i = 0;
        (int, int) ret = (0, 1);
        bool found = false;

        for (i = 1; i < Entity.Tresholds.Count; i++)
        {
            if (Entity.Tresholds[i] > Exp)
            {
                ret = (Entity.Tresholds[i - 1], Entity.Tresholds[i]);
                found = true;
                break;
            }
        }
        return found ? ret : (Entity.Tresholds.Count, Entity.Tresholds.Count);
    }
}

