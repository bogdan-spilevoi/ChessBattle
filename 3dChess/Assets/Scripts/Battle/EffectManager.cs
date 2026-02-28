using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.AssetImporters;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [Header("Hit Effect")]
    public float hitEffectDuration;
    public List<Material> hitEffects = new();
    public ParticleSystem P_HitEffect;
    public Transform Pos1, Pos2;

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
        var list = sideThatDidMove ? P1List : P2List;
        var listUI = sideThatDidMove ? Player1 : Player2;

        list.ForEach(e => { e.rounds--; });
        if (sideThatDidMove)
            P1List = list.Where(e => e.rounds > 0).ToList();
        else
            P2List = list.Where(e => e.rounds > 0).ToList();
        listUI.UpdateEffects();
    }

    public float GetEffecttypeAction(bool side, Effect.Type type)
    {
        var list = side ? P1List : P2List;
        return list.Find(e => e.type == type).action;
    }

    public bool HasEffect(bool side, Effect.Type type)
    {
        var list = side ? P1List : P2List;
        return list.Any(e => e.type == type);
    }


    public void DeliverHitEffect(MoveType t, bool side)
    {
        (int, bool) effectInfo = t switch { MoveType.Heal => (0, side), MoveType.Defense => (1, side), MoveType.Weaken => (2, !side), MoveType.Evasion => (3, side), MoveType.Slow => (4, !side), MoveType.Poison => (5, !side), _ => (-1, false) };
        var newParticleSystem = Instantiate(P_HitEffect);
        newParticleSystem.gameObject.transform.position = effectInfo.Item2 ? Pos1.position : Pos2.position;
        newParticleSystem.GetComponent<ParticleSystemRenderer>().material = hitEffects[effectInfo.Item1];
        newParticleSystem.Play();
        Helper.ActionAfterTime(hitEffectDuration, () => { Destroy(newParticleSystem.gameObject); });       
    }

    public void DeliverEffect(MoveType t, bool side, int rounds, float action)
    {
        (Effect.Type, bool) et = t switch { MoveType.Defense => (Effect.Type.Defense, side), MoveType.Weaken => (Effect.Type.Weaken, !side), MoveType.Slow => (Effect.Type.Slow, !side), MoveType.Poison => (Effect.Type.Poison, !side), MoveType.Evasion => (Effect.Type.Evasion, side) };
        var effectList = et.Item2 ? Player1 : Player2;
        var list = et.Item2 ? P1List : P2List;

        Effect e = new(et.Item1, rounds, action);

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
        text.transform.localPosition = (side ? Pos1.transform.localPosition : Pos2.transform.localPosition) + add + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0);
        Vector3 direction = text.transform.position - BattleCam.transform.position;
        text.transform.rotation = Quaternion.LookRotation(direction);
        text.text = info;
        text.color = c;
        text.gameObject.SetActive(true);
        Tween.LocalPosition(text.transform, text.transform.localPosition + new Vector3(0, 0.2f, 0), 1f, 0, Tween.EaseOut, completeCallback: () => { Destroy(text.gameObject); });
    }
}
