using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PotionSlotUI : MonoBehaviour
{
    public PotionData PotionData;
    public Image I_Icon;

    public void Create(PotionData potionData)
    {
        PotionData = potionData;

        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(() => {
            GetComponent<Button>().onClick.RemoveAllListeners();
            Ref.BattleManager.PrepareUsePotion(PotionData, ChessManager.Side);
            I_Icon.gameObject.SetActive(false);
        });

        I_Icon = transform.GetChild(0).GetComponent<Image>();
        I_Icon.gameObject.SetActive(true);
        I_Icon.sprite = Resources.Load<Sprite>($"Icons/Potions/{potionData.Name}");
        I_Icon.Fit(65);
    }
}
