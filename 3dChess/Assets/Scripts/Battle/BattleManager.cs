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

    public Piece ActivePlayer1 { get { return player1Team.Single(p => p.Value.Item2).Key; } }
    public Piece ActivePlayer2 { get { return player2Team.Single(p => p.Value.Item2).Key; } }

    public GameObject P1 { get { return player1Team.Single(p => p.Value.Item2).Value.Item1; } }
    public GameObject P2 { get { return player2Team.Single(p => p.Value.Item2).Value.Item1; } }

    public Dictionary<Piece, (GameObject, bool)> player1Team = new(), player2Team = new();
    public Transform Pos1, Pos2, Pos1Behind, Pos2Behind;
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

    public void StartBattle(Piece e1, Piece e2, Tile t, bool start)
    {
        Ref.AI.CreateBattle();  
        Debug.LogError("Starting battle between " + e1.name + " and " + e2.name + " at tile " + t.x + "," + t.y + " at side " + start);
        
        Original1 = e1;
        Original2 = e2;
        var player1 = e1.side == ChessManager.Side ? e1 : e2;
        var player2 = e1.side == ChessManager.Side ? e2 : e1;
        contestedTile = t;
        Turn = start == ChessManager.Side ? 0 : 1;

        var player1team = FindObjectsOfType<Piece>().Where(e => e.side == player1.side && e.GetCurrentHelpingTiles(e.currentTile).Contains(t) && e.gameObject.activeInHierarchy && e != player1).ToList();
        var player2team = FindObjectsOfType<Piece>().Where(e => e.side == player2.side && e.GetCurrentHelpingTiles(e.currentTile).Contains(t) && e.gameObject.activeInHierarchy && e != player2).ToList();

        player1Team.Clear();
        player2Team.Clear();

        var P1 = CreateNewShowPiece(player1.gameObject, Pos1);
        var P2 = CreateNewShowPiece(player2.gameObject, Pos2);    

        foreach (var otherPiece in player1team)
        {
            var newPiece = CreateNewShowPiece(otherPiece.gameObject, Pos1Behind);
            player1Team.Add(otherPiece, (newPiece, false));
        }
        foreach (var otherPiece in player2team)
        {
            var newPiece = CreateNewShowPiece(otherPiece.gameObject, Pos2Behind);
            player2Team.Add(otherPiece, (newPiece, false));
        }

        player1Team.Add(player1, (P1, true));
        player2Team.Add(player2, (P2, true));

        Ref.Camera.gameObject.SetActive(false);
        Ref.BattleCamera.gameObject.SetActive(true);

        Ref.BattleUI.gameObject.SetActive(true);
        Ref.BattleUI.Create(this);
        
    }

    public void PrepareFleeBattle()
    {
        Ref.CommandManager.AddCommandLocal(new FleeBattleCommand(ChessManager.Side));
    }

    public void FleeBattle(bool side)
    {
        Result = BattleResult.Fleed;
        EndBattle();
    }

    public void EndBattle()
    {
        Ongoing = false;
        Ref.BattleUI.gameObject.SetActive(false);

        Ref.Camera.gameObject.SetActive(true);
        Ref.BattleCamera.gameObject.SetActive(false);

        if(ChessManager.Local)
            Ref.AI.StopBattle();       

        foreach(var p in player1Team.Concat(player2Team))
        {
            p.Key.UpdateUI();
            p.Key.OnIncludedBattleEnd();
            p.Key.OnIncludedBattleEnd();
            Destroy(p.Value.Item1);
        }
        player1Team.Clear();
        player2Team.Clear();

        ChessManager.IncreaseTurn();
    }

    public void PrepareUsePotion(PotionData potionData, bool side)
    {     
        Ref.BattleUI.UpdateUI();
        var player = side == ChessManager.Side ? ActivePlayer1 : ActivePlayer2;
        var pieceInd = Ref.ChessManager.GetPieceIndex(side, player);
        Ref.CommandManager.AddCommandLocal(new UsePotionCommand(side, pieceInd, potionData.Position));
    }

    public void ProcessPotion(bool side, int pieceInd, int potionInd)
    {
        IncreaseTurn();
        var player = side == ChessManager.Side ? ActivePlayer1 : ActivePlayer2;
        var potion = Ref.ChessManager.GetPotionByIndex(side, potionInd);
        StartCoroutine(ProcessPotionCor(player, potion, ChessManager.Side ? side : !side));
    }

    private IEnumerator ProcessPotionCor(Entity entity, PotionData potion, bool side)
    {
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
        }



        Ref.ChessManager.RemovePotionAtIndex(side, potion.Position);

        Ref.BattleUI.Create(this);
        OnTurnEnd?.Invoke((Turn - 1) % 2 == 0);
    }

    public void PrepareUseMove(Move m, bool attackingSide)
    {
        var player = attackingSide == ChessManager.Side ? ActivePlayer1 : ActivePlayer2;
        print("Preparing move " + m.Name + " for player " + player.Name + " at side " + attackingSide);
        var pieceInd = Ref.ChessManager.GetPieceIndex(attackingSide, player);
        Ref.CommandManager.AddCommandLocal(new UseMoveCommand(attackingSide, pieceInd, player.GetMoveIndex(m), (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue)));
    }    

    public void ProcessMove(bool attackingSide, int pieceInd, int moveIndex, Rng32 rng)
    {
        var player = attackingSide == ChessManager.Side ? ActivePlayer1 : ActivePlayer2;
        var move = player.GetMoveByIndex(moveIndex);
        ProcessMove(move, attackingSide, rng);
    }

    public void ProcessMove(Move m, bool attackingSide, Rng32 rng)
    {
        var list0 = Turn % 2 == 0 ? player1Team : player2Team;
        var list1 = Turn % 2 != 0 ? player1Team : player2Team;

        var attacker = list0.Single(p => p.Value.Item2);
        var defender = list1.Single(p => p.Value.Item2);

        IncreaseTurn();

        Ref.BattleUI.UpdateUI();

        StartCoroutine(ProcessMoveCor((attacker.Key, attacker.Value.Item1), (defender.Key, defender.Value.Item1), m, ChessManager.Side ? attackingSide : !attackingSide , rng));
    }

    private IEnumerator ProcessMoveCor((Entity, GameObject) att, (Entity, GameObject) deff, Move m, bool attackingSide, Rng32 rng)
    {
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

                    int defenderDefensePercent = EffectManager.HasEffect(!attackingSide, Effect.Type.Defense) ? (int)EffectManager.GetEffecttypeAction(!attackingSide, Effect.Type.Defense) : 0;
                    int attackerWeakenPercent = EffectManager.HasEffect(attackingSide, Effect.Type.Weaken) ? (int)EffectManager.GetEffecttypeAction(attackingSide, Effect.Type.Weaken) : 0;
                    int attackerSlow = EffectManager.HasEffect(attackingSide, Effect.Type.Slow) ? (int)EffectManager.GetEffecttypeAction(attackingSide, Effect.Type.Slow) : 0;
                    int defenderEvasion = EffectManager.HasEffect(!attackingSide, Effect.Type.Evasion) ? (int)EffectManager.GetEffecttypeAction(!attackingSide, Effect.Type.Evasion) : 0;
                    int defenderPoison = EffectManager.HasEffect(!attackingSide, Effect.Type.Poison) ? (int)EffectManager.GetEffecttypeAction(!attackingSide, Effect.Type.Poison) : 0;

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
                    attacker.GiveExp(m.Action / 10);
                    if (defender.Health <= 0)
                        attacker.GiveExp(30);

                    if (defender.Health < 0)
                        defender.Health = 0;
                
                    yield return _waitForSeconds0_5;
                    EffectManager.TextAnimation(T_Animtation, !attackingSide, damageInfo, Color.red, new(0, 0.2f, -0.15f));
                    if (defenderPoison > 0)
                    {
                        defender.Health -= defenderPoison;
                        this.ActionAfterTime(0.2f, () => { EffectManager.TextAnimation(T_Animtation, !attackingSide, "-" + defenderPoison, Color.magenta, new(0, 0.2f, -0.15f)); });
                    }
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
                    defender.Health -= (int)m.Action;
                    EffectManager.DeliverHitEffect(MoveType.Poison, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Poison, attackingSide, 4, m.Action);
                    EffectManager.TextAnimation(T_Animtation, !attackingSide, "-" + m.Action, Color.magenta, new(0, 0.2f, -0.15f));
                    yield return _waitForSeconds1;
                    Ref.BattleUI.UpdateHealth();
                    break;
                }
        }

        m.Count--;
        Ref.BattleUI.Create(this);
        OnTurnEnd?.Invoke((Turn - 1) % 2 == 0);

        if (Original1.Health <= 0 || Original2.Health <= 0)
        {
            var defeatedPiece = Original1.Health <= 0 ? Original1 : Original2;
            var winningPiece = Original1.Health > 0 ? Original1 : Original2;
            bool sideOfDefeatedPiece = defeatedPiece.side;
            print("Side of defeated piece " + sideOfDefeatedPiece);
            var winningTeam = sideOfDefeatedPiece == ChessManager.Side ? player2Team : player1Team;
            Result = sideOfDefeatedPiece == ChessManager.Side ? BattleResult.WonSide0 : BattleResult.WonSide1;

            foreach (var pair in winningTeam)
            {
                pair.Key.GiveExp(15);
            }

            defeatedPiece.Defeat();
            winningPiece.GoToTile(contestedTile);

            EndBattle();
            
            if(defeatedPiece.GetType() == typeof(King))
            {
                Ref.ChessManager.EndMatch(!sideOfDefeatedPiece);
            }
        }
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

    public void PrepareSwitchPiece(Piece e, bool side)
    {      
        var player = side == ChessManager.Side ? ActivePlayer1 : ActivePlayer2;
        Ref.CommandManager.AddCommandLocal(new SwitchPieceCommand(side, Ref.ChessManager.GetPieceIndex(side, player), Ref.ChessManager.GetPieceIndex(side, e)));
    }

    public void SwitchPiece(bool side, int pieceInd, int newPieceInd)
    {
        IncreaseTurn();
        var player = side == ChessManager.Side ? ActivePlayer1 : ActivePlayer2;
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
        var team = side == ChessManager.Side ? player1Team : player2Team;

        var entry = team.Single(p => p.Value.Item2);
        team[entry.Key] = (entry.Value.Item1, false);

        var newEntry = team.Single(p => p.Key == e);
        team[newEntry.Key] = (newEntry.Value.Item1, true);

        var posToGoTo = side == ChessManager.Side ? Pos1Behind : Pos2Behind;
        var posToGoBackTo = side == ChessManager.Side ? Pos1 : Pos2;

        Tween.Position(entry.Value.Item1.transform, posToGoTo.position, 0.5f, 0, Tween.EaseIn);
        yield return new WaitForSeconds(0.75f);
        Tween.Position(newEntry.Value.Item1.transform, posToGoBackTo.position, 0.5f, 0, Tween.EaseOut);
        yield return _waitForSeconds0_5;

        int defenderPoison = EffectManager.HasEffect(!side, Effect.Type.Poison) ? (int)EffectManager.GetEffecttypeAction(!side, Effect.Type.Poison) : 0;
        if (defenderPoison > 0)
        {
            ActivePlayer2.Health -= defenderPoison;
            Helper.ActionAfterTime(0.2f, () => { EffectManager.TextAnimation(T_Animtation, !side, "-" + defenderPoison, Color.magenta, new(0, 0.2f, -0.15f)); });
        }

        Ref.BattleUI.Create(this);
        OnTurnEnd?.Invoke((Turn - 1) % 2 == 0);
        Ref.BattleUI.UpdateUI();
    }

    public void IncreaseTurn()
    {
        Debug.LogWarning("Battle turn increased");
        Turn++;
    }
}
