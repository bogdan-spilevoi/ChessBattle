using Pixelplacement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PotionsEdit : MonoBehaviour
{
    public GameObject InventoryInfo;
    public List<PotionSlot> InventorySlots = new();
    public List<ListPotionUI> potionUIs = new();
    public PotionGraphic OrgPotionGraphic;

    public ListPotionUI OriginalPotionUI;

    private void Start()
    {
        InventorySlots.Clear();
        for (int i = 0; i < InventoryInfo.transform.childCount; i++)
        {
            InventorySlots.Add(new(InventoryInfo.transform.GetChild(i).GetComponent<Image>(), null));
        }
        Invoke(nameof(PlacePotions), 0.5f);
    }

    public void RefreshPotions()
    {
        foreach(var prevPotionUI in potionUIs)
        {
            Destroy(prevPotionUI.gameObject);
        }
        potionUIs.Clear();

        foreach(var p in Variants.PotionVariants)
        {
            var playerPotionData = GameRef.PlayerBehaviour.GetPotionsByName(p.Name);
            CreatePotionUI(p.Name, playerPotionData);
        }
    }

    public void UpdateAllPotionGraphicsCounts()
    {
        foreach(var potionUI in potionUIs)
        {
            potionUI.UpdateCount();
        }
    }

    private void CreatePotionUI(string potionName, List<PotionData> potions)
    {
        var newP = Instantiate(OriginalPotionUI, OriginalPotionUI.transform.parent);
        newP.Create(potionName, potions);
        newP.gameObject.SetActive(true);
        potionUIs.Add(newP);
    }

    public PotionGraphic CreatePotionGraphic(Vector3 pos, PotionData data)
    {
        var newPotionGraphic = Instantiate(OrgPotionGraphic, InventoryInfo.transform);
        newPotionGraphic.gameObject.SetActive(true);
        newPotionGraphic.Create(data);
        newPotionGraphic.GetComponent<RectTransform>().position = pos;
        return newPotionGraphic;
    }

    public void PlacePotionAtTile(PotionGraphic pg, int pos)
    {
        pg.transform.SetParent(InventoryInfo.transform);
        pg.transform.SetAsLastSibling();
        pg.transform.localPosition = InventorySlots[pos].SlotImage.transform.localPosition;
        InventorySlots[pos] = new(InventorySlots[pos].SlotImage, pg);


        pg.SetPosition(pos);
        UpdateAllPotionGraphicsCounts();
    }

    public void RemovePotionFromTile(int pos)
    {
        if (InventorySlots[pos].PotionGraphic == null)
            return;

        Destroy(InventorySlots[pos].PotionGraphic.gameObject);
        InventorySlots[pos] = new(InventorySlots[pos].SlotImage, null);
    }

    public void SwapPotions(int pos1, int pos2)
    {
        if (!TryGetPotionUnderSlot(pos1, out var p1) || !TryGetPotionUnderSlot(pos2, out var p2))
            return;

        Tween.LocalPosition(p1.transform, InventorySlots[pos2].SlotImage.transform.localPosition, 0.1f, 0, Tween.EaseInOut);
        Tween.LocalPosition(p2.transform, InventorySlots[pos1].SlotImage.transform.localPosition, 0.1f, 0, Tween.EaseInOut);

        InventorySlots[pos1] = new(InventorySlots[pos1].SlotImage, p2);
        InventorySlots[pos2] = new(InventorySlots[pos2].SlotImage, p1);
    }

    public bool TryGetPotionUnderSlot(int pos, out PotionGraphic potionGraphic)
    {
        if (InventorySlots[pos].PotionGraphic == null)
        {
            potionGraphic = null;
            return false;
        }

        potionGraphic = InventorySlots[pos].PotionGraphic;
        return true;
    }

    public bool TryGetTileByPotion(PotionGraphic pg, out int pos)
    {
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            if (InventorySlots[i].PotionGraphic == pg)
            {
                pos = i;
                return true;
            }
        }
        pos = -1;
        return false;
    }

    public int GetSlotFromPosition(Vector3 pos)
    {
        for (int i = 0; i < InventorySlots.Count; i++)
        {
            if (IsInsideSquare(
                pos, 
                InventorySlots[i].SlotImage.GetComponent<RectTransform>().localPosition, 
                InventorySlots[i].SlotImage.GetComponent<RectTransform>().sizeDelta.x))
                return i;
        }
        return -1;
    }
    bool IsInsideSquare(Vector2 point, Vector2 center, float sideLength)
    {
        float halfSide = sideLength / 2f;

        return
            point.x >= center.x - halfSide &&
            point.x <= center.x + halfSide &&
            point.y >= center.y - halfSide &&
            point.y <= center.y + halfSide;
    }

    public void PlacePotions()
    {
        for(int i = 0; i < InventorySlots.Count; i++)
        {
            if (GameRef.PlayerBehaviour.TryGetPotionOnPosition(i, out var potion))
            {
                var pg = CreatePotionGraphic(Vector3.zero, potion);
                PlacePotionAtTile(pg, i);
            }
        }
    }
}

[Serializable]
public struct PotionSlot
{
    public Image SlotImage;
    public PotionGraphic PotionGraphic;
    public PotionSlot(Image slotImage, PotionGraphic potionGraphic)
    {
        SlotImage = slotImage;
        PotionGraphic = potionGraphic;
    }
}
