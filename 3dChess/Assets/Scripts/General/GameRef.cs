using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRef : MonoBehaviour
{
    public static GameRef Instance;

    public TrainerSpeak trainerSpeak;
    public static TrainerSpeak TrainerSpeak { get { return Instance.trainerSpeak; } }

    public PlayerBehaviour playerBehaviour;
    public static PlayerBehaviour PlayerBehaviour {  get { return Instance.playerBehaviour; } }

    public LayoutEdit layoutEdit;
    public static LayoutEdit LayoutEdit {  get { return Instance.layoutEdit; } }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
}
