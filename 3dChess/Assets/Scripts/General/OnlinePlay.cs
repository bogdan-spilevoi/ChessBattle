using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlinePlay : MonoBehaviour
{
    public void PlayOnline()
    {
        PlayerPrefsExtentions.SetBool("online", true);
        SceneManager.LoadScene("Chess");
    }
}
