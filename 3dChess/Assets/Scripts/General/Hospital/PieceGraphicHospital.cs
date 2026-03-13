using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PieceGraphicHospital : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public Image Icon;
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
        T_Name.gameObject.SetActive(true);
        transform.SetAsLastSibling();

        if(thisEntity.Position < -1)
        {
            GameRef.HospitalEdit.RemoveFromSlot(thisEntity.Position);
            thisEntity.Position = -1;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            isDragging = true;
            StartDrag();
        }

        rectTransform.anchoredPosition += eventData.delta / GameRef.MainCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        if (GameRef.HospitalEdit.TryPlaceOnSlot(this, out var slot))
        {
            slot.Create(this);
            thisEntity.Position = slot.Index;
            GameRef.HospitalEdit.RefreshListPiecesUI();
            T_Name.gameObject.SetActive(false);
        }
        else
        {
            thisEntity.Position = -1;
            GameRef.HospitalEdit.PieceGraphics.Remove(this);
            Destroy(gameObject);
            GameRef.HospitalEdit.RefreshListPiecesUI();
        }

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
