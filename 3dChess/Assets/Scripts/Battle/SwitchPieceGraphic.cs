using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SwitchPieceGraphic : MonoBehaviour
{
    public TMP_Text T_Name, T_Level;
    public Image I_Graphic;
    public Slider S_Health;

    public void Create(string name, int level, int health, int maxHealth, Sprite img = null)
    {
        T_Name.text = name;
        T_Level.text = "lvl " + level;
        S_Health.maxValue = maxHealth;
        S_Health.value = health;
        I_Graphic.sprite = img;
        Helper.FitImageToSize(I_Graphic, 200);
    }
}
