using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MoveCommand : CommandBase
{
    [JsonProperty(Order = 1)]
    public int PieceInd;

    [JsonProperty(Order = 2)]
    public int TileInd;

    public MoveCommand() { Type = CommandType.Move; }

    public MoveCommand(bool side, int pieceInd, int tileInd) : base(side, CommandType.Move)
    {
        PieceInd = pieceInd;
        TileInd = tileInd;
    }
    public override void Execute()
    {
        Ref.ChessManager.MovePiece(Side, PieceInd, TileInd);
    }
}
