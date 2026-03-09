using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private readonly static WaitForSeconds _waitForSeconds1 = new(1);
    private readonly static WaitForSeconds _waitForSeconds0_5 = new(0.5f);

    public EffectManager EffectManager;
    public Piece Original1, Original2;

    public Piece ActiveWhitePlayer { get { return whiteTeam.Single(p => p.Value.Item2).Key; } }
    public Piece ActiveBlackPlayer { get { return blackTeam.Single(p => p.Value.Item2).Key; } }
    public Piece GetActivePlayer(bool side)
    {
        return side ? ActiveWhitePlayer : ActiveBlackPlayer;
    }


    public Dictionary<Piece, (GameObject, bool)> whiteTeam = new(), blackTeam = new();
    public Dictionary<Piece, (GameObject, bool)> GetTeam(bool side)
    {
        return side ? whiteTeam : blackTeam;
    }


    public Transform PosLeft, PosRight, PosLeftBehind, PosRightBehind;
    public Transform GetPos(bool side, bool behind = false)
    {
        if (side)
            return behind ? PosLeftBehind : PosLeft;
        else
            return behind ? PosRightBehind : PosRight;
    }

    public Tile contestedTile;
    public int Turn;
    public BattleResult Result = BattleResult.Empty;

    public static Action<bool> OnTurnEnd;

    public static bool Ongoing = false;

    public TMP_Text T_Animtation;

    public enum BattleResult
    {
        Empty = -1,
        WonSide0 = 0, // black
        WonSide1 = 1, // white
        Fleed = 2,
    }

    public void StartBattle(Piece attacker, Piece defender, Tile t, bool start)
    {
        Turn = attacker.side ? 0 : 1;

        Ref.AI.CreateBattle();  
        Debug.LogError("Starting battle between " + attacker.name + " and " + defender.name + " at tile " + t.x + "," + t.y + " at side " + start);
        
        Original1 = attacker;
        Original2 = defender;

        var whiteStarterPiece = attacker.side == true ? attacker : defender;
        var blackStarterPiece = attacker.side == true ? defender : attacker;
        contestedTile = t;   

        var whiteHelperTeam = FindObjectsOfType<Piece>().Where(e => e.side == whiteStarterPiece.side && e.GetCurrentHelpingTiles(e.currentTile).Contains(t) && e.gameObject.activeInHierarchy && e != whiteStarterPiece).ToList();
        var blackHelperTeam = FindObjectsOfType<Piece>().Where(e => e.side == blackStarterPiece.side && e.GetCurrentHelpingTiles(e.currentTile).Contains(t) && e.gameObject.activeInHierarchy && e != blackStarterPiece).ToList();

        whiteTeam.Clear();
        blackTeam.Clear();

        print(string.Join(", ", whiteHelperTeam.Select(p => p.name)) + " vs " + string.Join(", ", blackHelperTeam.Select(p => p.name)));

        var whiteShowPiece = CreateNewShowPiece(whiteStarterPiece.gameObject, GetPos(ChessManager.Side));
        var blackShowPiece = CreateNewShowPiece(blackStarterPiece.gameObject, GetPos(!ChessManager.Side));    

        foreach (var otherPiece in whiteHelperTeam)
        {
            var newPiece = CreateNewShowPiece(otherPiece.gameObject, GetPos(ChessManager.Side, true));
            whiteTeam.Add(otherPiece, (newPiece, false));
        }
        foreach (var otherPiece in blackHelperTeam)
        {
            var newPiece = CreateNewShowPiece(otherPiece.gameObject, GetPos(!ChessManager.Side, true));
            blackTeam.Add(otherPiece, (newPiece, false));
        }

        whiteTeam.Add(whiteStarterPiece, (whiteShowPiece, true));
        blackTeam.Add(blackStarterPiece, (blackShowPiece, true));

        Ref.Camera.gameObject.SetActive(false);
        Ref.BattleCamera.gameObject.SetActive(true);

        Ref.BattleUI.gameObject.SetActive(true);
        Ref.BattleUI.Create(this);
        
    }

    public void EndBattle()
    {
        Ongoing = false;
        Ref.BattleUI.gameObject.SetActive(false);

        Ref.Camera.gameObject.SetActive(true);
        Ref.BattleCamera.gameObject.SetActive(false);

        if(ChessManager.Local)
            Ref.AI.StopBattle();       

        foreach(var p in whiteTeam.Concat(blackTeam))
        {
            p.Key.UpdateUI();
            p.Key.OnIncludedBattleEnd();
            p.Key.OnIncludedBattleEnd();
            Destroy(p.Value.Item1);
        }
        whiteTeam.Clear();
        blackTeam.Clear();

        EffectManager.ClearAll();

        ChessManager.IncreaseTurn();
    }

    public GameObject CreateNewShowPiece(GameObject p, Transform pos)
    {
        var newPiece = Instantiate(p);
        float y = newPiece.GetComponent<Piece>().normalY;
        Destroy(newPiece.GetComponent<Piece>());
        Destroy(newPiece.transform.GetChild(0).gameObject);
        newPiece.transform.SetParent(transform);
        newPiece.transform.position = new Vector3(pos.transform.position.x, y, pos.transform.position.z);
        return newPiece;
    }

    #region Potion

    public void PrepareUsePotion(PotionData potionData, bool side)
    {     
        Ref.BattleUI.UpdateUI();
        var player = GetActivePlayer(side);
        var pieceInd = Ref.ChessManager.GetPieceIndex(side, player);
        Ref.CommandManager.AddCommandLocal(new UsePotionCommand(side, pieceInd, potionData.Position));
    }

    public void ProcessPotion(bool side, int pieceInd, int potionInd)
    {
        IncreaseTurn();
        Ref.BattleUI.UpdateUI();
        var player = GetActivePlayer(side);
        var potion = Ref.ChessManager.GetPotionByIndex(side, potionInd);
        StartCoroutine(ProcessPotionCor(player, potion, side));
    }

    private IEnumerator ProcessPotionCor(Entity entity, PotionData potion, bool side)
    {
        print("Processing potion " + potion.Name + " for player " + entity.Name + " at side " + side);
        if (side != ChessManager.Side)
        {
            Ref.OpponentMoveGraphic.Create(potion);
        }

        switch(potion.Name)
        {
            case "Heal":            
                int toGive = (int)(entity.MaxHealth * 0.4);                
                entity.GiveHealth(toGive);

                EffectManager.TextAnimation(T_Animtation, side, "+" + toGive, Color.green, new(0, 0.2f, -0.15f));
                EffectManager.DeliverHitEffect(MoveType.Heal, side);

                yield return _waitForSeconds0_5;
                Ref.BattleUI.UpdateHealth();
                yield return _waitForSeconds0_5;
                break;

            case "Cleanse":
                EffectManager.ClearBadForSide(side);
                EffectManager.TextAnimation(T_Animtation, side, "Cleaned", Color.yellow, new(0, 0.2f, -0.15f));

                yield return _waitForSeconds0_5;
                Ref.BattleUI.UpdateHealth();
                yield return _waitForSeconds0_5;
                break;

            case "Exp":
                EffectManager.DeliverEffect(Effect.Type.Exp, side, 5, 2);
                EffectManager.TextAnimation(T_Animtation, side, "x2 Exp", Color.cyan, new(0, 0.2f, -0.15f));

                yield return _waitForSeconds0_5;
                Ref.BattleUI.UpdateHealth();
                yield return _waitForSeconds0_5;
                break;
        }



        Ref.ChessManager.RemovePotionAtIndex(side, potion.Position);

        EndOfTurn(side);
    }
    #endregion

    #region Attack
    public void PrepareUseMove(Move m, bool attackingSide)
    {
        var player = GetActivePlayer(attackingSide);
        print("Preparing move " + m.Name + " for player " + player.Name + " at side " + attackingSide);
        var pieceInd = Ref.ChessManager.GetPieceIndex(attackingSide, player);
        Ref.CommandManager.AddCommandLocal(new UseMoveCommand(attackingSide, pieceInd, player.GetMoveIndex(m), (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue)));
    }    

    public void ProcessMove(bool attackingSide, int pieceInd, int moveIndex, Rng32 rng)
    {
        var player = GetActivePlayer(attackingSide);
        var move = player.GetMoveByIndex(moveIndex);
        ProcessMove(move, attackingSide, rng);
    }

    public void ProcessMove(Move m, bool attackingSide, Rng32 rng)
    {
        var list0 = Turn % 2 == 0 ? whiteTeam : blackTeam;
        var list1 = Turn % 2 != 0 ? whiteTeam : blackTeam;

        var attacker = list0.Single(p => p.Value.Item2);
        var defender = list1.Single(p => p.Value.Item2);

        IncreaseTurn();

        Ref.BattleUI.UpdateUI();

        StartCoroutine(ProcessMoveCor((attacker.Key, attacker.Value.Item1), (defender.Key, defender.Value.Item1), m, attackingSide, rng));
    }

    private IEnumerator ProcessMoveCor((Entity, GameObject) att, (Entity, GameObject) deff, Move m, bool attackingSide, Rng32 rng)
    {
        if(attackingSide != ChessManager.Side)
        {
            Ref.OpponentMoveGraphic.Create(m);
        }

        var attacker = att.Item1;
        var defender = deff.Item1;
        var attackerObj = att.Item2;
        var defenderObj = deff.Item2;

        Vector3 initialPos = attackerObj.transform.position;
        

        switch (m.Type)
        {
            case MoveType.Attack:
                {
                    Tween.Position(attackerObj.transform, defenderObj.transform.position, 0.5f, 0, Tween.EaseIn);
                    print("Extra: " + (float)attacker.Attack * 2 / 100 * m.Action + " " + (int)((float)attacker.Attack * 2 / 100 * m.Action));
                    int damage = (int)m.Action + (int)((float)attacker.Attack * 2/100 * m.Action) + rng.RangeInt(0, attacker.HiddenStat);   

                    int defenderDefensePercent = EffectManager.HasEffect(!attackingSide, Effect.Type.Defense) ? (int)EffectManager.GetEffectTypeAction(!attackingSide, Effect.Type.Defense) : 0;
                    int attackerWeakenPercent = EffectManager.HasEffect(attackingSide, Effect.Type.Weaken) ? (int)EffectManager.GetEffectTypeAction(attackingSide, Effect.Type.Weaken) : 0;
                    int attackerSlow = EffectManager.HasEffect(attackingSide, Effect.Type.Slow) ? (int)EffectManager.GetEffectTypeAction(attackingSide, Effect.Type.Slow) : 0;
                    int defenderEvasion = EffectManager.HasEffect(!attackingSide, Effect.Type.Evasion) ? (int)EffectManager.GetEffectTypeAction(!attackingSide, Effect.Type.Evasion) : 0;
                    int defenderExp = EffectManager.HasEffect(!attackingSide, Effect.Type.Exp) ? (int)EffectManager.GetEffectTypeAction(!attackingSide, Effect.Type.Exp) : 1;
                    //print(defenderDefensePercent + " " + attackerWeakenPercent + " " + attackerSlow + " " + defenderEvasion + " " + defenderPoison);

                    int orgDamage = damage;
                    damage -= (int)(((float)defenderDefensePercent + attackerWeakenPercent) / 100 * orgDamage);

                    string damageInfo = "-" + damage;
                    if (rng.Percent(attacker.Luck * 5 - attackerSlow))
                    {
                        damage *= 2;
                        damageInfo = "-" + damage + " CRIT";
                    }
                    if (rng.Percent(defender.Speed * 5 + defenderEvasion))
                    {
                        damage = 0;
                        damageInfo = "EVADE";
                    }
                    

                    defender.Health -= damage;
                    attacker.GiveExp(m.Action / 10 * defenderExp);
                    if (defender.Health <= 0)
                        attacker.GiveExp(30 * defenderExp);

                    if (defender.Health < 0)
                        defender.Health = 0;
                
                    yield return _waitForSeconds0_5;
                    EffectManager.TextAnimation(T_Animtation, !attackingSide, damageInfo, Color.red, new(0, 0.2f, -0.15f));
                    Ref.BattleUI.UpdateHealth();
                    Tween.Position(attackerObj.transform, initialPos, 0.5f, 0, Tween.EaseOut);
                    yield return _waitForSeconds0_5;
                    break;
                }
            case MoveType.Heal:
                {                   
                    attacker.GiveHealth((int)m.Action);

                    EffectManager.TextAnimation(T_Animtation, attackingSide, "+" + (int)m.Action, Color.green, new(0, 0.2f, -0.15f));
                    EffectManager.DeliverHitEffect(MoveType.Heal, attackingSide);

                    yield return _waitForSeconds1;
                    Ref.BattleUI.UpdateHealth();
                    break;
                }
            case MoveType.Defense:
                {

                    EffectManager.DeliverHitEffect(MoveType.Defense, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Defense, attackingSide, 4, m.Action);
                    yield return _waitForSeconds1;
                    Ref.BattleUI.UpdateHealth();
                    break;
                }
            case MoveType.Weaken:
                {

                    EffectManager.DeliverHitEffect(MoveType.Weaken, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Weaken, attackingSide, 4, m.Action);
                    yield return _waitForSeconds1;
                    Ref.BattleUI.UpdateHealth();
                    break;
                }
            case MoveType.Slow:
                {

                    EffectManager.DeliverHitEffect(MoveType.Slow, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Slow, attackingSide, 4, m.Action);
                    yield return _waitForSeconds1;
                    Ref.BattleUI.UpdateHealth();
                    break;
                }
            case MoveType.Evasion:
                {

                    EffectManager.DeliverHitEffect(MoveType.Evasion, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Evasion, attackingSide, 4, m.Action);
                    yield return _waitForSeconds1;
                    Ref.BattleUI.UpdateHealth();
                    break;
                }
            case MoveType.Poison:
                {
                    EffectManager.DeliverHitEffect(MoveType.Poison, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Poison, attackingSide, 4, m.Action);
                    yield return _waitForSeconds1;
                    Ref.BattleUI.UpdateHealth();
                    break;
                }
        }

        m.Count--;

        EndOfTurn(attackingSide);
    }

    #endregion

    #region SwitchPiece

    public void PrepareSwitchPiece(Piece e, bool side)
    {      
        var player = GetActivePlayer(side);
        Ref.CommandManager.AddCommandLocal(new SwitchPieceCommand(side, Ref.ChessManager.GetPieceIndex(side, player), Ref.ChessManager.GetPieceIndex(side, e)));
    }

    public void SwitchPiece(bool side, int pieceInd, int newPieceInd)
    {
        IncreaseTurn();
        var player = GetActivePlayer(side);
        var piece = Ref.ChessManager.GetPieceByIndex(side, pieceInd);
        var newPiece = Ref.ChessManager.GetPieceByIndex(side, newPieceInd);
       SwitchPiece(newPiece, side);
    }

    public void SwitchPiece(Piece e, bool side)
    {
        StartCoroutine(SwitchPieceCor(e, side));
    }

    private IEnumerator SwitchPieceCor(Piece e, bool side)
    {      
        var team = side ? whiteTeam : blackTeam;

        var entry = team.Single(p => p.Value.Item2);
        team[entry.Key] = (entry.Value.Item1, false);

        var newEntry = team.Single(p => p.Key == e);
        team[newEntry.Key] = (newEntry.Value.Item1, true);

        if (side != ChessManager.Side)
        {
            Ref.OpponentMoveGraphic.Create(entry.Key, newEntry.Key);
        }

        var posToGoTo = GetPos(ChessManager.Side == side, true);
        var posToGoBackTo = GetPos(ChessManager.Side == side);

        Tween.Position(entry.Value.Item1.transform, posToGoTo.position, 0.5f, 0, Tween.EaseIn);
        yield return new WaitForSeconds(0.75f);
        Tween.Position(newEntry.Value.Item1.transform, posToGoBackTo.position, 0.5f, 0, Tween.EaseOut);
        yield return _waitForSeconds0_5;

        EndOfTurn(side);
    }
    #endregion

    #region Flee
    public void PrepareFleeBattle()
    {
        Ref.CommandManager.AddCommandLocal(new FleeBattleCommand(ChessManager.Side));
    }

    public void FleeBattle(bool side)
    {
        Result = BattleResult.Fleed;
        Ref.BattleUI.ToggleOverlay(true);

        if(side != ChessManager.Side)
        {
            Ref.OpponentMoveGraphic.Create();
        }
        
        this.ActionAfterTime(2, () => EndBattle());
    }

    #endregion

    #region Helpers

    public void IncreaseTurn()
    {
        Debug.LogWarning("Battle turn increased");
        Turn++;
    }

    public void EndOfTurn(bool sideEnding)
    {
        print("Ending turn for " + sideEnding);
        ManagePoison(sideEnding);
        Ref.BattleUI.Create(this);
        OnTurnEnd?.Invoke((Turn - 1) % 2 == 0);

        ManagePieceDeath(sideEnding);
    }

    public void ManagePoison(bool attackingSide)
    {
        var sideReceiving = !attackingSide;
        print("managing posin for side " + sideReceiving + " " + EffectManager.HasEffect(sideReceiving, Effect.Type.Poison) + " ");
        if(EffectManager.HasEffect(sideReceiving, Effect.Type.Poison))
        {
            int defenderPoison = (int)EffectManager.GetEffectTypeAction(sideReceiving, Effect.Type.Poison);

            GetActivePlayer(sideReceiving).Health -= defenderPoison;

            EffectManager.TextAnimation(T_Animtation, sideReceiving, "-" + defenderPoison, Color.magenta, new(0, 0.2f, -0.15f));
            Ref.BattleUI.UpdateHealth();
        }
    }

    public void ManagePieceDeath(bool attackingSide)
    {
        if (Original1.Health <= 0 || Original2.Health <= 0)
        {
            var defeatedPiece = Original1.Health <= 0 ? Original1 : Original2;
            var winningPiece = Original1.Health > 0 ? Original1 : Original2;
            bool sideOfDefeatedPiece = defeatedPiece.side;

            print("Side of defeated piece " + sideOfDefeatedPiece);
            var winningTeam = sideOfDefeatedPiece ? whiteTeam : blackTeam;

            Result = sideOfDefeatedPiece == ChessManager.Side ? BattleResult.WonSide0 : BattleResult.WonSide1;

            foreach (var pair in winningTeam)
            {
                pair.Key.GiveExp(15);
            }

            defeatedPiece.Defeat();
            winningPiece.GoToTile(contestedTile);

            EndBattle();

            if (defeatedPiece.GetType() == typeof(King))
            {
                Ref.ChessManager.EndMatch(!sideOfDefeatedPiece);
            }
        }
    }

    #endregion
}
