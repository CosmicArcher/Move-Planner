using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NodeManager : MonoBehaviour
{
    [SerializeField] private GameObject nodeTemplate;
    [SerializeField] private GameObject map;
    [SerializeField] private NodeEditor editor;
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private MovePlanner planner;
    
    private List<GameObject> nodes = new List<GameObject>();
    private Dictionary<Faction, List<NodeBehavior>> surroundedNodes = new Dictionary<Faction, List<NodeBehavior>>();

    private void Awake()
    {
        if (!surroundedNodes.ContainsKey(Faction.Blue))
            SetupDictionary();
    }

    public void SetupDictionary()
    {
        surroundedNodes.Add(Faction.Neutral, new List<NodeBehavior>());
        surroundedNodes.Add(Faction.Blue, new List<NodeBehavior>());
        surroundedNodes.Add(Faction.Red, new List<NodeBehavior>());
        surroundedNodes.Add(Faction.Yellow, new List<NodeBehavior>());
    }

    public void CreateNode()
    {
        if (gameStateManager.gameState == GameStates.PlacingNodes)
        {
            GameObject newNode = Instantiate(nodeTemplate, Input.mousePosition, Quaternion.identity, map.transform);
            if (nodes.Count > 0)
                newNode.GetComponent<RectTransform>().sizeDelta = nodes[0].GetComponent<RectTransform>().sizeDelta;
            
            NodeBehavior behavior = newNode.GetComponent<NodeBehavior>();
            behavior.SetNodeManager(this);
            behavior.SetNodeEditor(editor);
            behavior.SetGameStateManager(gameStateManager);
            behavior.SetPlanner(planner);

            EventTrigger.Entry DragEntry = new EventTrigger.Entry();
            DragEntry.eventID = EventTriggerType.Drag;
            DragEntry.callback.AddListener(delegate { editor.MoveNode(); });
            newNode.GetComponent<EventTrigger>().triggers.Add(DragEntry);

            EventTrigger.Entry ClickEntry = new EventTrigger.Entry();
            ClickEntry.eventID = EventTriggerType.PointerDown;
            ClickEntry.callback.AddListener(delegate { behavior.SelectNode(); });
            newNode.GetComponent<EventTrigger>().triggers.Add(ClickEntry);

            nodes.Add(newNode);
        }
    }
    
    public NodeBehavior CreateNodeAtCoordinates(Vector2 location, Vector2 size, Faction owner)
    {
        if (!surroundedNodes.ContainsKey(Faction.Blue))
            SetupDictionary();

        GameObject newNode = Instantiate(nodeTemplate, map.transform);
        newNode.GetComponent<RectTransform>().anchoredPosition = location;

        NodeBehavior behavior = newNode.GetComponent<NodeBehavior>();
        behavior.SetNodeManager(this);
        behavior.SetNodeEditor(editor);
        behavior.SetGameStateManager(gameStateManager);
        behavior.SetPlanner(planner);
        
        EventTrigger.Entry DragEntry = new EventTrigger.Entry();
        DragEntry.eventID = EventTriggerType.Drag;
        DragEntry.callback.AddListener(delegate { editor.MoveNode(); });
        newNode.GetComponent<EventTrigger>().triggers.Add(DragEntry);
        
        EventTrigger.Entry ClickEntry = new EventTrigger.Entry();
        ClickEntry.eventID = EventTriggerType.PointerDown;
        ClickEntry.callback.AddListener(delegate { behavior.SelectNode(); });
        newNode.GetComponent<EventTrigger>().triggers.Add(ClickEntry);

        newNode.GetComponent<RectTransform>().sizeDelta = size;
        behavior.SetOwner(owner);

        nodes.Add(newNode);

        return behavior;
    }

    public void DeleteNode(GameObject node)
    {
        foreach (GameObject gameObject in nodes)
        {
            if (gameObject != node)
            {
                NodeBehavior nodeBehavior = gameObject.GetComponent<NodeBehavior>();
                if (nodeBehavior.GetConnectedNodes().Contains(node.GetComponent<NodeBehavior>()))
                    nodeBehavior.GetConnectedNodes().Remove(node.GetComponent<NodeBehavior>());
            }
        }
        nodes.Remove(node);
    }

    public void MarkNodeForSurround(NodeBehavior node, Faction surroundingFaction)
    {
        surroundedNodes[surroundingFaction].Add(node);
    }

    public void TriggerSurroundCaptures(Faction faction)
    {
        foreach (NodeBehavior node in surroundedNodes[faction])
        {
            if (node.CheckSurroundingFaction(faction))
                node.SetOwner(faction);
        }
        surroundedNodes[faction].Clear();
    }

    public List<GameObject> GetNodeList()
    {
        return nodes;
    }

    public void CheckInitiallySurroundedNodes()
    {
        foreach (GameObject node in nodes)
        {
            node.GetComponent<NodeBehavior>().CheckSurroundingFactions();
        }
    }
}
