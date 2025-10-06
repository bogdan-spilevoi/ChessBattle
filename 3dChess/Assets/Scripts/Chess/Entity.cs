using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    public EntityData Data;
    public string Name { get { return Data.Name; } }
    public int Health { get { return Data.Health; } set { Data.Health = value; } }
    public int MaxHealth { get { return Data.MaxHealth; } }// max 1000

    public int Attack { get { return Data.Attack; } } // max 10
    public int Speed { get { return Data.Speed; } } // max 10
    public int Luck { get { return Data.Luck; } } // max 10

    public int HiddenStat { get { return Data.HiddenStat; } }

    public string Variant { get { return Data.Variant; } }

    public EntityData.Type PieceType {  get { return Data.PieceType; } }

    public int Level = 1;
    public float Exp;

    // Damage will be between 50 and 200
    // Health will be max 1000
    // How to give Exp
    // +/10 of dmg
    // +30 for kill
    // +15 for won chess battle
    // +25 for won chess game


    public float ExpIncrementFactor => GetType() switch
    {
        Type t when t == typeof(King) => 1,
        Type t when t == typeof(Pawn) => 2,
        Type t when t == typeof(Knight) => 1.5f,
        Type t when t == typeof(Bishop) => 1.2f,
        Type t when t == typeof(Rook) => 0.8f,
        Type t when t == typeof(Queen) => 0.5f,
        _ => 0
    };


    public static readonly List<int> Tresholds = new()
    {
        0,    // 1
        10,   // 2
        40,   // 3
        98,   // 4
        190,  // 5
        321,  // 6
        497,  // 7
        722,  // 8
        1001, // 9
        1337, // 10
        1735, // 11
        2199, // 12
        2732, // 13
        3338, // 14
        4020, // 15
        4782, // 16
        5626, // 17
        6557, // 18
        7577, // 19
        8689  // 20
    };

    public void GiveExp(float amount)
    {
        amount *= ExpIncrementFactor;
        Exp += amount;
        Data.Exp += amount;
    }

    public List<Move> Moves { get { return Data.Moves; } }
    public abstract void UpdateUI();

    public abstract void OnIncludedBattleEnd();
}
