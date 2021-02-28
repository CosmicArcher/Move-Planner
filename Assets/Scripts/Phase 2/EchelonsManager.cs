using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EchelonsManager : MonoBehaviour
{
    private List<EchelonBehavior> regularEchelons = new List<EchelonBehavior>();
    private List<ParachuteBehavior> parachuteEchelons = new List<ParachuteBehavior>();
    private List<EchelonBehavior> HOCs = new List<EchelonBehavior>();
    private List<EchelonBehavior> NPCs = new List<EchelonBehavior>();
    private List<EchelonBehavior> regularEnemies = new List<EchelonBehavior>();
    private List<EchelonBehavior> deathstacks = new List<EchelonBehavior>();
    private List<EchelonBehavior> allEchelons = new List<EchelonBehavior>();

    [SerializeField] private MovePlanner planner;
    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private GameObject mapImage;

    [SerializeField] private GameObject regularEcheTemplate;
    [SerializeField] private GameObject parachuteTemplate;
    [SerializeField] private GameObject HOCTemplate;
    [SerializeField] private GameObject NPCTemplate;
    [SerializeField] private GameObject regularEnemyTemplate;
    [SerializeField] private GameObject deathstackTemplate;
    [SerializeField] private GameObject ELIDTemplate;
    [SerializeField] private GameObject ParadeusTemplate;

    private EchelonBehavior selectedEchelon;
    private Queue<int> retreatedEchelons = new Queue<int>();
    private Queue<int> retreatedHOCs = new Queue<int>();

    [SerializeField] private Text SelectedEchelonDescription;

    [SerializeField] private GameObject InputFieldHolder;
    [SerializeField] private Text InputFieldLabel;
    [SerializeField] private InputField InputField;
    private bool submitted = false;
    private EchelonType newestType;

    public void AddEchelon(EchelonType type, NodeBehavior node)
    {
        newestType = type;
        GameObject newEchelon = null;
        switch (type)
        {
            case EchelonType.RegularGriffin:
                newEchelon = Instantiate(regularEcheTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                regularEchelons.Add(newEchelon.GetComponent<EchelonBehavior>());
                planner.AddEchelon();
                StartCoroutine(InputEchelonName(newEchelon.GetComponent<EchelonBehavior>()));
                break;
            case EchelonType.Parachute:
                newEchelon = Instantiate(parachuteTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                parachuteEchelons.Add(newEchelon.GetComponent<ParachuteBehavior>());
                planner.AddEchelon();
                StartCoroutine(InputEchelonName(newEchelon.GetComponent<EchelonBehavior>()));
                break;
            case EchelonType.HOC:
                newEchelon = Instantiate(HOCTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                HOCs.Add(newEchelon.GetComponent<EchelonBehavior>());
                planner.AddHOC();
                StartCoroutine(InputEchelonName(newEchelon.GetComponent<EchelonBehavior>()));
                break;
            case EchelonType.NPC:
                newEchelon = Instantiate(NPCTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                NPCs.Add(newEchelon.GetComponent<EchelonBehavior>());
                break;
            case EchelonType.KCCO:
                newEchelon = Instantiate(regularEnemyTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                regularEnemies.Add(newEchelon.GetComponent<EchelonBehavior>());
                break;
            case EchelonType.Deathstack:
                newEchelon = Instantiate(deathstackTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                deathstacks.Add(newEchelon.GetComponent<EchelonBehavior>());
                break;
            case EchelonType.ELID:
                newEchelon = Instantiate(ELIDTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                regularEnemies.Add(newEchelon.GetComponent<EchelonBehavior>());
                break;
            case EchelonType.Paradeus:
                newEchelon = Instantiate(ParadeusTemplate, node.gameObject.transform.position, Quaternion.identity, mapImage.transform);
                regularEnemies.Add(newEchelon.GetComponent<EchelonBehavior>());
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
        else if (type == EchelonType.Deathstack)
        {
            StartCoroutine(InputEchelonName(behavior));
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

    private IEnumerator InputEchelonName(EchelonBehavior behavior)
    {
        InputFieldHolder.SetActive(true);
        if (newestType == EchelonType.Deathstack)
            InputFieldLabel.text = "Deathstack ID:";
        else
            InputFieldLabel.text = "Echelon Name:";

        submitted = false;
        while (!submitted)
        {
            yield return new WaitForEndOfFrame();
        }

        if (newestType == EchelonType.Deathstack)
            behavior.SetID(int.Parse(InputField.text));
        else
            behavior.SetName(InputField.text);

        InputFieldHolder.SetActive(false);
        InputField.text = "";
    }

    public void SelectEchelon(EchelonBehavior echelon)
    {
        if (!MoveEcheToNode(echelon.GetNode()))
        {
            DeselectEchelon();
            selectedEchelon = echelon;
            selectedEchelon.GetComponent<Image>().color = Color.yellow;
            SelectedEchelonDescription.text = "Echelon: " + echelon.GetID() + "\nFaction: " + echelon.GetFaction();
            if (selectedEchelon.GetName().Length > 0)
                SelectedEchelonDescription.text = SelectedEchelonDescription.text.Insert(0,"Name: " + selectedEchelon.GetName() + "\n");
            ParachuteBehavior para = echelon as ParachuteBehavior;
            if (para != null)
                SelectedEchelonDescription.text += "\nParachute CD: " + para.GetCooldown();
        }
    }

    public void DeselectEchelon()
    {
        if (selectedEchelon != null)
        {
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
                if (regularEnemies.Contains(echelon))
                {
                    regularEnemies.Remove(echelon);
                }
                else
                {
                    deathstacks.Remove(echelon);
                }
            }
            else
            {
                if (echelon as ParachuteBehavior != null)
                {
                    retreatedEchelons.Enqueue(echelon.GetID());
                    parachuteEchelons.Remove(echelon as ParachuteBehavior);
                    planner.RemoveEchelon();
                }
                else
                {
                    if (regularEchelons.Contains(echelon))
                    {
                        retreatedEchelons.Enqueue(echelon.GetID());
                        planner.RemoveEchelon();
                        regularEchelons.Remove(echelon);
                    }
                    else if (HOCs.Contains(echelon))
                    {
                        retreatedHOCs.Enqueue(echelon.GetID());
                        planner.RemoveHOC();
                        HOCs.Remove(echelon);
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
            foreach (EchelonBehavior echelon in regularEnemies)
            {
                if (echelon.GetFaction() == faction)
                {
                    echelon.CaptureNode();
                }
            }
            foreach (EchelonBehavior echelon in deathstacks)
            {
                if (echelon.GetFaction() == faction)
                {
                    echelon.CaptureNode();
                }
            }
        }
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
                        if ((selectedEchelon.GetFaction() == Faction.Blue && planner.GetAP() > 0) || NPCs.Contains(selectedEchelon) || selectedEchelon.GetFaction() != Faction.Blue)
                        {
                            RemoveEchelon(blockingEchelon);
                            selectedEchelon.SetNode(node);
                            selectedEchelon.gameObject.transform.position = node.gameObject.transform.position;

                            if (selectedEchelon.GetFaction() == Faction.Blue && !NPCs.Contains(selectedEchelon))
                                planner.AdjustAP(-1);

                            DeselectEchelon();
                            return true;
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
                    if (GetEchelonAtNode(node) == null)
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

    public void Submit()
    {
        if (newestType == EchelonType.Deathstack)
        {
            int x;
            if (int.TryParse(InputField.text, out x))
                submitted = true;
        }
        else
        {
            if (InputField.text.Length > 0)
                submitted = true;
        }
    }
}
