using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListPotionUI : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public string PotionName;
    public TMP_Text T_Name, T_Count;
    public Image I_Sprite;
    public PotionGraphic PotionGraphic;
    public List<PotionData> thesePotions = new();
    private List<PotionData> AvailablePotions  { get { return thesePotions.Where(p => p.Position == -1).ToList(); } }

    public void Create(string potionName, List<PotionData> potions)
    {
        PotionName = potionName;
        thesePotions.Clear();
        thesePotions = new(potions);

        T_Name.text = potionName;
        T_Count.text = "x" + AvailablePotions.Count;

        I_Sprite.sprite = Resources.Load<Sprite>($"Icons/Potions/{potionName}");
        I_Sprite.Fit(75);
    }

    public void UpdateCount()
    {
        T_Count.text = "x" + AvailablePotions.Count;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (AvailablePotions.Count <= 0) return;
        if (PotionGraphic == null)
        {
            print("AA");
            PotionGraphic = GameRef.PotionsEdit.CreatePotionGraphic(eventData.position, AvailablePotions.First());
        }
        else
        {
            PotionGraphic.rectTransform.anchoredPosition += eventData.delta;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (PotionGraphic != null)
        {
            PotionGraphic.OnEndDrag(eventData);
            PotionGraphic = null;
        }
    }
}
