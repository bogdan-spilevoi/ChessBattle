using Pixelplacement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    public string Name;
    public List<EntityData> Inventory;
    [TextArea(10, 10)]
    public string ChallengeText, DefeatedText;
    public bool Defeated;

    private Quaternion InitialRotation;

    private void Awake()
    {
        InitialRotation = transform.rotation;
    }

    public void Create(TrainerData data)
    {
        Inventory = data.Inventory;
        Name = data.Name;
        Defeated = data.Defeated;
    }

    public void Speak()
    {
        Vector3 direction = (GameRef.PlayerBehaviour.transform.position - transform.position);
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Tween.Rotation(transform, targetRotation, 0.5f, 0, Tween.EaseInOut);
    }

    public void EndSpeak()
    {
        Tween.Rotation(transform, InitialRotation, 0.5f, 0, Tween.EaseInOut);
    }
}
