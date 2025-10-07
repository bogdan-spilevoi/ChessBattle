using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public ChessManager ChessManager;
    public BattleUI BattleUI;
    public EffectManager EffectManager;
    public Entity Original1, Original2;
    public Entity player1 { get { return player1Team.Single(p => p.Value.Item2).Key; } }
    public Entity player2 { get { return player2Team.Single(p => p.Value.Item2).Key; } }

    public GameObject P1 { get { return player1Team.Single(p => p.Value.Item2).Value.Item1; } }
    public GameObject P2 { get { return player2Team.Single(p => p.Value.Item2).Value.Item1; } }

    public Dictionary<Entity, (GameObject, bool)> player1Team = new(), player2Team = new();
    public Transform Pos1, Pos2, Pos1Behind, Pos2Behind;
    public Tile contestedTile;
    public int Turn;
    public BattleResult Result = BattleResult.Empty;

    public Action<BattleResult, Tile> OnBattleEnd;
    public static Action<bool> OnMoveEnd;

    public TMP_Text T_Animtation;

    public enum BattleResult
    {
        Empty = -1,
        WonSide0 = 0, // black
        WonSide1 = 1, // white
        Fleed = 2,
    }

    public void StartBattle(Entity e1, Entity e2, Tile t, bool start)
    {
        Ref.AI.Create();     
        OnBattleEnd = null;
        Original1 = e1;
        Original2 = e2;
        var player1 = ((Piece)e1).side ? e1 : e2;
        var player2 = ((Piece)e1).side ? e2 : e1;
        contestedTile = t;
        Turn = start ? 0 : 1;

        var player1team = FindObjectsOfType<Piece>().Where(e => e.side == ((Piece)player1).side && e.GetCurrentHelpingTiles(e.currentTile).Contains(t) && e.gameObject.activeInHierarchy && e != player1).Cast<Entity>().ToList();
        var player2team = FindObjectsOfType<Piece>().Where(e => e.side == ((Piece)player2).side && e.GetCurrentHelpingTiles(e.currentTile).Contains(t) && e.gameObject.activeInHierarchy && e != player2).Cast<Entity>().ToList();

        player1Team.Clear();
        player2Team.Clear();

        var P1 = CreateNewShowPiece(player1.gameObject, Pos1);
        var P2 = CreateNewShowPiece(player2.gameObject, Pos2);

        player1Team.Add(player1, (P1, true));
        player2Team.Add(player2, (P2, true));

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

        Ref.Camera.gameObject.SetActive(false);
        Ref.BattleCamera.gameObject.SetActive(true);

        BattleUI.gameObject.SetActive(true);
        BattleUI.Create(this);
        
    }

    public void FleeBattle()
    {
        Result = BattleResult.Fleed;
        EndBattle();
    }

    public void EndBattle()
    {
       
        BattleUI.gameObject.SetActive(false);

        Ref.Camera.gameObject.SetActive(true);
        Ref.BattleCamera.gameObject.SetActive(false);
        Ref.AI.Stop();       

        foreach(var p in player1Team.Concat(player2Team))
        {
            p.Key.UpdateUI();
            p.Key.OnIncludedBattleEnd();
            p.Key.OnIncludedBattleEnd();
            Destroy(p.Value.Item1);
        }
        player1Team.Clear();
        player2Team.Clear();

        Debug.Log(Result);
        OnBattleEnd?.Invoke(Result, contestedTile);
    }


    public void ProcessMove(Move m, bool attackingSide)
    {
        var list0 = Turn % 2 == 0 ? player1Team : player2Team;
        var list1 = Turn % 2 != 0 ? player1Team : player2Team;

        var attacker = list0.Single(p => p.Value.Item2);
        var defender = list1.Single(p => p.Value.Item2);

        Turn++;
        BattleUI.UpdateUI();

        StartCoroutine(ProcessMoveCor((attacker.Key, attacker.Value.Item1), (defender.Key, defender.Value.Item1), m, attackingSide));
    }

    private IEnumerator ProcessMoveCor((Entity, GameObject) att, (Entity, GameObject) deff, Move m, bool attackingSide)
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
                    int damage = (int)m.Action + (int)((float)attacker.Attack * 2/100 * m.Action) + UnityEngine.Random.Range(0, attacker.HiddenStat);
                    

                    int defenderDefensePercent = EffectManager.HasEffect(!attackingSide, Effect.Type.Defense) ? (int)EffectManager.GetEffecttypeAction(!attackingSide, Effect.Type.Defense) : 0;
                    int attackerWeakenPercent = EffectManager.HasEffect(attackingSide, Effect.Type.Weaken) ? (int)EffectManager.GetEffecttypeAction(attackingSide, Effect.Type.Weaken) : 0;
                    int attackerSlow = EffectManager.HasEffect(attackingSide, Effect.Type.Slow) ? (int)EffectManager.GetEffecttypeAction(attackingSide, Effect.Type.Slow) : 0;
                    int defenderEvasion = EffectManager.HasEffect(!attackingSide, Effect.Type.Evasion) ? (int)EffectManager.GetEffecttypeAction(!attackingSide, Effect.Type.Evasion) : 0;
                    int defenderPoison = EffectManager.HasEffect(!attackingSide, Effect.Type.Poison) ? (int)EffectManager.GetEffecttypeAction(!attackingSide, Effect.Type.Poison) : 0;

                    print(defenderDefensePercent + " " + attackerWeakenPercent + " " + attackerSlow + " " + defenderEvasion + " " + defenderPoison);

                    int orgDamage = damage;
                    damage -= (int)(((float)defenderDefensePercent + attackerWeakenPercent) / 100 * orgDamage);

                    string damageInfo = "-" + damage;
                    if (RandomChance.Percent(attacker.Luck * 5 - attackerSlow))
                    {
                        damage *= 2;
                        damageInfo = "-" + damage + " CRIT";
                    }
                    if (RandomChance.Percent(defender.Speed * 5 + defenderEvasion))
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
                
                    yield return new WaitForSeconds(0.5f);
                    EffectManager.TextAnimation(T_Animtation, !attackingSide, damageInfo, Color.red, new(0, 0.2f, -0.15f));
                    if (defenderPoison > 0)
                    {
                        defender.Health -= defenderPoison;
                        Helper.ActionAfterTime(0.2f, () => { EffectManager.TextAnimation(T_Animtation, !attackingSide, "-" + defenderPoison, Color.magenta, new(0, 0.2f, -0.15f)); });
                    }
                    BattleUI.UpdateHealth();
                    Tween.Position(attackerObj.transform, initialPos, 0.5f, 0, Tween.EaseOut);
                    yield return new WaitForSeconds(0.5f);
                    break;
                }
            case MoveType.Heal:
                {
                    EffectManager.TextAnimation(T_Animtation, attackingSide, "+" + (int)m.Action, Color.green, new(0, 0.2f, -0.15f));
                    attacker.Health += (int)m.Action;
                    if(attacker.Health > attacker.MaxHealth)
                        attacker.Health = attacker.MaxHealth;
                    EffectManager.DeliverHitEffect(MoveType.Heal, attackingSide);
                    yield return new WaitForSeconds(1);
                    BattleUI.UpdateHealth();
                    break;
                }
            case MoveType.Defense:
                {

                    EffectManager.DeliverHitEffect(MoveType.Defense, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Defense, attackingSide, 4, m.Action);
                    yield return new WaitForSeconds(1);
                    BattleUI.UpdateHealth();
                    break;
                }
            case MoveType.Weaken:
                {

                    EffectManager.DeliverHitEffect(MoveType.Weaken, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Weaken, attackingSide, 4, m.Action);
                    yield return new WaitForSeconds(1);
                    BattleUI.UpdateHealth();
                    break;
                }
            case MoveType.Slow:
                {

                    EffectManager.DeliverHitEffect(MoveType.Slow, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Slow, attackingSide, 4, m.Action);
                    yield return new WaitForSeconds(1);
                    BattleUI.UpdateHealth();
                    break;
                }
            case MoveType.Evasion:
                {

                    EffectManager.DeliverHitEffect(MoveType.Evasion, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Evasion, attackingSide, 4, m.Action);
                    yield return new WaitForSeconds(1);
                    BattleUI.UpdateHealth();
                    break;
                }
            case MoveType.Poison:
                {
                    defender.Health -= (int)m.Action;
                    EffectManager.DeliverHitEffect(MoveType.Poison, attackingSide);
                    EffectManager.DeliverEffect(MoveType.Poison, attackingSide, 4, m.Action);
                    EffectManager.TextAnimation(T_Animtation, !attackingSide, "-" + m.Action, Color.magenta, new(0, 0.2f, -0.15f));
                    yield return new WaitForSeconds(1);
                    BattleUI.UpdateHealth();
                    break;
                }
        }

        m.Count--;
        BattleUI.Create(this);
        OnMoveEnd?.Invoke((Turn - 1) % 2 == 0);

        if (Original1.Health <= 0 || Original2.Health <= 0)
        {
            var defeatedPiece = Original1.Health <= 0 ? Original1 : Original2;
            bool sideOfDefeatedPiece = ((Piece)defeatedPiece).side;
            print("Side of defeated piece " + sideOfDefeatedPiece);
            var winningTeam = sideOfDefeatedPiece ? player2Team : player1Team;
            Result = sideOfDefeatedPiece ? BattleResult.WonSide0 : BattleResult.WonSide1;

            foreach (var pair in winningTeam)
            {
                pair.Key.GiveExp(15);
            }
            
            EndBattle();
            
            if(defeatedPiece.GetType() == typeof(King))
            {
                ChessManager.EndMatch(!sideOfDefeatedPiece);
            }
        }
    }  

    public GameObject CreateNewShowPiece(GameObject p, Transform pos)
    {
        var newPiece = Instantiate(p);
        Destroy(newPiece.GetComponent<Piece>());
        Destroy(newPiece.transform.GetChild(0).gameObject);
        newPiece.transform.SetParent(transform);
        newPiece.transform.position = new Vector3(pos.transform.position.x, newPiece.transform.position.y, pos.transform.position.z);
        return newPiece;
    }

    public void SwitchPiece(Entity e, bool side)
    {
        StartCoroutine(SwitchPieceCor(e, side));
    }

    private IEnumerator SwitchPieceCor(Entity e, bool side)
    {
        var team = side ? player1Team : player2Team;

        var entry = team.Single(p => p.Value.Item2);
        team[entry.Key] = (entry.Value.Item1, false);

        var newEntry = team.Single(p => p.Key == e);
        team[newEntry.Key] = (newEntry.Value.Item1, true);

        var posToGoTo = side ? Pos1Behind : Pos2Behind;
        var posToGoBackTo = side ? Pos1 : Pos2;

        Tween.Position(entry.Value.Item1.transform, posToGoTo.position, 0.5f, 0, Tween.EaseIn);
        yield return new WaitForSeconds(0.75f);
        Tween.Position(newEntry.Value.Item1.transform, posToGoBackTo.position, 0.5f, 0, Tween.EaseOut);
        yield return new WaitForSeconds(0.5f);

        int defenderPoison = EffectManager.HasEffect(!side, Effect.Type.Poison) ? (int)EffectManager.GetEffecttypeAction(!side, Effect.Type.Poison) : 0;
        if (defenderPoison > 0)
        {
            player2.Health -= defenderPoison;
            Helper.ActionAfterTime(0.2f, () => { EffectManager.TextAnimation(T_Animtation, !side, "-" + defenderPoison, Color.magenta, new(0, 0.2f, -0.15f)); });
        }

        BattleUI.Create(this);
        Turn++;
        OnMoveEnd?.Invoke((Turn - 1) % 2 == 0);
        BattleUI.UpdateUI();
    }

}
