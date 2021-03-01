using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class MapInput : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private GameObject previewTemplate;
    [SerializeField] private GameObject previewParent;
    [SerializeField] private GameStateManager gameStateManager;

    [SerializeField] private GameObject nextStageHolder;
    [SerializeField] private GameObject nextStage;
    [SerializeField] private GameObject holderObject;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private NodeEditor nodeEditor;

    private string[] files;
    private List<GameObject> previews = new List<GameObject>();
    private int currentSelectedMap = 0;
    private void OnEnable()
    {
        RefreshList();
    }

    public void RefreshList()
    {
        ClearPreviews();

        files = Directory.GetFiles(Application.dataPath + "/MapImages/", "*.png");
        for (int i = 0; i < files.Length; i++)
        {
            GameObject newPreview = Instantiate(previewTemplate, previewParent.transform);
            previews.Add(newPreview);
            int x = i;
            newPreview.GetComponent<Button>().onClick.AddListener(delegate { SelectMap(x); });
        
            byte[] byteArray = File.ReadAllBytes(files[i]);
            Texture2D sampleTex = new Texture2D(1028, 1028, TextureFormat.RGBA32, false);
            bool isLoaded = sampleTex.LoadImage(byteArray);
            Sprite sprite = Sprite.Create(sampleTex, new Rect(new Vector2(0, 0), new Vector2(sampleTex.width, sampleTex.height)), new Vector2(0, 0));
            if (isLoaded)
                newPreview.GetComponent<Image>().sprite = sprite;
        }
    }

    public void SelectMap(int mapIndex)
    {
        previews[currentSelectedMap].GetComponent<Image>().color = Color.white;
        previews[mapIndex].GetComponent<Image>().color = Color.yellow;
        currentSelectedMap = mapIndex;
    }

    public void UseMap()
    {
        ClearPreviews();

        byte[] byteArray = File.ReadAllBytes(files[currentSelectedMap]);
        Texture2D sampleTex = new Texture2D(1028, 1028, TextureFormat.RGBA32, false);
        bool isLoaded = sampleTex.LoadImage(byteArray);
        if (isLoaded)
            image.texture = sampleTex;

        gameStateManager.gameState = GameStates.PlacingNodes;

        nodeEditor.SetMapPath(files[currentSelectedMap].Remove(0, (Application.dataPath + "/MapImages/").Length));
        nextStage.SetActive(true);
        nextStageHolder.SetActive(true);
        startPanel.SetActive(false);
    }

    public void ClearPreviews()
    {
        foreach (GameObject preview in previews)
            Destroy(preview);
        previews.Clear();
    }

    public void Back()
    {
        ClearPreviews();
        holderObject.SetActive(false);
        mainMenu.SetActive(true);
    }
}
