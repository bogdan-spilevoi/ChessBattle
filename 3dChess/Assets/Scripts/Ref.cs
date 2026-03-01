using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ref : MonoBehaviour
{
    public static Ref Instance;

    public ManageTiles manageTiles;
    public static ManageTiles ManageTiles { get {  return Instance.manageTiles; } }


    public Camera cam;
    public static Camera Camera { get { return Instance.cam; } }

    public Camera battleCamera;
    public static Camera BattleCamera {  get { return Instance.battleCamera; } }


    public BattleManager battleManager;
    public static BattleManager BattleManager {  get { return Instance.battleManager; } }

    public PieceUI orgPieceUI;
    public static PieceUI OrgPieceUI {  get { return Instance.orgPieceUI; } }

    public AI aI;
    public static AI AI { get { return Instance.aI; } }

    public BattleUI battleUI;
    public static BattleUI BattleUI { get { return Instance.battleUI; } }

    public ChessManager chessManager;
    public static ChessManager ChessManager { get { return Instance.chessManager; } }

    public CommandManager commandManager;
    public static CommandManager CommandManager { get { return Instance.commandManager; } }

    public OnlineManager onlineManager;
    public static OnlineManager OnlineManager { get { return Instance.onlineManager; } }

    public VersusUI versusUI;
    public static VersusUI VersusUI { get { return Instance.versusUI; } }

    public RtdbRestListener rtdbRestListener;
    public static RtdbRestListener RtdbRestListener { get { return Instance.rtdbRestListener; } }

    public Look look;
    public static Look Look { get { return Instance.look; } }


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }
}
