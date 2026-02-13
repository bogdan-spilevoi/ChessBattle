using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoveUI : MonoBehaviour
{
    public TMP_Text T_Name, T_Description, T_Type, T_Action, T_LockInfo;
    private Move thisMove;
    public GameObject Overlay;
    public Image I_Rarity, I_Type;

    public void Create(Move m, bool lockState = false, int required = 0)
    {
        thisMove = m;
        T_Name.text = m.Name;
        T_Description.text = m.Description;
        T_Type.text = m.Type.ToString();
        T_Action.text = m.Action.ToString();
        I_Rarity.sprite = Resources.Load<Sprite>($"Rarities/{m.Rarity.ToString().ToLower()}");
        I_Rarity.Fit(65);

        if(lockState)
        {
            Overlay.SetActive(true);
            T_LockInfo.text = "Requires lvl " + required;
        }
        I_Type.sprite = Resources.Load<Sprite>($"MoveIcons/{m.Type.ToString().ToLower()}");
        I_Type.Fit(40);
    }

    
}
