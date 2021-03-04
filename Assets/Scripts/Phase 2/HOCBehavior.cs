using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HOCBehavior : EchelonBehavior
{
    private int range = 2;
    public bool IsNodeInRange(NodeBehavior node)
    {
        List<NodeBehavior> nodes = new List<NodeBehavior>();
        nodes.Add(attachedNode);
        return CheckAdjacentNodes(nodes, range, attachedNode, node);
    }

    private bool CheckAdjacentNodes(List<NodeBehavior> checkedNodes, int distance, NodeBehavior baseNode, NodeBehavior nodeToFind)
    {
        List<NodeBehavior> newNodes = new List<NodeBehavior>();
        List<NodeBehavior> nodesToCheck = baseNode.GetConnectedNodes();

        if (nodesToCheck.Contains(nodeToFind))
            return true;
        else if (distance > 1)
        {
            foreach (NodeBehavior node in nodesToCheck)
            {
                if (!checkedNodes.Contains(node))
                {
                    newNodes.Add(node);
                    checkedNodes.Add(node);
                }
            }
        }
        else
            return false;

        while (distance > 1)
        {
            List<NodeBehavior> nextNodeLayer = new List<NodeBehavior>();
            foreach (NodeBehavior node in newNodes)
            {
                if (node.GetConnectedNodes().Contains(nodeToFind))
                    return true;

                foreach (NodeBehavior adjacent in node.GetConnectedNodes())
                {
                    if (!checkedNodes.Contains(adjacent))
                    {
                        nextNodeLayer.Add(adjacent);
                        checkedNodes.Add(adjacent);
                    }
                }
            }

            newNodes = nextNodeLayer;
            distance--;
        }
        return false;
    }

    public void SetRange(int HOCRange)
    {
        range = HOCRange;
    } 

    public int GetRange()
    {
        return range;
    }

    public List<NodeBehavior> GetAllNodesInRange()
    {
        List<NodeBehavior> nodes = new List<NodeBehavior>();
        nodes.Add(attachedNode);
        GetNodesInRange(nodes, range, attachedNode);
        return nodes;
    }

    private void GetNodesInRange(List<NodeBehavior> checkedNodes, int distance, NodeBehavior baseNode)
    {
        List<NodeBehavior> newNodes = new List<NodeBehavior>();
        foreach (NodeBehavior node in baseNode.GetConnectedNodes())
        {
            if (!checkedNodes.Contains(node))
            {
                newNodes.Add(node);
                checkedNodes.Add(node);
            }
        }

        while (distance > 1)
        {
            List<NodeBehavior> nextNodeLayer = new List<NodeBehavior>();
            foreach (NodeBehavior node in newNodes)
            {
                foreach(NodeBehavior adjacent in node.GetConnectedNodes())
                {
                    if (!checkedNodes.Contains(adjacent))
                    {
                        nextNodeLayer.Add(adjacent);
                        checkedNodes.Add(adjacent);
                    }
                }
            }

            newNodes = nextNodeLayer;
            distance--;
        }
    }
}
