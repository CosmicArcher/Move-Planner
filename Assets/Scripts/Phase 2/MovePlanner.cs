using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePlanner : MonoBehaviour
{
    [SerializeField] private GameObject statusHolder;
    [SerializeField] private GameObject AddEchelonsHolder;
    [SerializeField] private GameObject helpHolder;

    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private EchelonsManager echelonsManager;
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private AddEchelonsMenu addEchelonsMenu;

    [SerializeField] private Text factionMovingText;
    [SerializeField] private Text turnText;
    [SerializeField] private Text APText;
    [SerializeField] private Text echelonsDeployedText;
    [SerializeField] private Text hocsDeployedText;

    [SerializeField] private GameObject inputFieldHolder;
    [SerializeField] private Text inputFieldLabel;
    [SerializeField] private InputField inputField;
    private bool submitted = false;

    [SerializeField] private Text endTurnText;

    [SerializeField] private Button redHasTurnInput;
    [SerializeField] private Button yellowHasTurnInput;

    private int AP = 0;
    private int turn = 0;
    private int echelonsDeployed = 0;
    private int hocsDeployed = 0;

    private bool redHasTurn = true;
    private bool yellowHasTurn = false;

    public void AddEchelon()
    {
        echelonsDeployed++;
        echelonsDeployedText.text = "Echelons Deployed: " + echelonsDeployed + "/10";
    }

    public void RemoveEchelon()
    {
        echelonsDeployed--;
        echelonsDeployedText.text = "Echelons Deployed: " + echelonsDeployed + "/10";
    }

    public void AddHOC()
    {
        hocsDeployed++;
        hocsDeployedText.text = "HOCs Deployed: " + hocsDeployed + "/4";
    }

    public void RemoveHOC()
    {
        hocsDeployed--;
        hocsDeployedText.text = "HOCs Deployed: " + hocsDeployed + "/4";
    }

    public IEnumerator SetAP()
    {
        inputFieldHolder.SetActive(true);
        inputFieldLabel.text = "Starting AP for the turn:";

        submitted = false;
        while (!submitted)
        {
            yield return new WaitForEndOfFrame();
        }

        AP = int.Parse(inputField.text);
        APText.text = "AP: " + AP;
        inputField.text = "";
        inputFieldHolder.SetActive(false);
    }

    public void AdjustAP(int amount)
    {
        AP += amount;
        APText.text = "AP: " + AP;
    }

    public int GetAP()
    {
        return AP;
    }

    public IEnumerator SetTurn()
    {
        inputFieldHolder.SetActive(true);
        inputFieldLabel.text = "Starting Turn:";

        submitted = false;
        while (!submitted)
        {
            yield return new WaitForEndOfFrame();
        }

        turn = int.Parse(inputField.text);
        turnText.text = "Turn: " + turn;
        inputField.text = "";
        inputFieldHolder.SetActive(false);

        StartCoroutine(SetAP());
    }

    public void NextTurn()
    {
        StartCoroutine(SetAP());
        turn++;
        turnText.text = "Turn: " + turn;
    }

    public void CheckAddEchelonsTab()
    {
        AddEchelonsHolder.SetActive(true);
        statusHolder.SetActive(false);
        helpHolder.SetActive(false);
    }

    public void CheckStatus()
    {
        AddEchelonsHolder.SetActive(false);
        statusHolder.SetActive(true);
        helpHolder.SetActive(false);
    }

    public void CheckHelp()
    {
        AddEchelonsHolder.SetActive(false);
        statusHolder.SetActive(false);
        helpHolder.SetActive(true);
    }

    public void SelectNode(NodeBehavior node)
    {
        if (gameStateManager.gameState == GameStates.Default)
        {
            if (!echelonsManager.MoveEcheToNode(node))
                echelonsManager.SelectEchelonByNode(node);
        }
        else
            addEchelonsMenu.AddEchelonToNode(node);
    }

    public void EndTurn()
    {
        if (gameStateManager.factionMoving != FactionTurn.Setup)
        {
            echelonsManager.CaptureNodesForFaction(ConvertFactionTurnToFaction(gameStateManager.factionMoving));
        }
        switch (gameStateManager.factionMoving)
        {
            case FactionTurn.Blue:
                if (redHasTurn)
                {
                    nodeManager.TriggerSurroundCaptures(Faction.Red);
                    gameStateManager.factionMoving = FactionTurn.Red;
                }
                else if (yellowHasTurn)
                {
                    nodeManager.TriggerSurroundCaptures(Faction.Yellow);
                    gameStateManager.factionMoving = FactionTurn.Yellow;
                }
                else
                {
                    nodeManager.TriggerSurroundCaptures(Faction.Blue);
                    NextTurn();
                }
                break;
            case FactionTurn.Red:
                if (yellowHasTurn)
                {
                    nodeManager.TriggerSurroundCaptures(Faction.Yellow);
                    gameStateManager.factionMoving = FactionTurn.Yellow;
                }
                else
                {
                    nodeManager.TriggerSurroundCaptures(Faction.Blue);
                    gameStateManager.factionMoving = FactionTurn.Blue;
                    NextTurn();
                }
                break;
            case FactionTurn.Yellow:
                nodeManager.TriggerSurroundCaptures(Faction.Blue);
                gameStateManager.factionMoving = FactionTurn.Blue;
                NextTurn();
                break;
            case FactionTurn.Setup:
                gameStateManager.factionMoving = FactionTurn.Blue;
                endTurnText.text = "End Turn";
                nodeManager.CheckInitiallySurroundedNodes();
                StartCoroutine(SetTurn());
                break;
        }
        factionMovingText.text = gameStateManager.factionMoving + "'s Turn";
    }

    private Faction ConvertFactionTurnToFaction(FactionTurn factionTurn)
    {
        switch(factionTurn)
        {
            case FactionTurn.Blue:
                return Faction.Blue;
            case FactionTurn.Red:
                return Faction.Red;
            case FactionTurn.Yellow:
                return Faction.Yellow;
            default:
                return Faction.Blue;
        }
    }

    public void ChangeYellowHasTurn()
    {
        yellowHasTurn = !yellowHasTurn;
        if (yellowHasTurnInput.image.color == Color.white)
            yellowHasTurnInput.image.color = Color.gray;
        else
            yellowHasTurnInput.image.color = Color.white;
    }
    
    public void ChangeRedHasTurn()
    {
        redHasTurn = !redHasTurn;
        if (redHasTurnInput.image.color == Color.white)
            redHasTurnInput.image.color = Color.gray;
        else
            redHasTurnInput.image.color = Color.white;
    }

    public void Submit()
    {
        int x;
        if (int.TryParse(inputField.text, out x))
            submitted = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
