using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineManager : MonoBehaviour
{
    private void Awake()
    {
        bool flag = PlayerPrefsExtentions.GetBool("online");
        ChessManager.Local = !flag;
    }
}
