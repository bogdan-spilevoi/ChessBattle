using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandBase
{
    [JsonProperty(Order = -1)]
    public CommandType Type;

    [JsonProperty(Order = -2)]
    public bool Side;

    protected CommandBase() { }
    public CommandBase(bool side, CommandType type)
    {
        Side = side;
        Type = type;
    }

    public abstract void Execute();
}

public enum CommandType
{
    Move,
    StartBattle,
    BattleUseMove,
    BattleUsePotion,
    BattleSwitchPiece,
    BattleFlee,
    BattleEnd,
}
