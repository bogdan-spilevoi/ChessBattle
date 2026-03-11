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

    public PotionsEdit potionsEdit;
    public static PotionsEdit PotionsEdit { get { return Instance.potionsEdit; } }

    public Canvas mainCanvas;
    public static Canvas MainCanvas { get { return Instance.mainCanvas; } }

    public UI ui;
    public static UI UI { get { return Instance.ui; } }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
}
