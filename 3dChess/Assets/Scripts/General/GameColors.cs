using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameColors : MonoBehaviour
{
    public static List<Color> PiecesText = new();
    public static List<Color> SideColors = new();

    public Color pawnText, bishopText, knightText, rookText, queenText, kingText;
    public Color WhiteHealth, BlackHeatlh;

    public static Color GetColorByType(EntityData.Type p)
    {
        return PiecesText[(int)p];
    }

    public static Color GetColorBySide(bool side)
    {
        return side ? SideColors[0] : SideColors[1];
    }

    public static GameColors Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        PiecesText = new() { 
            Instance.pawnText, 
            Instance.knightText, 
            Instance.bishopText, 
            Instance.rookText, 
            Instance.queenText, 
            Instance.kingText
        };

        SideColors = new() {
            WhiteHealth,
            BlackHeatlh,
        };
    }
}
