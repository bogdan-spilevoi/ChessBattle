using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Effect 
{
    public enum Type
    {
        Defense,
        Weaken,
        Evasion,
        Slow,
        Poison,
        Exp
    }

    public Effect(Type type, int rounds, float action, bool affectedSide)
    {
        this.type = type;
        this.rounds = rounds;
        this.action = action;
        AffectedSide = affectedSide;
    }

    public Type type;
    public int rounds;
    public float action;
    public bool AffectedSide;

    public Action OnTurnPass;

    public bool IsPositive()
    {
        return type switch
        {
            Type.Defense => true,
            Type.Weaken => false,
            Type.Evasion => true,
            Type.Slow => false,
            Type.Poison => false,
            Type.Exp => true,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
