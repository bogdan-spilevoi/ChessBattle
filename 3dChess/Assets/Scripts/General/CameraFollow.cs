using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    public Vector3 Offset;

    public EnvBounds Bounds;
    public EnvBounds DefaultBounds = new()
    {
        Left = -60,
        Right = 60,
        Top = 60,
        Bottom = -60
    };
    public EnvBounds CameraBounds = new()
    {
        Top = 18,
        Bottom = 0,
        Left = 10,
        Right = 10,
    };
    private Camera cam;
    public float top, bottom;
    public static bool Enabled = true;

    void Start()
    {
        Enabled = true;
        Bounds = DefaultBounds;
        cam = GetComponent<Camera>();
    }


    void Update()
    {
        if (House.InsideAnyHouse || !Enabled)
        {
            return;
        }
        transform.position = GameRef.PlayerBehaviour.transform.position + Offset;

        /*float maxZ = Bounds.Top - CameraBounds.Top;
        float minZ = Bounds.Bottom + CameraBounds.Bottom;

        float maxX = Bounds.Right - CameraBounds.Right;
        float minX = Bounds.Left + CameraBounds.Left;

        transform.position = new(
            Mathf.Clamp(transform.position.x, minX, maxX),
            transform.position.y,
            Mathf.Clamp(transform.position.z, minZ, maxZ));*/
    }
}

[System.Serializable]
public struct EnvBounds
{
    public float Left, Right, Top, Bottom;
}
