using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public TMP_Text T_Info;
    public Button B_Cancel;
    public GameObject Tab_LoadingScreen;

    private void Awake()
    {
        B_Cancel.onClick.AddListener(Cancel);
    }

    public void SetInfo(string info)
    {
        T_Info.text = info;
    }

    public void Toggle(bool toggle)
    {
        Tab_LoadingScreen.SetActive(toggle);
    }

    public void HideCancel()
    {
        B_Cancel.gameObject.SetActive(false);
    }

    public void Cancel()
    {
        
    }
}
