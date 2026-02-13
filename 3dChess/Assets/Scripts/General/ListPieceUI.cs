using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListPieceUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Image Icon;
    public TMP_Text T_Name, T_Type, T_Level;
    public Slider S_Health;
    private EntityData thisEntity;
    public LayoutEdit LayoutEdit;
    public PieceGraphic PieceGraphic;
    public GameObject Overlay;
    private float fitTo = 60;
    private UnityEngine.UI.Outline Outline;




    public void Create(EntityData data)
    {
        thisEntity = data;
        T_Name.text = thisEntity.Name;
        T_Type.text = $"<b><i>{thisEntity.PieceType}</i></b>";
        T_Type.color = GameColors.GetColorByType(data.PieceType);
        T_Level.text = "lvl " + thisEntity.Level;
        S_Health.maxValue = thisEntity.MaxHealth;
        S_Health.value = thisEntity.Health;
        if (S_Health.value == 0)
            Overlay.SetActive(true);

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => {
            FindObjectOfType<ViewPiece>().OpenViewPiece(thisEntity);
        });

        Outline = GetComponent<UnityEngine.UI.Outline>();
        if (S_Health.value == 0)
        {
            S_Health.fillRect.GetComponent<Image>().color = Color.red;
        }

        GetIcon();
        UpdateOutline();
    }

    public void UpdateOutline()
    {
        Outline.enabled = thisEntity.Position != -1 && thisEntity.Health > 0;
    }



    public void OnDrag(PointerEventData eventData)
    {
        if (thisEntity.Health <= 0) return;
        if (thisEntity.Position != -1) return;
        if (LayoutEdit.PiecesOnBoard >= LayoutEdit.Limit) return;
        if(PieceGraphic == null)
        {
            PieceGraphic = LayoutEdit.CreatePieceGraphic(eventData.position, thisEntity);
        }
        else
        {
            PieceGraphic.rectTransform.anchoredPosition += eventData.delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(PieceGraphic != null)
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
}
