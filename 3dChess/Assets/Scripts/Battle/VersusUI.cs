using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersusUI : MonoBehaviour
{
    public GameObject Tab_Versus;
    public RectTransform Versus;

    public Image I_Attacker, I_Defender;


    public void Create(Piece attacker, Piece defender, Action after)
    {
        Tab_Versus.SetActive(true);
        Versus.transform.localScale = Vector3.zero;
        Tween.LocalScale(Versus, Vector3.one, 0.5f, 0f, Tween.EaseInOut);

        I_Attacker.sprite = Resources.Load<Sprite>($"Icons/{attacker.PieceType}/{attacker.Variant}");
        I_Defender.sprite = Resources.Load<Sprite>($"Icons/{defender.PieceType}/{defender.Variant}");
        if (attacker.Variant == "basic" && !attacker.side)
            I_Attacker.color = Color.gray;
        if (defender.Variant == "basic" && !defender.side)
            I_Attacker.color = Color.gray;
        I_Attacker.Fit(350);
        I_Defender.Fit(350);

        var initialAttackerPos = I_Attacker.GetComponent<RectTransform>().localPosition;
        var initialDefenderPos = I_Defender.GetComponent<RectTransform>().localPosition;

        I_Attacker.GetComponent<RectTransform>().localPosition = new Vector3(initialAttackerPos.x - 800, initialAttackerPos.y, initialAttackerPos.z);
        I_Defender.GetComponent<RectTransform>().localPosition = new Vector3(initialDefenderPos.x + 800, initialDefenderPos.y, initialDefenderPos.z);

        Tween.LocalPosition(I_Attacker.GetComponent<RectTransform>(), initialAttackerPos, 0.5f, 0f, Tween.EaseInOut);
        Tween.LocalPosition(I_Defender.GetComponent<RectTransform>(), initialDefenderPos, 0.5f, 0f, Tween.EaseInOut);

        this.ActionAfterTime(3f, () =>
        {
            Tab_Versus.SetActive(false);
            after.Invoke();
        });
    }

}
