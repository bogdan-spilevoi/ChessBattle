using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchableManager : MonoBehaviour
{
    public List<Searchable> Searchables = new();
    public HashSet<Searchable> InRangeSearchables = new();
    public Searchable ClosestSearchable;

    private void Awake()
    {
        Searchables = new(FindObjectsOfType<Searchable>());
    }

    private void Update()
    {
        Vector3 playerPos = GameRef.PlayerBehaviour.transform.position;

        ClosestSearchable = null;
        float closestSqrDist = float.MaxValue;

        foreach (var searchable in Searchables)
        {
            if (searchable.isSearched)
                continue;

            float sqrDist = (searchable.transform.position - playerPos).sqrMagnitude;
            if (sqrDist <= searchable.sqrRange)
            {
                InRangeSearchables.Add(searchable);

                if (sqrDist < closestSqrDist)
                {
                    closestSqrDist = sqrDist;
                    ClosestSearchable = searchable;
                }
            }
            else
            {
                InRangeSearchables.Remove(searchable);
            }
        }

        if (ClosestSearchable != null)
        {
            GameRef.UI.ToggleSearchItem(true, ClosestSearchable.Name);
            if (Input.GetKeyDown(KeyCode.F))
            {
                Search(ClosestSearchable);
            }
        }
        else
        {
            GameRef.UI.ToggleSearchItem(false);
        }
    }

    public void Search(Searchable searchable)
    {
        InRangeSearchables.Remove(searchable);
        searchable.Search();
    }
}
