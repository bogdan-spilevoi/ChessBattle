using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseMoveCommand : CommandBase
{
    [JsonProperty(Order = 1)]
    public int PieceInd;
    [JsonProperty(Order = 2)]
    public int MoveInd;
    [JsonProperty(Order = 4)]
    public uint Seed;

    public UseMoveCommand() { Type = CommandType.BattleUseMove; }

    public UseMoveCommand(bool side, int pieceInd, int moveInd, uint seed) : base(side, CommandType.BattleUseMove)
    {
        PieceInd = pieceInd;
        MoveInd = moveInd;
        Seed = seed;
    }

    public override void Execute()
    {
        Ref.BattleManager.ProcessMove(Side, PieceInd, MoveInd, new Rng32(Seed));
    }
}
