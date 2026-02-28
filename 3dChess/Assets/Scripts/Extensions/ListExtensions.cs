using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{ 
    public static void ClearObjects<T>(this List<T> list) where T : MonoBehaviour
    {
        if (list == null) return;

        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            if (item != null)
            {
                Object.Destroy(item.gameObject);
            }
        }

        list.Clear();
    }
}
