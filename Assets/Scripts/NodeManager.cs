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

    [SerializeField] private Sprite regularNodeSprite;
    [SerializeField] private Sprite HQSprite;
    [SerializeField] private Sprite heliSprite;
    [SerializeField] private Sprite heavyHeliSprite;
    [SerializeField] private Sprite crateSprite;
    [SerializeField] private Sprite supplyFlagSprite;
    [SerializeField] private Sprite radarSprite;
    [SerializeField] private Sprite closedHeliSprite;
    [SerializeField] private Sprite closedHeavyHeliSprite;
    
    private List<GameObject> nodes = new List<GameObject>();
    private Dictionary<Faction, List<NodeBehavior>> surroundedNodes = new Dictionary<Faction, List<NodeBehavior>>();

    private bool isDragging = false;
    private Vector3 startPosition;

    private void Awake()
    {
        if (!surroundedNodes.ContainsKey(Faction.Blue))
            SetupDictionary();
    }

    private void FixedUpdate()
    {
        if (isDragging)
        {
            map.transform.position += Input.mousePosition - startPosition;
            startPosition = Input.mousePosition;
        }
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
    
    public NodeBehavior CreateNodeAtCoordinates(Vector2 location, Vector2 size, Faction owner, NodeType nodeType, List<Vector2> typeChangeList)
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
        behavior.SetNodeType(nodeType);
        
        foreach(Vector2 entry in typeChangeList)
        {
            behavior.AddTypeChangeOnTurn((int)entry.x, (NodeType)entry.y);
        }

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

    public int GetAPFromNodes()
    {
        int APFromNodes = 0;
        foreach(GameObject gameObject in nodes)
        {
            NodeBehavior node = gameObject.GetComponent<NodeBehavior>();
            switch (node.GetNodeType())
            {
                case NodeType.HQ:
                case NodeType.Helipad:
                case NodeType.HeavyHelipad:
                case NodeType.ClosedHeli:
                case NodeType.ClosedHeavyHeli:
                    if (node.GetOwner() == Faction.Blue)
                        APFromNodes++;
                    break;
            }
        }

        return APFromNodes;
    }

    public void ChangeNodesMarkedForTurn(int turn)
    {
        foreach(GameObject node in nodes)
        {
            node.GetComponent<NodeBehavior>().CheckTypeChange(turn);
        }
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

    public Sprite GetSpriteOfType(NodeType nodeType)
    {
        switch (nodeType)
        {
            case NodeType.Regular:
                return regularNodeSprite;
            case NodeType.HQ:
                return HQSprite;
            case NodeType.Helipad:
                return heliSprite;
            case NodeType.HeavyHelipad:
                return heavyHeliSprite;
            case NodeType.Crate:
                return crateSprite;
            case NodeType.SupplyFlag:
                return supplyFlagSprite;
            case NodeType.Radar:
                return radarSprite;
            case NodeType.ClosedHeli:
                return closedHeliSprite;
            case NodeType.ClosedHeavyHeli:
                return closedHeavyHeliSprite;
            default:
                return regularNodeSprite;
        }
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

    public void DragMap()
    {
        if (gameStateManager.gameState == GameStates.Default)
        {
            startPosition = Input.mousePosition;
            isDragging = true;
        }
    }

    public void StopDragging()
    {
        isDragging = false;
    }

    public void AdjustMapSize()
    {
        if (gameStateManager.gameState == GameStates.Default)
        {
            float scale = 0.05f;
            map.transform.localScale += new Vector3(Input.mouseScrollDelta.y * scale, Input.mouseScrollDelta.y * scale);
        }
    }

    public void SwitchToDefault()
    {
        if (gameStateManager.gameState == GameStates.PlacingNodes)
            gameStateManager.gameState = GameStates.Default;
    }

    public void SwitchToEditing()
    {
        if (gameStateManager.gameState == GameStates.Default)
            gameStateManager.gameState = GameStates.PlacingNodes;
    }
}
