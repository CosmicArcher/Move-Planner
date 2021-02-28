using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EchelonBehavior : MonoBehaviour
{
    private NodeBehavior attachedNode;
    [SerializeField] private Faction faction;
    private int ID = 0;
    [SerializeField] private Text IDText;
    private string echelonName = "";

    public void SetName(string name)
    {
        echelonName = name;
    }

    public string GetName()
    {
        return echelonName;
    }

    public void SetNode(NodeBehavior node)
    {
        attachedNode = node;
    }

    public NodeBehavior GetNode()
    {
        return attachedNode;
    }

    public void SetFaction(Faction owner)
    {
        faction = owner;
    }

    public Faction GetFaction()
    {
        return faction;
    }

    public void SetID(int echelonID)
    {
        ID = echelonID;
        if (IDText != null)
            IDText.text = ID.ToString();
    }

    public int GetID()
    {
        return ID;
    }

    public void CaptureNode()
    {
        if (attachedNode.GetOwner() != faction)
        {
            attachedNode.SetOwner(faction);
        }
    }
}
