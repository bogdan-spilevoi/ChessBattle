using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookPieceUI : MonoBehaviour
{
    public Image Icon;
    public TMP_Text T_Type;
    public Slider S_Attack, S_Defence, S_Speed, S_Luck;
    private EntityData thisEntity;
    public GameObject Overlay;
    public float fitTo;
    private PlayerBehaviour player;

    public void Create(EntityData data)
    {
        player = FindAnyObjectByType<PlayerBehaviour>();
        player.OnGetPiece += RefreshOnPlayerGetPiece;

        thisEntity = data;
        T_Type.text = $"<b><i>{thisEntity.PieceType}</i></b>";
        T_Type.color = GameColors.GetColorByType(data.PieceType);

        S_Attack.maxValue = 10;
        S_Defence.maxValue = 1000;
        S_Speed.maxValue = 10;
        S_Luck.maxValue = 10;

        S_Attack.value = data.Attack;
        S_Defence.value = data.MaxHealth;
        S_Speed.value = data.Speed;
        S_Luck.value = data.Luck;

        GetIcon();
        Overlay.SetActive(!player.pieceFoundData.PiecesFound.Contains(thisEntity.Variant + "/" + thisEntity.PieceType));
    }

    public void RefreshOnPlayerGetPiece(EntityData e)
    {
        if(e.Variant == thisEntity.Variant && e.PieceType == thisEntity.PieceType)
            Overlay.SetActive(false);
    }

    public void GetIcon()
    {
        Sprite mySprite = Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/{thisEntity.Variant}");

        Icon.sprite = mySprite != null ? mySprite : Resources.Load<Sprite>($"Icons/{thisEntity.PieceType}/basic");
        Helper.FitImageToSize(Icon, fitTo);
    }
}
