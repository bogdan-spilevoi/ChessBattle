using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsExtentions
{
    public static bool GetBool(string key)
    {
        if(!PlayerPrefs.HasKey(key))      
        { 
            return false; 
        }
        return PlayerPrefs.GetInt(key) == 1;
    }

    public static void SetBool(string key, bool value)
    {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }
}
