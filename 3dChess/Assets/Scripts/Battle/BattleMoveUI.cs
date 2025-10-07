using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleMoveUI : MonoBehaviour
{
    public TMP_Text T_Name, T_Action, T_Count;
    public Image I_Icon, I_Rarity;
    public GameObject Overlay;

    public void Create(Move m, bool isLocked = false)
    {
        if (m.Count == 0)
            isLocked = true;

        T_Name.text = m.Name;
        T_Action.text = m.Action.ToString();
        I_Rarity.sprite = Resources.Load<Sprite>($"Rarities/{m.Rarity.ToString().ToLower()}");
        I_Icon.sprite = Resources.Load<Sprite>($"MoveIcons/{m.Type.ToString().ToLower()}");
        Helper.FitImageToSize(I_Rarity, 65);
        Helper.FitImageToSize(I_Icon, 50);
        Overlay.SetActive(isLocked);
        //GetComponent<Button>().enabled = !isLocked;
        T_Count.text = $"{m.Count}/{m.MaxCount}";
    }
}
