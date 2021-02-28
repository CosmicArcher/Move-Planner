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
    KCCO = 4,
    Deathstack = 5,
    ELID = 6,
    Paradeus = 7
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
    [SerializeField] private Image KCCOHolder;
    [SerializeField] private Image DeathstackHolder;
    [SerializeField] private Image ELIDHolder;
    [SerializeField] private Image ParadeusHolder;

    private EchelonType echelonToPlace;

    public void AddNormalEchelon()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.RegularGriffin;

        NormalEchelonHolder.color = Color.yellow;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        KCCOHolder.color = Color.white;
        DeathstackHolder.color = Color.white;
        ELIDHolder.color = Color.white;
        ParadeusHolder.color = Color.white;
    }
    public void AddParachute()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.Parachute;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.yellow;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        KCCOHolder.color = Color.white;
        DeathstackHolder.color = Color.white;
        ELIDHolder.color = Color.white;
        ParadeusHolder.color = Color.white;
    }
    public void AddHOC()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.HOC;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.yellow;
        NPCHolder.color = Color.white;
        KCCOHolder.color = Color.white;
        DeathstackHolder.color = Color.white;
        ELIDHolder.color = Color.white;
        ParadeusHolder.color = Color.white;
    }
    public void AddNPC()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.NPC;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.yellow;
        KCCOHolder.color = Color.white;
        DeathstackHolder.color = Color.white;
        ELIDHolder.color = Color.white;
        ParadeusHolder.color = Color.white;
    }
    public void AddEnemy()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.KCCO;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        KCCOHolder.color = Color.yellow;
        DeathstackHolder.color = Color.white;
        ELIDHolder.color = Color.white;
        ParadeusHolder.color = Color.white;
    }
    public void AddDeathstack()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.Deathstack;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        KCCOHolder.color = Color.white;
        DeathstackHolder.color = Color.yellow;
        ELIDHolder.color = Color.white;
        ParadeusHolder.color = Color.white;
    }
    public void AddELID()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.ELID;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        KCCOHolder.color = Color.white;
        DeathstackHolder.color = Color.white;
        ELIDHolder.color = Color.yellow;
        ParadeusHolder.color = Color.white;
    }
    public void AddParadeus()
    {
        gameStateManager.gameState = GameStates.PlacingEchelon;
        echelonToPlace = EchelonType.Paradeus;

        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        KCCOHolder.color = Color.white;
        DeathstackHolder.color = Color.white;
        ELIDHolder.color = Color.white;
        ParadeusHolder.color = Color.yellow;
    }

    public void Cancel()
    {
        gameStateManager.gameState = GameStates.Default;
        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        KCCOHolder.color = Color.white;
        DeathstackHolder.color = Color.white;
        ELIDHolder.color = Color.white;
        ParadeusHolder.color = Color.white;
    }

    public void AddEchelonToNode(NodeBehavior node)
    {
        gameStateManager.gameState = GameStates.Default;
        NormalEchelonHolder.color = Color.white;
        ParachuteHolder.color = Color.white;
        HOCHolder.color = Color.white;
        NPCHolder.color = Color.white;
        KCCOHolder.color = Color.white;
        DeathstackHolder.color = Color.white;
        ELIDHolder.color = Color.white;
        ParadeusHolder.color = Color.white;

        echelonsManager.AddEchelon(echelonToPlace, node);
    }
}
