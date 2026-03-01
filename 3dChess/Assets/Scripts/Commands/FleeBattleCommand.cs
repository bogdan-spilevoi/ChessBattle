using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeBattleCommand : CommandBase
{

    public FleeBattleCommand() { Type = CommandType.BattleFlee; }
    public FleeBattleCommand(bool side) : base(side, CommandType.BattleFlee)
    {

    }

    public override void Execute()
    {
        Ref.BattleManager.FleeBattle(Side);
    }
}
