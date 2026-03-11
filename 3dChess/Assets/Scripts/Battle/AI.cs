using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
    private static readonly WaitForSeconds _waitForSeconds2 = new(2);
    public Coroutine ThinkBattleMove;
    public Coroutine ThinkChessMove;
    public ChessEngine engine;
    public bool TestMode = false;

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
            Debug.LogError("Ai is making a move " + Ref.BattleManager.Turn);

            var decision = ChooseBattleAction();

            switch (decision.Type)
            {
                case AIActionType.Move:
                    Ref.BattleManager.PrepareUseMove(decision.Move, false);
                    break;

                case AIActionType.Potion:
                    Ref.BattleManager.PrepareUsePotion(decision.Potion, false);
                    break;

                case AIActionType.Switch:
                    Ref.BattleManager.PrepareSwitchPiece(decision.SwitchPiece, false);
                    break;

                case AIActionType.Flee:
                    Ref.BattleManager.FleeBattle(false);
                    break;
            }
        }     
    }

    public void CreateChess()
    {
        if (TestMode)
            return;
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
                Ref.ChessManager.PrepareMove(false, piecesToAttack[UnityEngine.Random.Range(0, piecesToAttack.Count)], whiteKingTile);
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

    public AIDecision ChooseBattleAction()
    {
        Dictionary<Move, int> moveChoices = new();
        Dictionary<PotionData, int> potionChoices = new();
        Dictionary<Piece, int> switchPieceChoices = new();
        int fleeChoice = 0;
        

        foreach (var potion in Ref.ChessManager.BlackData.Potions)
        {
            potionChoices.Add(potion, 0);
        }
        foreach(var move in Ref.BattleManager.ActiveBlackPlayer.Moves.Where(m => m.Count > 0))
        {
            moveChoices.Add(move, 0);
        }
        foreach(var piece in Ref.BattleManager.blackTeam)
        {
            if (!piece.Value.Item2)
            {
                switchPieceChoices.Add(piece.Key, 0);
            }
        }

        var whitePlayer = Ref.BattleManager.ActiveWhitePlayer;
        var blackPlayer = Ref.BattleManager.ActiveBlackPlayer;

        var botHealthQuality = 100 * (float)blackPlayer.Health / blackPlayer.MaxHealth;
        var playerHealthQuality = 100 * (float)whitePlayer.Health / whitePlayer.MaxHealth;
        
        //Potions
        TryAddScore(potionChoices, m => m.Name == "Heal", Math.Min(blackPlayer.MaxHealth - blackPlayer.Health, (int)(Variants.HealFactor * blackPlayer.MaxHealth)));
        TryAddScore(potionChoices, m => m.Name == "Cleanse", 50 * Ref.BattleManager.EffectManager.GetEffectCount(false, false, null));

        //Moves general
        foreach(var actionMove in blackPlayer.Moves.Where(m => m.Count > 0))
        {
            if (actionMove.Type != MoveType.Poison)
            {
                moveChoices[actionMove] += (int)actionMove.Action;
            }
            else
            {
                moveChoices[actionMove] += 4 * (int)actionMove.Action;
            }
        }

        //Heal
        if (botHealthQuality < 70)
            TryAddScore(moveChoices, m => m.Type == MoveType.Heal, 20);
        if (botHealthQuality < 50)
            TryAddScore(moveChoices, m => m.Type == MoveType.Heal, 35);
        if (botHealthQuality < 30)
            TryAddScore(moveChoices, m => m.Type == MoveType.Heal, 60);

        //Reward attack more if player is low on health, and punish heal if bot is high on health
        if (botHealthQuality > 90)
        {
            TryAddScore(moveChoices, m => m.Type == MoveType.Heal, -30);
            TryAddScore(moveChoices, m => m.Type == MoveType.Attack, 40);
            TryAddScore(potionChoices, m => m.Name == "Heal", -30);
        }
        if (botHealthQuality > 80)
        {
            TryAddScore(moveChoices, m => m.Type == MoveType.Heal, -20);
            TryAddScore(moveChoices, m => m.Type == MoveType.Attack, 30);
            TryAddScore(potionChoices, m => m.Name == "Heal", -20);
        }
        if (botHealthQuality > 70)
        {
            TryAddScore(moveChoices, m => m.Type == MoveType.Heal, -10);
            TryAddScore(moveChoices, m => m.Type == MoveType.Attack, 20);
            TryAddScore(potionChoices, m => m.Name == "Heal", -10);
        }

        //Attack
        TryAddScore(moveChoices, m => m.Type == MoveType.Attack, 100 - (int)playerHealthQuality);

        //Poison
        TryAddScore(moveChoices, m => m.Type == MoveType.Poison, -100 * Ref.BattleManager.EffectManager.GetEffectCount(true, null, Effect.Type.Poison));
        TryAddScore(moveChoices, m => m.Type == MoveType.Poison, (int)playerHealthQuality);


        //Switch Piece
        foreach (var piece in switchPieceChoices.ToList())
        {
             switchPieceChoices[piece.Key] += piece.Key.Health / 10 + 100 - (int)botHealthQuality;
        }

        print("Scores: \n" +
            "\n Moves \n" + string.Join(", ", moveChoices.Select(m => $"{m.Key.Name}: {m.Value}")) +
            "\n Potions \n" + string.Join(", ", potionChoices.Select(m => $"{m.Key.Name}: {m.Value}")) +
            "\n Switch \n" + string.Join(", ", switchPieceChoices.Select(m => $"{m.Key.Name}: {m.Value}")) +
            "\n Flee \n" + fleeChoice
            );

        var bestMove = moveChoices.OrderByDescending(x => x.Value).FirstOrDefault();
        var bestPotion = potionChoices.OrderByDescending(x => x.Value).FirstOrDefault();
        var bestSwitch = switchPieceChoices.OrderByDescending(x => x.Value).FirstOrDefault();

        int bestScore = int.MinValue;
        AIDecision decision = new AIDecision();

        if (bestMove.Value > bestScore)
        {
            bestScore = bestMove.Value;
            decision.Type = AIActionType.Move;
            decision.Move = bestMove.Key;
        }

        if (bestPotion.Value > bestScore)
        {
            bestScore = bestPotion.Value;
            decision.Type = AIActionType.Potion;
            decision.Potion = bestPotion.Key;
        }

        if (bestSwitch.Value > bestScore)
        {
            bestScore = bestSwitch.Value;
            decision.Type = AIActionType.Switch;
            decision.SwitchPiece = bestSwitch.Key;
        }

        if (fleeChoice > bestScore)
        {
            bestScore = fleeChoice;
            decision.Type = AIActionType.Flee;
        }

        return decision;
    }

    private static void TryAddScore<T>(Dictionary<T, int> choices, Func<T, bool> filter, int score)
    {
        var matchingKeys = choices
            .Where(pair => filter(pair.Key))
            .Select(pair => pair.Key)
            .ToList();

        foreach (var key in matchingKeys)
        {
            choices[key] += score;
        }
    }
}

public enum AIActionType
{
    Move,
    Potion,
    Switch,
    Flee
}

public class AIDecision
{
    public AIActionType Type;

    public Move Move;
    public PotionData Potion;
    public Piece SwitchPiece;
    public bool Flee;
}
