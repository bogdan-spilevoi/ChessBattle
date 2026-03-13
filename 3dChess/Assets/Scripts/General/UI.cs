using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject B_BattleTrainer;
    public GameObject B_EnterHouse, B_ExitHouse;
    public GameObject B_SeachItem;

    public CanvasGroup Tab_Hospital;

    public void ToggleEnterHouse(bool toggle)
    {
        B_EnterHouse.SetActive(toggle);
    }

    public void ToggleExitHouse(bool toggle)
    {
        B_ExitHouse.SetActive(toggle);
    }

    public void ShowBattleTrainerButton(string trainerName)
    {
        B_BattleTrainer.transform.Find("name").GetComponent<TMP_Text>().text = trainerName;
        B_BattleTrainer.SetActive(true);
    }

    public void HideBattleTrainerBUtton() => B_BattleTrainer.SetActive(false);

    public void ActivateTab(CanvasGroup canvasGroup)
    {
        canvasGroup.Activate();
    }

    public void DeActivateTab(CanvasGroup canvasGroup)
    {
        canvasGroup.Deactivate();
    }

    public void ToggleSearchItem(bool toggle, string name = "")
    {
        B_SeachItem.SetActive(toggle);
        if(toggle)
        {
            B_SeachItem.transform.GetComponentInChildren<TMP_Text>().text = "Press <b>F</b> to search " + name;
        }
    }
}
