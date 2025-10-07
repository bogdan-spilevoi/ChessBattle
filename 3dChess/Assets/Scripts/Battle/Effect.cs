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
        Poison
    }

    public Effect(Type type, int rounds, float action)
    {
        this.type = type;
        this.rounds = rounds;
        this.action = action;
    }

    public Type type;
    public int rounds;
    public float action;

    public Action OnTurnPass;
}
