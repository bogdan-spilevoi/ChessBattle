using Newtonsoft.Json;
using Pixelplacement.TweenSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : MonoBehaviour
{
    public SaveManager SaveManager;
    public PieceFoundData pieceFoundData;
    public List<EntityData> PiecesInventory = new();
    [HideInInspector]
    public Trainer TrainerInRange;
    public UI UI;
    [HideInInspector]
    public LayoutEdit LayoutEdit;

    public Action<EntityData> OnGetPiece;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T) && !Movement.IsPaused)
        {
            SaveManager.SaveGame();
        }

        if (TrainerInRange != null)
        {
            UI.ShowBattleTrainerButton(TrainerInRange.Name);
            if(Input.GetKeyDown(KeyCode.Space) &&!Movement.IsPaused)
            {
                PlayerPrefs.SetString("trainer", JsonConvert.SerializeObject(new TrainerData(TrainerInRange), Formatting.Indented));
                SceneManager.LoadScene("Chess");
            }            
        }
        else
        {
            UI.HideBattleTrainerBUtton();
        }
    }

    public void ChangePiecePosition(int oldPos, int newPos)
    {
        var found = PiecesInventory.Find(e => e.Position == oldPos);
        found.Position = newPos;
    }

    private void OnTriggerEnter(Collider other)
    {

        if(other.TryGetComponent(out Trainer t) && !t.Defeated)
        {
            TrainerInRange = t;
        }
        if(other.CompareTag("test"))
        {
            GetPiece(Variants.GetRandom());
        }
        if (other.CompareTag("test2"))
        {
            GetPiece(Variants.GetRandomOfType(EntityData.Type.King));
        }
    }

    public void GetPiece(EntityData e)
    {
        string p = e.Variant + "/" + e.PieceType;
        if(!pieceFoundData.PiecesFound.Contains(p))
            pieceFoundData.PiecesFound.Add(p);

        PiecesInventory.Add(e);
        SaveManager.SaveGame();
        LayoutEdit.RefreshListPiecesUI();
        OnGetPiece?.Invoke(e);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Trainer t))
        {
            TrainerInRange = null;
        }
    }

    public void ChangePlayerPos(Vector3 pos)
    {
        GetComponent<CharacterController>().enabled = false;
        transform.position = pos;
        GetComponent<CharacterController>().enabled = true;
    }
}
