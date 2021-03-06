﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
    [SerializeField] private MapDataFileHandler dataLoader;
    [SerializeField] private RawImage image;
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private GameObject saveTemplate;
    [SerializeField] private GameObject saveParent;

    [SerializeField] private GameObject nextStageHolder;
    [SerializeField] private GameObject planner;
    [SerializeField] private GameObject editor;
    [SerializeField] private GameObject holderObject;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private GameStateManager gameStateManager;
    [SerializeField] private NodeEditor nodeEditor;

    private string[] files;
    private List<GameObject> saves = new List<GameObject>();
    private int currentSelectedSave = 0;

    private bool editingMap;

    private void OnEnable()
    {
        ShowFiles();
    }

    public void ShowFiles()
    {
        ClearPreviews();

        files = Directory.GetFiles(Application.dataPath + "/MapData/", "*.json");
        for (int i = 0; i < files.Length; i++)
        {
            GameObject newSave = Instantiate(saveTemplate, saveParent.transform);
            saves.Add(newSave);
            int x = i;
            newSave.GetComponent<Button>().onClick.AddListener(delegate {SelectSave(x);});
            string fileName = files[i].Remove(0, (Application.dataPath + "/MapData/").Length);
            newSave.GetComponentInChildren<Text>().text = fileName.Remove(fileName.Length - 5);
        }
    }

    public void LoadMap()
    {
        editingMap = false;
        StartCoroutine(LoadNodes());
    }

    public void EditMap()
    {
        editingMap = true;
        StartCoroutine(LoadNodes());
    }

    private IEnumerator LoadNodes()
    {
        MapDataFileHandler.MapData data = dataLoader.LoadFile(saves[currentSelectedSave].GetComponentInChildren<Text>().text + ".json");

        ClearPreviews();

        byte[] byteArray = File.ReadAllBytes(Application.dataPath + "/MapImages/" + data.MapAddress);
        Texture2D sampleTex = new Texture2D(1028, 1028, TextureFormat.RGBA32, false);
        bool isLoaded = sampleTex.LoadImage(byteArray);
        if (isLoaded)
            image.texture = sampleTex;

        if (!editingMap)
            planner.SetActive(true);
        else
        {
            nodeEditor.SetMapPath(data.MapAddress);
            gameStateManager.gameState = GameStates.PlacingNodes;
            editor.SetActive(true);
        }

        nextStageHolder.SetActive(true);
        yield return new WaitForEndOfFrame();
        startPanel.SetActive(false);

        List<NodeBehavior> nodeBehaviors = new List<NodeBehavior>();
        foreach (MapDataFileHandler.MapData.NodeData node in data.nodes)
        {
            Vector2 size = image.GetComponent<RectTransform>().rect.size;
            nodeBehaviors.Add(nodeManager.CreateNodeAtCoordinates(new Vector2(node.nodeLocationPerc.x * size.x, node.nodeLocationPerc.y * size.y),
                new Vector2(node.nodeSizePerc.x * size.x, node.nodeSizePerc.y * size.y), node.nodeFaction, node.nodeType, node.typeChangeList));
        }

        for (int i = 0; i < nodeBehaviors.Count; i++)
        {
            List<int> connectedNodes = data.nodes[i].connectedNodes;
            for (int j = 0; j < connectedNodes.Count; j++)
                nodeBehaviors[i].SetConnectedNode(nodeBehaviors[connectedNodes[j] - 1]);
        }
    }

    public void SelectSave(int saveIndex)
    {
        saves[currentSelectedSave].GetComponent<Image>().color = Color.white;
        saves[saveIndex].GetComponent<Image>().color = Color.yellow;
        currentSelectedSave = saveIndex;
    }

    public void ClearPreviews()
    {
        foreach (GameObject save in saves)
            Destroy(save);
        saves.Clear();
    }

    public void Back()
    {
        ClearPreviews();
        holderObject.SetActive(false);
        mainMenu.SetActive(true);
    }
}
