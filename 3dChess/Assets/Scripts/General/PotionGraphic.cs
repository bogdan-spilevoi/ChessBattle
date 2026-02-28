using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PotionGraphic : DraggableGraphic<PotionData>
{
    public override void Create(PotionData potionData)
    {
        thisData = potionData;
        GetIcon();
    }

    public void SetPosition(int pos)
    {
        thisData.Position = pos;
    }

    public override void GetIcon()
    {
        if (Icon == null)
            Icon = GetComponent<Image>();

        Sprite mySprite = Resources.Load<Sprite>($"Icons/Potions/{thisData.Name}");
        Icon.sprite = mySprite != null ? mySprite : Resources.Load<Sprite>($"Icons/Potions/{thisData.Name}");
        Icon.Fit(fitTo);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        int pos = GameRef.PotionsEdit.GetSlotFromPosition(rectTransform.localPosition);
        if (pos == -1) 
        {
            thisData.Position = -1;
            GameRef.PotionsEdit.UpdateAllPotionGraphicsCounts();
            Destroy(gameObject); 
            return; 
        }

        if(GameRef.PotionsEdit.TryGetPotionUnderSlot(pos, out var otherPotionGraphic))
        {
            if(GameRef.PotionsEdit.TryGetTileByPotion(this, out var ind))
            {
                GameRef.PotionsEdit.SwapPotions(pos, ind);
                return;
            }
            else
            {
                GameRef.PotionsEdit.RemovePotionFromTile(pos);
            }
        }

        GameRef.PotionsEdit.PlacePotionAtTile(this, pos);
    }
}
