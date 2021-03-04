using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EchelonsManager : MonoBehaviour
{
    private List<EchelonBehavior> regularEchelons = new List<EchelonBehavior>();
    private List<ParachuteBehavior> parachuteEchelons = new List<ParachuteBehavior>();
    private List<HOCBehavior> HOCs = new List<HOCBehavior>();
    private List<EchelonBehavior> NPCs = new List<EchelonBehavior>();
    private List<EchelonBehavior> enemies = new List<EchelonBehavior>();
    private List<EchelonBehavior> allEchelons = new List<EchelonBehavior>();

    [SerializeField] private MovePlanner planner;
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private GameObject mapImage;

    [SerializeField] private GameObject regularEcheTemplate;
    [SerializeField] private GameObject parachuteTemplate;
    [SerializeField] private GameObject HOCTemplate;
    [SerializeField] private GameObject NPCTemplate;
    [SerializeField] private GameObject regularEnemyTemplate;

    private EchelonBehavior selectedEchelon;
    private List<NodeBehavior> nodesInSelectedHOCRange;
    private Queue<int> retreatedEchelons = new Queue<int>();
    private Queue<int> retreatedHOCs = new Queue<int>();

    [SerializeField] private Text SelectedEchelonDescription;

    [SerializeField] private GameObject InputFieldHolder;
    [SerializeField] private Text InputFieldLabel;
    [SerializeField] private InputField InputField;
    [SerializeField] private InputField HOCRangeInput;
    private bool submitted = false;
    private bool makingHOC = false;

    [SerializeField] private GameObject enemyCreatorHolder;
    [SerializeField] private EnemyCreator enemyCreator;
    private bool creatingNewEnemy = false;

    [SerializeField] private GameObject spawnSetterHolder;
    [SerializeField] private EnemySpawnSetter spawnSetter;
    [SerializeField] private GameObject redSpawnPreview;
    [SerializeField] private GameObject yellowSpawnPreview;
    private bool hasSetEnemySpawn = false;

    [SerializeField] private GameObject enemySpawn;

    [SerializeField] private GameObject battleHandlerObject;
    [SerializeField] private GameObject attackerButton;
    [SerializeField] private GameObject defenderButton;
    private bool attackerWins = true;
    private bool combatSettled = false;

    public void AddEchelon(EchelonType type, NodeBehavior node)
    {
        bool valid = false;
        if (type == EchelonType.Enemy || type == EchelonType.NPC)
            valid = true;
        else if (node.GetOwner() == Faction.Blue)
        {
            if (node.GetNodeType() == NodeType.HeavyHelipad)
                valid = true;
            else if (type != EchelonType.HOC)
            {
                if (node.GetNodeType() == NodeType.HQ || node.GetNodeType() == NodeType.Helipad)
                    valid = true;
            }
        }

        if (valid)
        {
            GameObject newEchelon = null;
            switch (type)
            {
                case EchelonType.RegularGriffin:
                    newEchelon = Instantiate(regularEcheTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                    regularEchelons.Add(newEchelon.GetComponent<EchelonBehavior>());
                    planner.AddEchelon();
                    makingHOC = false;
                    StartCoroutine(InputEchelonName(newEchelon.GetComponent<EchelonBehavior>()));
                    break;
                case EchelonType.Parachute:
                    newEchelon = Instantiate(parachuteTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                    parachuteEchelons.Add(newEchelon.GetComponent<ParachuteBehavior>());
                    planner.AddEchelon();
                    makingHOC = false;
                    StartCoroutine(InputEchelonName(newEchelon.GetComponent<EchelonBehavior>()));
                    break;
                case EchelonType.HOC:
                    newEchelon = Instantiate(HOCTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                    HOCs.Add(newEchelon.GetComponent<HOCBehavior>());
                    planner.AddHOC();
                    makingHOC = true;
                    StartCoroutine(InputEchelonName(newEchelon.GetComponent<EchelonBehavior>()));
                    break;
                case EchelonType.NPC:
                    newEchelon = Instantiate(NPCTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                    NPCs.Add(newEchelon.GetComponent<EchelonBehavior>());
                    break;
                case EchelonType.Enemy:
                    newEchelon = Instantiate(regularEnemyTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                    enemies.Add(newEchelon.GetComponent<EchelonBehavior>());
                    StartCoroutine(InputEnemyAttributes(newEchelon.GetComponent<EchelonBehavior>()));
                    break;
            }
            EchelonBehavior behavior = newEchelon.GetComponent<EchelonBehavior>();
            behavior.SetNode(node);
            allEchelons.Add(behavior);

            if (type == EchelonType.RegularGriffin || type == EchelonType.Parachute)
            {
                if (retreatedEchelons.Count > 0)
                    behavior.SetID(retreatedEchelons.Dequeue());
                else
                    behavior.SetID(regularEchelons.Count + parachuteEchelons.Count);
            }
            else if (type == EchelonType.HOC)
            {
                if (retreatedHOCs.Count > 0)
                    behavior.SetID(retreatedHOCs.Dequeue());
                else
                    behavior.SetID(HOCs.Count);
            }

            if (gameStateManager.factionMoving == FactionTurn.Blue)
            {
                if (behavior.GetFaction() == Faction.Blue)
                    if (type != EchelonType.NPC)
                        planner.AdjustAP(-1);
            }

            newEchelon.transform.localScale = new Vector3(node.GetComponent<RectTransform>().rect.width / 200, node.GetComponent<RectTransform>().rect.height / 200, 1);

            EventTrigger.Entry ClickEntry = new EventTrigger.Entry();
            ClickEntry.eventID = EventTriggerType.PointerClick;
            ClickEntry.callback.AddListener(delegate { SelectEchelon(behavior); });
            newEchelon.GetComponent<EventTrigger>().triggers.Add(ClickEntry);
        }
    }

    private IEnumerator InputEchelonName(EchelonBehavior behavior)
    {
        InputFieldHolder.SetActive(true);
        InputFieldLabel.text = "Echelon Name:";

        if (makingHOC)
            HOCRangeInput.gameObject.SetActive(true);       

        submitted = false;
        while (!submitted)
        {
            yield return new WaitForEndOfFrame();
        }

        behavior.SetName(InputField.text);
        if (makingHOC)
        {
            (behavior as HOCBehavior).SetRange(int.Parse(HOCRangeInput.text));
            HOCRangeInput.text = "";
            HOCRangeInput.gameObject.SetActive(false);
        }

        InputFieldHolder.SetActive(false);
        InputField.text = "";
    }

    private IEnumerator InputEnemyAttributes(EchelonBehavior behavior)
    {
        enemyCreatorHolder.SetActive(true);
        creatingNewEnemy = false;
        while (!creatingNewEnemy)
        {
            yield return new WaitForEndOfFrame();
        }

        if (enemyCreator.GetEnemyID().Length > 0)
            behavior.SetID(int.Parse(enemyCreator.GetEnemyID()));

        behavior.SetFaction(enemyCreator.GetEnemyFaction());
        behavior.GetComponent<Image>().sprite = enemyCreator.GetChosenSprite();

        enemyCreator.ResetInputField();
        enemyCreatorHolder.SetActive(false);
    }

    public void SelectEchelon(EchelonBehavior echelon)
    {
        if (!MoveEcheToNode(echelon.GetNode()))
        {
            DeselectEchelon();
            selectedEchelon = echelon;
            selectedEchelon.GetComponent<Image>().color = Color.yellow;

            SelectedEchelonDescription.text = "Echelon: " + echelon.GetID() + "\nFaction: " + echelon.GetFaction();

            if (selectedEchelon.GetFaction() == Faction.Blue && !NPCs.Contains(selectedEchelon))
                SelectedEchelonDescription.text += "\nSupply: " + echelon.GetSupply().x + " / " + echelon.GetSupply().y;

            if (selectedEchelon.GetName().Length > 0)
                SelectedEchelonDescription.text = SelectedEchelonDescription.text.Insert(0,"Name: " + selectedEchelon.GetName() + "\n");

            ParachuteBehavior para = echelon as ParachuteBehavior;
            if (para != null)
                SelectedEchelonDescription.text += "\nParachute CD: " + para.GetCooldown();
            else
            {
                HOCBehavior HOC = echelon as HOCBehavior;
                if (HOC != null)
                {
                    SelectedEchelonDescription.text += "\nRange: " + HOC.GetRange();

                    nodesInSelectedHOCRange = HOC.GetAllNodesInRange();
                    HighlightNodesInHOCRange();
                }
            }
        }
    }

    private void HighlightNodesInHOCRange()
    {
        foreach(NodeBehavior node in nodesInSelectedHOCRange)
        {
            node.gameObject.GetComponent<Image>().color = Color.magenta;
        }
    }

    private void UnhighlightNodes()
    {
        foreach(NodeBehavior node in nodesInSelectedHOCRange)
        {
            switch (node.GetOwner())
            {
                case Faction.Neutral:
                    node.gameObject.GetComponent<Image>().color = Color.white;
                    break;
                case Faction.Blue:
                    node.gameObject.GetComponent<Image>().color = Color.cyan;
                    break;
                case Faction.Red:
                    node.gameObject.GetComponent<Image>().color = Color.red;
                    break;
                case Faction.Yellow:
                    node.gameObject.GetComponent<Image>().color = Color.yellow;
                    break;
            }
        }

        nodesInSelectedHOCRange.Clear();
    }

    public void DeselectEchelon()
    {
        if (selectedEchelon != null)
        {
            if (selectedEchelon as HOCBehavior != null)
                UnhighlightNodes();

            selectedEchelon.GetComponent<Image>().color = Color.white;
            selectedEchelon = null;
            SelectedEchelonDescription.text = "";
        }
    }

    public void RemoveSelectedEchelon()
    {
        RemoveEchelon(selectedEchelon);
        DeselectEchelon();
    }

    private void RemoveEchelon(EchelonBehavior echelon)
    {
        if (echelon != null)
        {
            allEchelons.Remove(echelon);
            if (echelon.GetFaction() != Faction.Blue)
            {
                enemies.Remove(echelon);
            }
            else
            {
                if (echelon as ParachuteBehavior != null)
                {
                    retreatedEchelons.Enqueue(echelon.GetID());
                    parachuteEchelons.Remove(echelon as ParachuteBehavior);
                    planner.RemoveEchelon();
                }
                else if (echelon as HOCBehavior != null)
                {
                    retreatedHOCs.Enqueue(echelon.GetID());
                    HOCs.Remove(echelon as HOCBehavior);
                    planner.RemoveHOC();
                }
                else
                {
                    if (regularEchelons.Contains(echelon))
                    {
                        retreatedEchelons.Enqueue(echelon.GetID());
                        regularEchelons.Remove(echelon);
                        planner.RemoveEchelon();
                    }
                    else
                        NPCs.Remove(echelon);
                }
            }
            Destroy(echelon.gameObject);
        }
    }

    public void CaptureNodesForFaction(Faction faction)
    {
        if (faction == Faction.Blue)
        {
            foreach (EchelonBehavior echelon in regularEchelons)
                echelon.CaptureNode();
            foreach (ParachuteBehavior echelon in parachuteEchelons)
            {
                echelon.CaptureNode();
                echelon.ReduceCooldown();
            }
            foreach (EchelonBehavior echelon in HOCs)
                echelon.CaptureNode();
            foreach (EchelonBehavior echelon in NPCs)
                echelon.CaptureNode();
        }
        else
        {
            foreach (EchelonBehavior echelon in enemies)
            {
                if (echelon.GetFaction() == faction)
                {
                    echelon.CaptureNode();
                }
            }
        }
    }

    public void ResupplyEchelon()
    {
        if (selectedEchelon != null)
        {
            NodeType attachedNode = selectedEchelon.GetNode().GetNodeType();
            if ((selectedEchelon.GetNode().GetOwner() == Faction.Blue && 
                (attachedNode == NodeType.HeavyHelipad || attachedNode == NodeType.Helipad || attachedNode == NodeType.HQ)) || attachedNode == NodeType.Crate)
            {
                selectedEchelon.Resupply();
                if (attachedNode == NodeType.Crate)
                    selectedEchelon.GetNode().SetNodeType(NodeType.Regular);
                DeselectEchelon();
            }
        }
    }

    public void ReduceRationsOnAll()
    {
        foreach (EchelonBehavior echelon in regularEchelons)
            echelon.UseSupply(new Vector2Int(0, 1));
        foreach (EchelonBehavior echelon in parachuteEchelons)
            echelon.UseSupply(new Vector2Int(0, 1));
        foreach (EchelonBehavior echelon in HOCs)
            echelon.UseSupply(new Vector2Int(0, 1));
    }

    private void UseCombatSupply(EchelonBehavior echelon)
    {
        echelon.UseSupply(new Vector2Int(1, 1));
    }

    public int GetAPFromEches()
    {
        return regularEchelons.Count + parachuteEchelons.Count + HOCs.Count;
    }

    public void SelectEchelonByNode(NodeBehavior node)
    {
        foreach(EchelonBehavior echelon in allEchelons)
        {
            if (echelon.GetNode() == node)
            {
                SelectEchelon(echelon);
                return;
            }
        }
    }

    private EchelonBehavior GetEchelonAtNode(NodeBehavior node)
    {
        foreach(EchelonBehavior echelon in allEchelons)
        {
            if (echelon.GetNode() == node)
                return echelon;
        }
        return null;
    }

    public bool MoveEcheToNode(NodeBehavior node)
    {
        if (selectedEchelon != null)
        {
            if (selectedEchelon.GetNode().GetConnectedNodes().Contains(node))
            {
                EchelonBehavior blockingEchelon = GetEchelonAtNode(node);
                if (blockingEchelon == null)
                {
                    if ((selectedEchelon.GetFaction() == Faction.Blue && planner.GetAP() > 0) || NPCs.Contains(selectedEchelon) || selectedEchelon.GetFaction() != Faction.Blue)
                    {
                        selectedEchelon.SetNode(node);
                        selectedEchelon.gameObject.transform.position = node.gameObject.transform.position;

                        if (selectedEchelon.GetFaction() == Faction.Blue && !NPCs.Contains(selectedEchelon))
                            planner.AdjustAP(-1);

                        DeselectEchelon();
                        return true;
                    }
                }
                else
                {
                    if (blockingEchelon.GetFaction() != selectedEchelon.GetFaction())
                    {
                        if (selectedEchelon as HOCBehavior == null)
                        {
                            if ((selectedEchelon.GetFaction() == Faction.Blue && planner.GetAP() > 0) ||
                                NPCs.Contains(selectedEchelon) || selectedEchelon.GetFaction() != Faction.Blue)
                            {
                                if (selectedEchelon.GetFaction() == Faction.Blue && !NPCs.Contains(selectedEchelon))
                                {
                                    if (selectedEchelon.GetSupply().x > 0 && selectedEchelon.GetSupply().y > 0)
                                    {
                                        UseCombatSupply(selectedEchelon);
                                        planner.AdjustAP(-1);
                                    }
                                    else
                                        return false;
                                }

                                if (blockingEchelon as HOCBehavior != null || 
                                    ((regularEchelons.Contains(blockingEchelon) || blockingEchelon as ParachuteBehavior != null) &&
                                    (blockingEchelon.GetSupply().x == 0 || blockingEchelon.GetSupply().y == 0)))
                                {
                                    selectedEchelon.SetNode(node);
                                    selectedEchelon.gameObject.transform.position = node.gameObject.transform.position;
                                    RemoveEchelon(blockingEchelon);
                                }
                                else
                                {
                                    if (blockingEchelon.GetFaction() == Faction.Blue || selectedEchelon.GetFaction() == Faction.Blue)
                                    {
                                        foreach (HOCBehavior HOC in HOCs)
                                            if (HOC.IsNodeInRange(blockingEchelon.GetNode()))
                                                HOC.UseSupply(new Vector2Int(1, 2));
                                    }

                                    StartCoroutine(DetermineCombat(blockingEchelon));
                                }
                                return true;
                            }
                        }
                    }
                    else if (selectedEchelon.GetFaction() == Faction.Blue)
                    {
                        blockingEchelon.SetNode(selectedEchelon.GetNode());
                        blockingEchelon.gameObject.transform.position = selectedEchelon.GetNode().gameObject.transform.position;
                        selectedEchelon.SetNode(node);
                        selectedEchelon.gameObject.transform.position = node.gameObject.transform.position;

                        DeselectEchelon();
                        return true;
                    }
                }
            }
            else if (selectedEchelon as ParachuteBehavior != null)
            {
                ParachuteBehavior para = selectedEchelon as ParachuteBehavior;
                if (para.GetCooldown() == 0)
                {
                    if (GetEchelonAtNode(node) == null && (node.GetNodeType() == NodeType.Helipad || node.GetNodeType() == NodeType.HeavyHelipad ||
                        node.GetNodeType() == NodeType.ClosedHeli || node.GetNodeType() == NodeType.ClosedHeavyHeli))
                    {
                        para.UsePara();
                        selectedEchelon.SetNode(node);
                        selectedEchelon.gameObject.transform.position = node.gameObject.transform.position;

                        DeselectEchelon();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void SpawnEnemiesAtHelis(List<GameObject> nodes, Faction faction) 
    {
        foreach(GameObject nodeObject in nodes)
        {
            NodeBehavior node = nodeObject.GetComponent<NodeBehavior>();
            if (GetEchelonAtNode(node) == null)
            {
                if (node.GetOwner() == faction)
                {
                    if (node.GetNodeType() == NodeType.Helipad || node.GetNodeType() == NodeType.HeavyHelipad)
                    {
                        GameObject newEchelon = Instantiate(enemySpawn, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                        if (faction == Faction.Red)
                            newEchelon.GetComponent<Image>().sprite = redSpawnPreview.GetComponentsInChildren<Image>()[1].sprite;
                        else if (faction == Faction.Yellow)
                            newEchelon.GetComponent<Image>().sprite = yellowSpawnPreview.GetComponentsInChildren<Image>()[1].sprite;

                        EchelonBehavior behavior = newEchelon.GetComponent<EchelonBehavior>();
                        enemies.Add(behavior);

                        behavior.SetFaction(faction);
                        behavior.SetNode(node);
                        allEchelons.Add(behavior);

                        newEchelon.transform.localScale = new Vector3(node.GetComponent<RectTransform>().rect.width / 200, node.GetComponent<RectTransform>().rect.height / 200, 1);

                        EventTrigger.Entry ClickEntry = new EventTrigger.Entry();
                        ClickEntry.eventID = EventTriggerType.PointerClick;
                        ClickEntry.callback.AddListener(delegate { SelectEchelon(behavior); });
                        newEchelon.GetComponent<EventTrigger>().triggers.Add(ClickEntry);
                    }
                }
            }
        }
    }

    public void AdjustEnemySpawnSetting(int faction)
    {
        StartCoroutine(SetEnemySpawnSetting((Faction)faction));
    }

    private IEnumerator SetEnemySpawnSetting(Faction faction)
    {
        if (faction == Faction.Red)
            redSpawnPreview.GetComponent<Image>().color = Color.yellow;
        else if (faction == Faction.Yellow)
            yellowSpawnPreview.GetComponent<Image>().color = Color.yellow;

        spawnSetterHolder.SetActive(true);

        hasSetEnemySpawn = false;
        while (!hasSetEnemySpawn)
        {
            yield return new WaitForEndOfFrame();
        }

        if (faction == Faction.Red)
        {
            redSpawnPreview.GetComponent<Image>().color = Color.white;
            redSpawnPreview.GetComponentsInChildren<Image>()[1].sprite = spawnSetter.GetChosenSprite();
        }
        else if (faction == Faction.Yellow)
        {
            yellowSpawnPreview.GetComponent<Image>().color = Color.white;
            yellowSpawnPreview.GetComponentsInChildren<Image>()[1].sprite = spawnSetter.GetChosenSprite();
        }

        spawnSetterHolder.SetActive(false);
    }

    public void Submit()
    {
        if (InputField.text.Length > 0)
        {
            if (makingHOC)
            {
                int x;
                if (int.TryParse(HOCRangeInput.text, out x))
                    submitted = true;
            }
            else
                submitted = true;
        }
    }

    public void CreateNewEnemy()
    {
        creatingNewEnemy = true;
    }

    public void SetEnemySpawn()
    {
        hasSetEnemySpawn = true;
    }

    public IEnumerator DetermineCombat(EchelonBehavior defender)
    {
        battleHandlerObject.SetActive(true);
        attackerButton.GetComponentsInChildren<Image>()[1].sprite = selectedEchelon.GetComponent<Image>().sprite;
        attackerButton.GetComponent<Image>().color = Color.yellow;
        defenderButton.GetComponentsInChildren<Image>()[1].sprite = defender.gameObject.GetComponent<Image>().sprite;
        defenderButton.GetComponent<Image>().color = Color.white;

        attackerWins = true;
        combatSettled = false;

        while(!combatSettled)
        {
            yield return new WaitForEndOfFrame();
        }

        if (attackerWins)
        {
            selectedEchelon.SetNode(defender.GetNode());
            selectedEchelon.gameObject.transform.position = defender.GetNode().gameObject.transform.position;
            RemoveEchelon(defender);
        }
        else
        {
            RemoveEchelon(selectedEchelon);
            if (defender.GetFaction() == Faction.Blue && (regularEchelons.Contains(defender) || parachuteEchelons.Contains(defender as ParachuteBehavior)))
                UseCombatSupply(defender);
        }

        DeselectEchelon();
        battleHandlerObject.SetActive(false);
    }

    public void SelectAttacker()
    {
        attackerButton.GetComponent<Image>().color = Color.yellow;
        defenderButton.GetComponent<Image>().color = Color.white;
        attackerWins = true;
    }

    public void SelectDefender()
    {
        attackerButton.GetComponent<Image>().color = Color.white;
        defenderButton.GetComponent<Image>().color = Color.yellow;
        attackerWins = false;
    }

    public void CompleteCombat()
    {
        combatSettled = true;
    }
}
