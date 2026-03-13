using Pixelplacement;
using Pixelplacement.TweenSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HospitalSlotUI : MonoBehaviour
{
    private static WaitForSecondsRealtime _waitForSeconds1 = new(1);
    public TMP_Text T_Name, T_Time;
    public GameObject HSlot;
    public Slider S_Health;
    public PieceGraphicHospital PieceGraphic;
    public int Index;
    public GameObject HealthAnimation;
    public TweenBase HealthTween;

    public bool Occupied => PieceGraphic != null;

    public void Clear()
    {
        HealthAnimation.SetActive(false);
        HealthTween?.Stop();

        if (healCoroutine != null)
            StopCoroutine(healCoroutine);

        PieceGraphic = null;
        T_Name.text = "Empty";
        T_Time.text = "- : -";
        S_Health.gameObject.SetActive(false);
    }

    public void Create(int index)
    {
        HealthAnimation.SetActive(false);
        HealthTween?.Stop();

        T_Name.text = "Empty";
        T_Time.text = "- : -";
        S_Health.gameObject.SetActive(false);
        Index = index;
    }

    public void Create(PieceGraphicHospital pieceGraphic)
    {
        HealthAnimation.SetActive(false);
        HealthTween?.Stop();

        PieceGraphic = pieceGraphic;
        pieceGraphic.T_Name.gameObject.SetActive(false);

        T_Name.text = pieceGraphic.thisEntity.Name;

        S_Health.gameObject.SetActive(true);
        S_Health.maxValue = pieceGraphic.thisEntity.MaxHealth;
        S_Health.value = pieceGraphic.thisEntity.Health;

        Tween.Position(pieceGraphic.GetComponent<RectTransform>(), HSlot.GetComponent<RectTransform>().position, 0.5f, 0, Tween.EaseInOut);

        if (healCoroutine != null)
            StopCoroutine(healCoroutine);
        healCoroutine = StartCoroutine(HealCor());
    }

    private void Update()
    {
        if(PieceGraphic == null) return;

        T_Time.text = TimeSpan.FromSeconds(PieceGraphic.thisEntity.GetHealthMissing()).ToString(@"m\:ss");
    }

    private Coroutine healCoroutine;
    private IEnumerator HealCor()
    {
        HealthAnimation.SetActive(true);
        HealthAnimation.GetComponent<RectTransform>().localPosition = new Vector3(-125, 0, 0);
        HealthTween = Tween.LocalPosition(HealthAnimation.GetComponent<RectTransform>(), new Vector3(125, 0, 0), 1, 0, loop: Tween.LoopType.Loop);

        while (PieceGraphic.thisEntity.Health < PieceGraphic.thisEntity.MaxHealth)
        {
            yield return _waitForSeconds1;
            PieceGraphic.thisEntity.Health++;
            S_Health.value = PieceGraphic.thisEntity.Health;  
            GameRef.HospitalEdit.GetPieceUIByPieceGraphic(PieceGraphic).UpdateHealth();
        }

        HealthAnimation.SetActive(false);
        HealthTween?.Stop();

        PieceGraphic.thisEntity.Position = -1;
        GameRef.HospitalEdit.RemovePieceGraphic(PieceGraphic);
        GameRef.HospitalEdit.RefreshListPiecesUI();
        Clear();
    }
}
