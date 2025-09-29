using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PieceGraphic : MonoBehaviour, IDragHandler,IEndDragHandler
{
    private Image Icon;
    public TMP_Text T_Name;
    public bool isHeld;
    [HideInInspector]
    public RectTransform rectTransform;
    [HideInInspector]
    public EntityData thisEntity;
    public int position = -1;
    [HideInInspector]
    public LayoutEdit LayoutEdit;
    private float fitTo;

    private void Awake()
    {
        Icon = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        fitTo = Icon.rectTransform.sizeDelta.x;
    }

    public void Create(EntityData e, int position)
    {       
        thisEntity = e;
        T_Name.text = e.Name;
        this.position = position;
        GetIcon();
    }

    public void OnDrag(PointerEventData eventData)
    {
        LayoutEdit.RemovePieceGraphicHelper.SetActive(true);
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LayoutEdit.RemovePieceGraphicHelper.SetActive(false);
        int pos = LayoutEdit.GetTileIndexFronPosition(rectTransform.localPosition);
        if (pos == -2 ||(pos == -1 && position == -1))
        {
            thisEntity.Position = -1;           
            LayoutEdit.RemovePieceGraphic(this);
            LayoutEdit.UpdateLimit();
            LayoutEdit.UpdateAllListPieces();
            return;
        }
        if (pos != -1)
        {
            thisEntity.Position = pos;
            position = pos;
        }

        Tween.AnchoredPosition(rectTransform, LayoutEdit.Squares[position].GetComponent<RectTransform>().localPosition, 0.1f, 0, Tween.EaseInOut);
        LayoutEdit.UpdateAllListPieces();
        LayoutEdit.UpdateLimit();
    }

    public void GetIcon()
    {
        Sprite mySprite = Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/{thisEntity.Variant}");

        Icon.sprite = mySprite != null ? mySprite : Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/basic"); ;
        Helper.FitImageToSize(Icon, fitTo);
    }
}
