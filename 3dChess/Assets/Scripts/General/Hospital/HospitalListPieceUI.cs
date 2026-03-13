using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HospitalListPieceUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Image Icon;
    public TMP_Text T_Name, T_Type, T_Level;
    public Slider S_Health;
    public EntityData thisEntity;
    public PieceGraphicHospital PieceGraphic;
    public GameObject Overlay;
    private float fitTo = 60;
    public TMP_Text T_OverlayReason;




    public void Create(EntityData data)
    {
        thisEntity = data;
        T_Name.text = thisEntity.Name;
        T_Type.text = $"<b><i>{thisEntity.PieceType}</i></b>";
        T_Type.color = GameColors.GetColorByType(data.PieceType);
        T_Level.text = "lvl " + thisEntity.Level;
        S_Health.maxValue = thisEntity.MaxHealth;
        S_Health.value = thisEntity.Health;

        if (thisEntity.Position != -1 || thisEntity.Health == thisEntity.MaxHealth)
            Overlay.SetActive(true);

        if (thisEntity.Position > -1)
            T_OverlayReason.text = "In Layout";
        if (thisEntity.Position < -1)
            T_OverlayReason.text = "Healing";
        if (thisEntity.Health == thisEntity.MaxHealth)
            T_OverlayReason.text = "Healthy";

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => {
            FindObjectOfType<ViewPiece>().OpenViewPiece(thisEntity);
        });

        if (S_Health.value == 0)
        {
            S_Health.fillRect.GetComponent<Image>().color = Color.red;
        }

        GetIcon();
    }



    public void OnDrag(PointerEventData eventData)
    {
        if (thisEntity.Position != -1 || thisEntity.Health == thisEntity.MaxHealth) return;
        if (PieceGraphic == null)
        {
            PieceGraphic = GameRef.HospitalEdit.CreatePieceGraphic(eventData.position, thisEntity);
        }
        else
        {
            PieceGraphic.rectTransform.anchoredPosition += eventData.delta / GameRef.MainCanvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PieceGraphic != null)
        {
            PieceGraphic.OnEndDrag(eventData);
            PieceGraphic = null;
        }
    }
    public void GetIcon()
    {
        Sprite mySprite = Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/{thisEntity.Variant}");

        Icon.sprite = mySprite != null ? mySprite : Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/basic");
        Icon.Fit(fitTo);
    }

    public void UpdateHealth()
    {
        S_Health.value = thisEntity.Health;
    }
}
