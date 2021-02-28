using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeEditor : MonoBehaviour
{
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private MapDataFileHandler mapSaver;
    [SerializeField] private GameObject editorHolder;
    [SerializeField] private GameObject plannerHolder;

    private NodeBehavior selectedNode;

    [SerializeField] private GameObject mapNameInputHolder;
    [SerializeField] private Text mapNameInputLabel;
    [SerializeField] private InputField mapNameInput;
    private bool submitted = false;

    private List<GameObject> highlightedNodes = new List<GameObject>();

    public void SelectNode(NodeBehavior node)
    {
        if (node != selectedNode)
        {
            if (gameStateManager.gameState == GameStates.PlacingNodes)
            {
                DeselectNode();
                selectedNode = node;
                selectedNode.GetComponent<Image>().color = Color.green;
                foreach (NodeBehavior adjacent in node.GetConnectedNodes())
                {
                    highlightedNodes.Add(adjacent.gameObject);
                    adjacent.GetComponent<Image>().color = Color.magenta;
                }
            }
            else if (gameStateManager.gameState == GameStates.Connecting)
            {
                ConnectNodes(node);
            }
            else if (gameStateManager.gameState == GameStates.DisconnectingNodes)
            {
                DisconnectNodes(node);
            }
        }
    }

    public void DeleteNode()
    {
        if (selectedNode != null)
        {
            foreach (GameObject adjacent in highlightedNodes)
            {
                switch (adjacent.GetComponent<NodeBehavior>().GetOwner())
                {
                    case Faction.Neutral:
                        adjacent.gameObject.GetComponent<Image>().color = Color.white;
                        break;
                    case Faction.Blue:
                        adjacent.gameObject.GetComponent<Image>().color = Color.cyan;
                        break;
                    case Faction.Red:
                        adjacent.gameObject.GetComponent<Image>().color = Color.red;
                        break;
                    case Faction.Yellow:
                        adjacent.gameObject.GetComponent<Image>().color = Color.yellow;
                        break;
                }
            }

            highlightedNodes.Clear();

            nodeManager.DeleteNode(selectedNode.gameObject);
            Destroy(selectedNode.gameObject);
        }
    }

    public void IncreaseNodeSize()
    {
        foreach (GameObject node in nodeManager.GetNodeList())
            node.GetComponent<RectTransform>().sizeDelta += new Vector2(10, 10);
    }

    public void DecreaseNodeSize()
    {
        foreach (GameObject node in nodeManager.GetNodeList())
            node.GetComponent<RectTransform>().sizeDelta -= new Vector2(10, 10);
    }

    public void MoveNode()
    {
        if (gameStateManager.gameState == GameStates.PlacingNodes)
            selectedNode.gameObject.transform.position = Input.mousePosition;
    }

    public void DeselectNode()
    {
        if (selectedNode != null)
        {
            switch (selectedNode.GetOwner())
            {
                case Faction.Neutral:
                    selectedNode.gameObject.GetComponent<Image>().color = Color.white;
                    break;
                case Faction.Blue:
                    selectedNode.gameObject.GetComponent<Image>().color = Color.cyan;
                    break;
                case Faction.Red:
                    selectedNode.gameObject.GetComponent<Image>().color = Color.red;
                    break;
                case Faction.Yellow:
                    selectedNode.gameObject.GetComponent<Image>().color = Color.yellow;
                    break;
            }
            selectedNode = null;

            gameStateManager.gameState = GameStates.PlacingNodes;

            foreach (GameObject adjacent in highlightedNodes)
            {
                switch (adjacent.GetComponent<NodeBehavior>().GetOwner())
                {
                    case Faction.Neutral:
                        adjacent.gameObject.GetComponent<Image>().color = Color.white;
                        break;
                    case Faction.Blue:
                        adjacent.gameObject.GetComponent<Image>().color = Color.cyan;
                        break;
                    case Faction.Red:
                        adjacent.gameObject.GetComponent<Image>().color = Color.red;
                        break;
                    case Faction.Yellow:
                        adjacent.gameObject.GetComponent<Image>().color = Color.yellow;
                        break;
                }
            }

            highlightedNodes.Clear();
        }
    }

    public void AssignFaction(int faction)
    {
        if (selectedNode != null)
        {
            selectedNode.SetOwner((Faction)faction);
            DeselectNode();
        }
    }

    public void BeginConnecting()
    {
        if (selectedNode != null)
            gameStateManager.gameState = GameStates.Connecting;
    }

    public void BeginDisconnecting()
    {
        if (selectedNode != null)
            gameStateManager.gameState = GameStates.DisconnectingNodes;
    }

    private void ConnectNodes(NodeBehavior otherNode)
    {
        selectedNode.SetConnectedNode(otherNode);
        otherNode.SetConnectedNode(selectedNode);
        gameStateManager.gameState = GameStates.PlacingNodes;
        DeselectNode();
    }

    private void DisconnectNodes(NodeBehavior otherNode)
    {
        selectedNode.RemoveConnectedNode(otherNode);
        otherNode.RemoveConnectedNode(selectedNode);
        gameStateManager.gameState = GameStates.DisconnectingNodes;
        DeselectNode();
    }

    public void SaveMapAndBeginPlanner()
    {
        DeselectNode();
        StartCoroutine(InputMapName());
    }

    private IEnumerator InputMapName()
    {
        mapNameInputHolder.SetActive(true);
        mapNameInputLabel.text = "Map Name:";

        submitted = false;
        while (!submitted)
        {
            yield return new WaitForEndOfFrame();
        }

        mapSaver.SaveFile(mapNameInput.text);
        gameStateManager.gameState = GameStates.Default;
        editorHolder.SetActive(false);
        plannerHolder.SetActive(true);

        mapNameInput.text = "";
        mapNameInputHolder.SetActive(false);
    }

    public void Submit()
    {
        if (mapNameInput.text.Length > 0)
            submitted = true;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
