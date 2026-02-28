using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PotionUI : MonoBehaviour
{
    public TMP_Text T_Name, T_Count;
    public Image I_Sprite;

    public void Create(string potionName, int count)
    {
        T_Name.text = potionName;
        T_Count.text = "x" + count;

        I_Sprite.sprite = Resources.Load<Sprite>($"Icons/Potions/{potionName}");
        I_Sprite.Fit(75);
    }

}
