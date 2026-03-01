using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class debug : MonoBehaviour
{
    //#if !UNITY_EDITOR
    public static string myLog = "";
    public string output;
    public string stack;
    public TMP_Text T_Log;
    public bool isOpen = false;
    public GameObject MyLog;

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    public void Log(string logString, string stackTrace, LogType type)
    {
        output = logString;
        stack = stackTrace;

        if(type == LogType.Warning)
            myLog = myLog + "\n\n" + "<color=yellow>" + output + "</color>";
        else if(type == LogType.Error)
            myLog = myLog + "\n\n" + "<color=red>" + output + "</color>";
        else
            myLog = myLog + "\n\n" + output;


        if (myLog.Length > 5000)
        {
            myLog = myLog.Substring(0, 4000);
        }
        T_Log.text = myLog;
    }

    //#endif
    public void OpenCloseMyLog()
    {
        if (isOpen)
        {
            MyLog.SetActive(false);

        }
        else
        {
            MyLog.SetActive(true);
        }

        isOpen = !isOpen;
    }

    public void Activate(GameObject g) => g.SetActive(true);
}
