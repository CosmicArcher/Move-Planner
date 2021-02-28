using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    Default = 0,
    PlacingNodes = 1,
    Connecting = 2,
    PlacingEchelon = 3,
    DisconnectingNodes = 4
}

public enum FactionTurn
{
    Blue = 0,
    Red = 1,
    Yellow = 2,
    Setup = 3
}

public class GameStateManager : MonoBehaviour
{
    public GameStates gameState = GameStates.Default;
    public FactionTurn factionMoving = FactionTurn.Setup;
}
