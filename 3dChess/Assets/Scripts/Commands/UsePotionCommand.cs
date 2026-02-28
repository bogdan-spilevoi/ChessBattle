using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePotionCommand : CommandBase
{
    public int PieceInd;
    public int PotionInd;

    public UsePotionCommand() { Type = CommandType.BattleUsePotion; }

    public UsePotionCommand(bool side, int pieceInd, int potionInd) : base(side, CommandType.BattleUsePotion)
    {
        PieceInd = pieceInd;
        PotionInd = potionInd;
    }

    public override void Execute()
    {
        Ref.BattleManager.ProcessPotion(Side, PieceInd, PotionInd);
    }
}
