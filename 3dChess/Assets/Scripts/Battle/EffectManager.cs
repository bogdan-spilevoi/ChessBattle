using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
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

    public void DeliverHitEffect(MoveType t, bool side)
    {
        (int, bool) effectInfo = t switch { MoveType.Heal => (0, side), MoveType.Defense => (1, side), MoveType.Weaken => (2, !side), MoveType.Evasion => (3, side), MoveType.Slow => (4, !side), _ => (-1, false) };

        var newParticleSystem = Instantiate(P_HitEffect);
        newParticleSystem.gameObject.transform.position = side ? Pos1.position : Pos2.position;
        newParticleSystem.GetComponent<ParticleSystemRenderer>().material = hitEffects[effectInfo.Item1];
        newParticleSystem.Play();
        Helper.ActionAfterTime(hitEffectDuration, () => { Destroy(newParticleSystem.gameObject); });
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
