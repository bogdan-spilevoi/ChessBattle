using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchPieceCommand : CommandBase
{
    [JsonProperty(Order = 1)]
    public int OriginalPieceInd;
    [JsonProperty(Order = 2)]
    public int NewPieceInd;

    public SwitchPieceCommand(bool side, int originalPieceInd, int newPieceInd) : base(side, CommandType.BattleSwitchPiece)
    {
        OriginalPieceInd = originalPieceInd;
        NewPieceInd = newPieceInd;
    }

    public override void Execute()
    {
        Ref.BattleManager.SwitchPiece(Side, OriginalPieceInd, NewPieceInd);
    }
}
