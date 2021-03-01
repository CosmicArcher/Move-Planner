using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Faction
{
    Neutral = 0, 
    Blue = 1, 
    Red = 2, 
    Yellow = 3
}

public enum NodeType
{
    Regular = 0,
    HQ = 1,
    Helipad = 2,
    HeavyHelipad = 3,
    Crate = 4,
    SupplyFlag = 5,
    Radar = 6,
    ClosedHeli = 7,
    ClosedHeavyHeli = 8
}

public class NodeBehavior : MonoBehaviour
{
    private List<NodeBehavior> connectedNodes = new List<NodeBehavior>();
    private int[] adjacentFactionsCount = new int[4];
    private Faction owner = Faction.Neutral;
    private NodeManager manager;
    private NodeEditor editor;
    private GameStateManager gameStateManager;
    private MovePlanner planner;
    private NodeType type;
    private Sprite baseSprite;

    private void Awake()
    {
        for (int i = 0; i < adjacentFactionsCount.Length; i++)
            adjacentFactionsCount[i] = 0;
    }

    public void SetOwner(Faction newOwner)
    {
        if (newOwner != owner)
        {
            for (int i = 0; i < connectedNodes.Count; i++)
            {
                connectedNodes[i].AdjustSurroundingFactions(newOwner, owner);
            }
            owner = newOwner;
            switch (newOwner)
            {
                case Faction.Neutral:
                    this.GetComponent<Image>().color = Color.white;
                    break;
                case Faction.Blue:
                    this.GetComponent<Image>().color = Color.cyan;
                    break;
                case Faction.Red:
                    this.GetComponent<Image>().color = Color.red;
                    break;
                case Faction.Yellow:
                    this.GetComponent<Image>().color = Color.yellow;
                    break;
            }
            CheckSurroundingFactions();
        }
    }

    public void SetConnectedNode(NodeBehavior connectedNode)
    {
        connectedNodes.Add(connectedNode);
        adjacentFactionsCount[(int)connectedNode.GetOwner()]++;
    }

    public void RemoveConnectedNode(NodeBehavior connectedNode)
    {
        if (connectedNodes.Contains(connectedNode))
        {
            connectedNodes.Remove(connectedNode);
            adjacentFactionsCount[(int)connectedNode.GetOwner()]--;
        }
    }

    public Faction GetOwner()
    {
        return owner;
    }

    public void SetNodeType(NodeType nodeType)
    {
        if (baseSprite == null)
            baseSprite = gameObject.GetComponent<Image>().sprite;
        switch (nodeType)
        {
            case NodeType.Regular:
                gameObject.GetComponent<Image>().sprite = baseSprite;
                break;
            case NodeType.HQ:
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/HQ");
                break;
            case NodeType.Helipad:
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Heli");
                break;
            case NodeType.HeavyHelipad:
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/HeavyHeli");
                break;
            case NodeType.Crate:
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Crate");
                break;
            case NodeType.SupplyFlag:
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/SupplyFlag");
                break;
            case NodeType.Radar:
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Radar");
                break;
            case NodeType.ClosedHeli:
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/ClosedHeli");
                break;
            case NodeType.ClosedHeavyHeli:
                gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/ClosedHeavyHeli");
                break;
        }
        type = nodeType;
    }

    public NodeType GetNodeType()
    {
        return type;
    }

    public void SetNodeManager(NodeManager nodeManager)
    {
        manager = nodeManager;
    }
    
    public void SetNodeEditor(NodeEditor nodeEditor)
    {
        editor = nodeEditor;
    }

    public void SetGameStateManager(GameStateManager stateManager)
    {
        gameStateManager = stateManager;
    }

    public void SetPlanner(MovePlanner movePlanner)
    {
        planner = movePlanner;
    }

    public List<NodeBehavior> GetConnectedNodes()
    {
        return connectedNodes;
    }

    public void AdjustSurroundingFactions(Faction newFaction, Faction oldFaction)
    {
        adjacentFactionsCount[(int)newFaction]++;
        adjacentFactionsCount[(int)oldFaction]--;
        CheckSurroundingFactions();
    }

    public void CheckSurroundingFactions()
    {
        for (int i = 1; i < adjacentFactionsCount.Length; i++)
        {
            if (CheckSurroundingFaction(i))
            {
                if (i != (int)owner)
                {
                    manager.MarkNodeForSurround(this, (Faction)i);
                    return;
                }
            }
        }
    }

    public bool CheckSurroundingFaction(Faction faction)
    {
        if (adjacentFactionsCount[(int)faction] == connectedNodes.Count && connectedNodes.Count > 0)
            return true;
        else
            return false;
    }

    public bool CheckSurroundingFaction(int faction)
    {
        if (adjacentFactionsCount[faction] == connectedNodes.Count && connectedNodes.Count > 0)
            return true;
        else
            return false;
    }

    public void SelectNode()
    {
        if (gameStateManager.gameState == GameStates.PlacingNodes || gameStateManager.gameState == GameStates.Connecting || gameStateManager.gameState == GameStates.DisconnectingNodes)
        {
            editor.SelectNode(this);
        }
        else
        {
            planner.SelectNode(this);
        }
    }
}
