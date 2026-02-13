using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EffectUI : MonoBehaviour
{
    public Image Icon;
    public Effect thisEffect;
    public TMP_Text T_Left;
    public EffectListUI thisList;

    public void Create(Effect e)
    {
        Icon.sprite = Resources.Load<Sprite>($"MoveIcons/{e.type.ToString().ToLower()}");
        Icon.Fit(50);
        T_Left.text = e.rounds.ToString();
        thisEffect = e;
    }

    public void UpdateEffectUI()
    {
        T_Left.text = thisEffect.rounds.ToString();
    }

    private void OnDisable()
    {
        thisEffect.OnTurnPass -= UpdateEffectUI;
    }
}
