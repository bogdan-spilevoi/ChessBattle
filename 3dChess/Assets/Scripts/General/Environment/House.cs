using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    public GameObject Door;
    public GameObject TeleportTo;

    public Vector3 FixedCameraPos = new();
    [HideInInspector]
    public Vector3 PlayerBeforeEnter = new();
    public bool IsInside = false;

    public static float Treshold = 2f;
    public static bool InsideAnyHouse = false;

    public void Update()
    {
        var dist = Vector3.Distance(GameRef.PlayerBehaviour.transform.position, Door.transform.position);

        if (dist < Treshold && GameRef.PlayerBehaviour.HouseInRange == null)
        {
            GameRef.PlayerBehaviour.HouseInRange = this;
        }

        if(dist > Treshold && GameRef.PlayerBehaviour.HouseInRange == this)
        {
            GameRef.PlayerBehaviour.HouseInRange = null;
        }

        if(IsInside)
        {
            var closeToExit = Vector3.Distance(GameRef.PlayerBehaviour.transform.position, TeleportTo.transform.position) < Treshold;

            GameRef.UI.ToggleExitHouse(closeToExit);

            if (closeToExit && Input.GetKey(KeyCode.E))
            {
                IsInside = false;
                InsideAnyHouse = false;
                GameRef.PlayerBehaviour.TeleportTo(PlayerBeforeEnter);
                GameRef.UI.ToggleExitHouse(false);
            }
            
        }
    }

    public void EnterHouse(bool teleport = true)
    {
        if(TeleportTo == null)
        {
            Debug.LogError("No teleport location set for house " + name);
            return;
        }
        if(teleport)
        {
            PlayerBeforeEnter = GameRef.PlayerBehaviour.transform.position;
            GameRef.PlayerBehaviour.TeleportTo(TeleportTo.transform.position);
        }
        else
        {
            PlayerBeforeEnter = Door.transform.position;
        }

        InsideAnyHouse = true;
        GameRef.PlayerBehaviour.Camera.transform.position = FixedCameraPos;

        this.ActionAfterTime(0.5f, () =>
        {
            IsInside = true;
        });
        
    }
}
