using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds2 = new(2);
    public Coroutine ThinkBattleMove;
    public Coroutine ThinkChessMove;
    public ChessEngine engine;

    public void CreateBattle()
    {
        if (!ChessManager.Local)
            return;
        ThinkBattleMove = StartCoroutine(MakeMove());
    }
    public void StopBattle()
    {
        StopCoroutine(ThinkBattleMove);
    }

    private IEnumerator MakeMove()
    {
        while (true)
        {
            yield return new WaitUntil(() => Ref.BattleManager.Turn % 2 == 1);
            yield return _waitForSeconds2;
            Debug.LogError("Ai is making a move");
            Ref.BattleManager.PrepareUseMove(Ref.BattleManager.ActivePlayer2.Moves[Random.Range(0, Ref.BattleManager.ActivePlayer2.Moves.Count)], false);
        }     
    }

    public void CreateChess()
    {
        engine.StartEngine();
        ThinkChessMove = StartCoroutine(MakeChessMove());
    }

    public void StopChess()
    {
        StopCoroutine(ThinkChessMove);
    }

    private IEnumerator MakeChessMove()
    {
        while (true)
        {
            yield return new WaitUntil(() => ChessManager.Turn % 2 == 1 && !BattleManager.Ongoing);

            if(TryAttackWhiteKing(out var piecesToAttack, out var whiteKingTile))
            {
                yield return _waitForSeconds2;
                Ref.ChessManager.PrepareMove(false, piecesToAttack[Random.Range(0, piecesToAttack.Count)], whiteKingTile);
                continue;
            }

            string fen = Ref.ManageTiles.GetFenTable() + " b - - 0 1";
            print(fen);

            var task = engine.GetBestMove(fen, 1000);

            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
                yield break;
            }

            string move = task.Result;

            Tile fromTile = Ref.ManageTiles.GetFenTile(move[..2]);
            Piece fromPiece = fromTile.currentPiece;
            Tile toTile = Ref.ManageTiles.GetFenTile(move[2..]);
            print(fromTile + " " + toTile);
            Ref.ChessManager.PrepareMove(false, fromPiece, toTile);
        }
    }

    public bool TryAttackWhiteKing(out List<Piece> piecesToAtack, out Tile whiteKingTile)
    {
        var whiteKing = Ref.ChessManager.WhitePieces.Find(p => p.Data.PieceType == EntityData.Type.King);
        whiteKingTile = whiteKing.currentTile;
        List<Piece> pieces = new();

        foreach (var piece in Ref.ChessManager.BlackPieces)
        {
            if(piece.gameObject.activeSelf == false)
            {
                continue;
            }
            print(piece.Name + " " + string.Join(",", piece.GetCurrentAttackTiles(piece.currentTile)));
            if(piece.GetCurrentAttackTiles(piece.currentTile).Contains(whiteKing.currentTile))
            {
                whiteKingTile = whiteKing.currentTile;
                pieces.Add(piece);
            }
        }
        piecesToAtack = pieces;
        if(pieces.Count > 0)
        {
            return true;
        }
        return false;
    }
}
