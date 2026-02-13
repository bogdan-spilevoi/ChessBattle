using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
    public static async void ActionAfterTime(float time, Action A)
    {
        await Task.Delay((int)(time * 1000));
        A?.Invoke();
    }

    public static IEnumerator ActionAfterTimeCor(float time, Action A)
    {
        yield return new WaitForSeconds(time);
        A?.Invoke();
    }

}
