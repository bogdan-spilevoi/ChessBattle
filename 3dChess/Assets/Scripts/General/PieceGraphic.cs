using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class PieceGraphic : MonoBehaviour, IDragHandler,IEndDragHandler
{
    private Image Icon;
    public TMP_Text T_Name;
    public bool isHeld;
    [HideInInspector]
    public RectTransform rectTransform;
    public EntityData thisEntity;
    public int position = -1;
    private float fitTo;
    public bool isDragging = false;

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

    public void StartDrag()
    {
        GameRef.LayoutEdit.RemovePieceGraphicHelper.SetActive(true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!isDragging)
        {
            isDragging = true;
            StartDrag();
        }
        
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        GameRef.LayoutEdit.RemovePieceGraphicHelper.SetActive(false);
        int pos = GameRef.LayoutEdit.GetTileIndexFromPosition(rectTransform.localPosition);
        if (pos == -2 ||(pos == -1 && position == -1))
        {
            thisEntity.Position = -1;
            GameRef.LayoutEdit.RemovePieceGraphic(this);
            GameRef.LayoutEdit.UpdateLimit();
            GameRef.LayoutEdit.UpdateAllListPieces();
            return;
        }
        var possibleOtherPieceInSquare = GameRef.LayoutEdit.player.PiecesInventory.Find(e => e.Position == pos && e != thisEntity && e.Position != -1);

        if (possibleOtherPieceInSquare != null)
        {
            var otherPieceGraphic = GameRef.LayoutEdit.PieceGraphics.Find(p => p.thisEntity == possibleOtherPieceInSquare);
            possibleOtherPieceInSquare.Position = position;
            otherPieceGraphic.position = position;
            if (position == -1)
            {
                GameRef.LayoutEdit.PieceGraphics.Remove(otherPieceGraphic);
                Destroy(otherPieceGraphic.gameObject);               
            }
            else
                Tween.AnchoredPosition(otherPieceGraphic.rectTransform, GameRef.LayoutEdit.Squares[position].GetComponent<RectTransform>().localPosition, 0.1f, 0, Tween.EaseInOut);
        }
        if (pos != -1)
        {
            thisEntity.Position = pos;
            position = pos;
        }
        

        Tween.AnchoredPosition(rectTransform, GameRef.LayoutEdit.Squares[position].GetComponent<RectTransform>().localPosition, 0.1f, 0, Tween.EaseInOut);
        GameRef.LayoutEdit.UpdateAllListPieces();
        GameRef.LayoutEdit.UpdateLimit();
    }

    public void GetIcon()
    {
        if (Icon == null)
            Icon = GetComponent<Image>();

        Sprite mySprite = Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/{thisEntity.Variant}");
        Icon.sprite = mySprite != null ? mySprite : Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/basic");
        Icon.Fit(fitTo);

        
    }
}
