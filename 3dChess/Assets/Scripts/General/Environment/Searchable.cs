using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Searchable : MonoBehaviour
{
    public string Name;
    public bool isSearched = false;
    public float distanceToPlayer;
    public float Range = 2f;
    [HideInInspector]
    public float sqrRange;

    private void Awake()
    {
        sqrRange = Range * Range;
    }


    public void Search()
    {
        isSearched = true;
        GetComponent<Outline>().enabled = false;

        GameRef.PlayerBehaviour.BoxBehaviour.PrepareBox();
    }
}
