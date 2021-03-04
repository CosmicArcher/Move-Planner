using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EchelonBehavior : MonoBehaviour
{
    protected NodeBehavior attachedNode;
    [SerializeField] private Faction faction;
    private int ID = 0;
    [SerializeField] private Text IDText;
    private string echelonName = "";

    private Vector2Int supply = new Vector2Int(5, 10);

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

    public void Resupply()
    {
        supply = new Vector2Int(5, 10);
    }

    public void UseSupply(Vector2Int loss)
    {
        supply -= loss;
        if (supply.x < 0)
            supply.x = 0;
        if (supply.y < 0)
            supply.y = 0;
    }

    public Vector2Int GetSupply()
    {
        return supply;
    }

    public void CaptureNode()
    {
        if (attachedNode.GetOwner() != faction)
        {
            attachedNode.SetOwner(faction);
        }
    }
}
