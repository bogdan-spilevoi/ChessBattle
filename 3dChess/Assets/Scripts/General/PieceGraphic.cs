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
    [HideInInspector]
    public LayoutEdit LayoutEdit;
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
        LayoutEdit.RemovePieceGraphicHelper.SetActive(true);
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
        LayoutEdit.RemovePieceGraphicHelper.SetActive(false);
        int pos = LayoutEdit.GetTileIndexFromPosition(rectTransform.localPosition);
        if (pos == -2 ||(pos == -1 && position == -1))
        {
            thisEntity.Position = -1;           
            LayoutEdit.RemovePieceGraphic(this);
            LayoutEdit.UpdateLimit();
            LayoutEdit.UpdateAllListPieces();
            return;
        }
        var possibleOtherPieceInSquare = LayoutEdit.player.PiecesInventory.Find(e => e.Position == pos && e != thisEntity && e.Position != -1);
        //print((possibleOtherPieceInSquare == null) + " " + position + " " + pos);
        if (possibleOtherPieceInSquare != null)
        {
            var otherPieceGraphic = LayoutEdit.PieceGraphics.Find(p => p.thisEntity == possibleOtherPieceInSquare);
            possibleOtherPieceInSquare.Position = position;
            otherPieceGraphic.position = position;
            if (position == -1)
            {
                LayoutEdit.PieceGraphics.Remove(otherPieceGraphic);
                Destroy(otherPieceGraphic.gameObject);               
            }
            else
                Tween.AnchoredPosition(otherPieceGraphic.rectTransform, LayoutEdit.Squares[position].GetComponent<RectTransform>().localPosition, 0.1f, 0, Tween.EaseInOut);
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
        try
        {
            Sprite mySprite = Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/{thisEntity.Variant}");
            print($"Icons/{thisEntity.PieceType}/{thisEntity.Variant}" + " " + (mySprite == null));
            Icon.sprite = mySprite != null ? mySprite : Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/basic");
            Icon.Fit(fitTo);
        }
        catch(NullReferenceException e)
        {
            Debug.LogException(e);
        }
        
    }
}
