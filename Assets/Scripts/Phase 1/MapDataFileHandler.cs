using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MapDataFileHandler : MonoBehaviour
{
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private MapInput mapChooser;
    [SerializeField] private GameObject image;
    
    public class MapData
    {
        [Serializable]
        public struct NodeData
        {
            public int NodeID;
            public Vector2 nodeLocationPerc;
            public Vector2 nodeSizePerc;
            public Faction nodeFaction;
            public List<int> connectedNodes;
            public NodeType nodeType;
            public List<Vector2> typeChangeList;
        }

        public string MapAddress;
        public List<NodeData> nodes = new List<NodeData>();

        public void AddNode(NodeBehavior node, List<NodeBehavior> nodeList, Vector2 baseSize)
        {
            NodeData data = new NodeData();
            data.NodeID = nodes.Count + 1;
            RectTransform rectTransform = node.GetComponent<RectTransform>();
            data.nodeLocationPerc = new Vector2(rectTransform.anchoredPosition.x / baseSize.x, rectTransform.anchoredPosition.y / baseSize.y);
            data.nodeSizePerc = new Vector2(rectTransform.sizeDelta.x / baseSize.x, rectTransform.sizeDelta.y / baseSize.y);
            data.nodeFaction = node.GetOwner();
            data.nodeType = node.GetNodeType();

            data.typeChangeList = new List<Vector2>();
            foreach (int key in node.GetTypeChangeList().Keys)
            {
                data.typeChangeList.Add(new Vector2(key, (int)node.GetTypeChangeList()[key]));
            }

            data.connectedNodes = new List<int>();
            for (int i = 0; i < node.GetConnectedNodes().Count; i++)
                data.connectedNodes.Add(nodeList.IndexOf(node.GetConnectedNodes()[i]) + 1);

            nodes.Add(data);
        }

        public void AddNodes(List<NodeBehavior> nodeList, Vector2 baseSize)
        {
            for (int i = 0; i < nodeList.Count; i++)
                AddNode(nodeList[i], nodeList, baseSize);
        }
    }

    public void SaveFile(string name, string imagePath)
    {
        List<GameObject> nodeList = nodeManager.GetNodeList();
        List<NodeBehavior> nodeBehaviors = new List<NodeBehavior>();
        for (int i = 0; i < nodeList.Count; i++)
            nodeBehaviors.Add(nodeList[i].GetComponent<NodeBehavior>());

        MapData mapData = new MapData();
        Vector2 baseSize = image.GetComponent<RectTransform>().rect.size;
        mapData.AddNodes(nodeBehaviors, baseSize);
        mapData.MapAddress = imagePath;

        string json = JsonUtility.ToJson(mapData, true);
        File.WriteAllText(Application.dataPath + "/MapData/" + name + ".json", json);
    }

    public MapData LoadFile(string fileName)
    {
        string data = File.ReadAllText(Application.dataPath + "/MapData/" + fileName);
        return JsonUtility.FromJson<MapData>(data);
    }
}
