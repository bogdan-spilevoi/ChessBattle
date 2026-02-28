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

    public void SendCommand(CommandBase command)
    {

    }

    public void ReceiveCommand()
    {

    }
}
