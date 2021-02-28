using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EchelonType
{
    RegularGriffin = 0,
    Parachute = 1,
    HOC = 2,
    NPC = 3,
    Enemy = 4
}

public class AddEchelonsMenu : MonoBehaviour
{
    [SerializeField] private EchelonsManager echelonsManager;
    [SerializeField] private MovePlanner planner;
    [SerializeField] private GameStateManager gameStateManager;

    [SerializeField] private Image NormalEchelonHolder;
    [SerializeField] private Image ParachuteHolder;
    [SerializeField] private Image HOCHolder;
    [SerializeField] private Image NPCHolder;
    [SerializeField] private Image EnemyHolder;

    private EchelonType echelonToPlace;

    public void AddNormalEchelon()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.RegularGriffin;

        NormalEchelonHolder.color = Color.yellow;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        EnemyHolder.color = Color.white;
    }
    public void AddParachute()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.Parachute;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.yellow;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        EnemyHolder.color = Color.white;
    }
    public void AddHOC()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.HOC;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.yellow;
        NPCHolder.color = Color.white;
        EnemyHolder.color = Color.white;
    }
    public void AddNPC()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.NPC;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.yellow;
        EnemyHolder.color = Color.white;
    }
    public void AddEnemy()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.Enemy;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        EnemyHolder.color = Color.yellow;
    }

    public void Cancel()
    {
        gameStateManager.gameState = GameStates.Default;
        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        EnemyHolder.color = Color.white;
    }

    public void AddEchelonToNode(NodeBehavior node)
    {
        gameStateManager.gameState = GameStates.Default;
        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        EnemyHolder.color = Color.white;

        echelonsManager.AddEchelon(echelonToPlace, node);
    }
}
