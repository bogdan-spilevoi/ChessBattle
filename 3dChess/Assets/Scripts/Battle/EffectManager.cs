using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("Hit Effect")]
    public float hitEffectDuration;
    public List<Material> hitEffects = new();
    public ParticleSystem P_HitEffect;

    [Header("Hit Text")]
    public GameObject BattleCam;
    public TMP_Text T_Animtation;

    [Header("Effect UI")]
    public EffectListUI Player1;
    public EffectListUI Player2;
    public List<Effect> P1List = new(), P2List = new();

    private void OnEnable()
    {
        BattleManager.OnTurnEnd += ManageAfterMove;
    }

    private void OnDisable()
    {
        BattleManager.OnTurnEnd -= ManageAfterMove;
    }

    public void ManageAfterMove(bool sideThatDidMove)
    {
        print("Managing effects after move for side " + sideThatDidMove);
        var list = sideThatDidMove == ChessManager.Side ? P1List : P2List;
        var listUI = sideThatDidMove == ChessManager.Side ? Player1 : Player2;

        list.ForEach(e => { e.rounds--; });
        if (sideThatDidMove == ChessManager.Side)
            P1List = list.Where(e => e.rounds > 0).ToList();
        else
            P2List = list.Where(e => e.rounds > 0).ToList();
        listUI.UpdateEffects();
    }

    public float GetEffectTypeAction(bool side, Effect.Type type)
    {
        var list = side == ChessManager.Side ? P1List : P2List;
        return list.Find(e => e.type == type).action;
    }

    public bool HasEffect(bool side, Effect.Type type)
    {
        var list = side == ChessManager.Side ? P1List : P2List;
        return list.Any(e => e.type == type);
    }


    public void DeliverHitEffect(MoveType t, bool side)
    {
        (int, bool) effectInfo = t switch { MoveType.Heal => (0, side), MoveType.Defense => (1, side), MoveType.Weaken => (2, !side), MoveType.Evasion => (3, side), MoveType.Slow => (4, !side), MoveType.Poison => (5, !side), _ => (-1, false) };
        var newParticleSystem = Instantiate(P_HitEffect);

        newParticleSystem.gameObject.transform.position = Ref.BattleManager.GetPos(ChessManager.Side == effectInfo.Item2).position;
        newParticleSystem.GetComponent<ParticleSystemRenderer>().material = hitEffects[effectInfo.Item1];
        newParticleSystem.Play();

        Helper.ActionAfterTime(hitEffectDuration, () => { Destroy(newParticleSystem.gameObject); });       
    }

    public void DeliverEffect(Effect.Type type, bool side, int rounds, float action)
    {
        var effectList = side == ChessManager.Side ? Player1 : Player2;
        var list = side == ChessManager.Side ? P1List : P2List;

        Effect e = new(type, rounds, action, !side);
        var elem = list.Find(ef => ef.type == e.type);

        if (elem != null)
        {
            list.Remove(elem);
            effectList.RemoveEffect(elem);
            effectList.AddEffect(e);
            list.Add(e);
        }
        else
        {
            effectList.AddEffect(e);
            list.Add(e);
        }
        effectList.UpdateEffects();
    }

    public void DeliverEffect(MoveType t, bool attackingSide, int rounds, float action)
    {
        print("Delivering effect " + t + " from side " + attackingSide);
        (Effect.Type, bool) et = t switch { 
            MoveType.Defense => (Effect.Type.Defense, attackingSide), 
            MoveType.Weaken => (Effect.Type.Weaken, !attackingSide), 
            MoveType.Slow => (Effect.Type.Slow, !attackingSide), 
            MoveType.Poison => (Effect.Type.Poison, !attackingSide), 
            MoveType.Evasion => (Effect.Type.Evasion, attackingSide) 
        };

        var effectList = et.Item2 == ChessManager.Side ? Player1 : Player2;
        var list = et.Item2 == ChessManager.Side ? P1List : P2List;

        Effect e = new(et.Item1, rounds, action, !attackingSide);

        var elem = list.Find(ef => ef.type == e.type);
        if(elem != null)
        {
            list.Remove(elem);
            effectList.RemoveEffect(elem);
            effectList.AddEffect(e);
            list.Add(e);           
        }
        else
        {
            effectList.AddEffect(e);
            list.Add(e);
        }

        effectList.UpdateEffects();
    }

    public void TextAnimation(TMP_Text T_Text, bool side, string info, Color c, Vector3 add)
    {
        var text = Instantiate(T_Text, T_Text.gameObject.transform.parent);
        text.transform.localPosition = Ref.BattleManager.GetPos(ChessManager.Side == side).localPosition + add + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
        Vector3 direction = text.transform.position - BattleCam.transform.position;
        text.transform.rotation = Quaternion.LookRotation(direction);
        text.text = info;
        text.color = c;
        text.gameObject.SetActive(true);
        Tween.LocalPosition(text.transform, text.transform.localPosition + new Vector3(0, 0.2f, 0), 1f, 0, Tween.EaseOut, completeCallback: () => { Destroy(text.gameObject); });
    }

    public int GetEffectCount(bool side, bool? kind, Effect.Type? type)
    {
        var list = side ? P1List : P2List;

        IEnumerable<Effect> query = list;

        if (kind.HasValue)
            query = query.Where(e => e.IsPositive() == kind.Value);

        if (type.HasValue)
            query = query.Where(e => e.type == type.Value);

        return query.Count();
    }

    public void ClearBadForSide(bool side)
    {
        print("Clearing bad effects for side " + side);
        if (side)
        {
            var badEffects = P1List.Where(e => !e.IsPositive()).ToList();
            Player1.RemoveEffects(badEffects.Select(e => e.type).ToList());
            P1List = P1List.Where(e => e.IsPositive()).ToList();
        }
        else
        {
            var badEffects = P2List.Where(e => !e.IsPositive()).ToList();
            Player2.RemoveEffects(badEffects.Select(e => e.type).ToList());
            P2List = P2List.Where(e => e.IsPositive()).ToList();
        }
    }
    public void ClearAll()
    {
        print("Clearing all effects");
        P1List.Clear();
        P2List.Clear();
        Player1.ClearEffects();
        Player2.ClearEffects();
    }
}
