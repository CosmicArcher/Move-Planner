using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EnemyCreator : MonoBehaviour
{
    [SerializeField] private InputField enemyIDInput;
    [SerializeField] private Button yellowFaction;
    [SerializeField] private Button redFaction;

    [SerializeField] private GameObject spriteChoicesHolder;
    [SerializeField] private GameObject spriteChoiceTemplate;
    private List<GameObject> spriteChoices = new List<GameObject>();
    private int currentSelectedSprite = 0;

    [SerializeField] private Text sourcePathText;

    [SerializeField] EchelonsManager echelonManager;

    private Faction enemyFaction = Faction.Red;

    private void Awake()
    {
        sourcePathText.text = Application.dataPath + "/EnemySprites/";
        string[] files = Directory.GetFiles(Application.dataPath + "/EnemySprites/", "*.png");
        for (int i = 0; i < files.Length; i++)
        {
            GameObject newPreview = Instantiate(spriteChoiceTemplate, spriteChoicesHolder.transform);
            spriteChoices.Add(newPreview);
            int x = i;
            newPreview.GetComponent<Button>().onClick.AddListener(delegate { SelectSprite(x); });

            byte[] byteArray = File.ReadAllBytes(files[i]);
            Texture2D sampleTex = new Texture2D(128, 128, TextureFormat.RGBA32, false);
            bool isLoaded = sampleTex.LoadImage(byteArray);
            Sprite sprite = Sprite.Create(sampleTex, new Rect(new Vector2(0, 0), new Vector2(sampleTex.width, sampleTex.height)), new Vector2(0, 0));
            Image[] imageComponents = newPreview.GetComponentsInChildren<Image>();
            if (isLoaded)
                imageComponents[imageComponents.Length - 1].sprite = sprite;
        }
        spriteChoices[0].GetComponent<Image>().color = Color.yellow;
    }

    public void SelectSprite(int index)
    {
        spriteChoices[currentSelectedSprite].GetComponent<Image>().color = Color.white;
        spriteChoices[index].GetComponent<Image>().color = Color.yellow;
        currentSelectedSprite = index;
    }

    public Sprite GetChosenSprite()
    {
        Image[] imageComponents = spriteChoices[currentSelectedSprite].GetComponentsInChildren<Image>();
        return imageComponents[imageComponents.Length - 1].sprite;
    }

    public void SelectYellow()
    {
        yellowFaction.image.color = Color.gray;
        redFaction.image.color = Color.white;
        enemyFaction = Faction.Yellow;
    }

    public void SelectRed()
    {
        yellowFaction.image.color = Color.white;
        redFaction.image.color = Color.gray;
        enemyFaction = Faction.Red;
    }

    public Faction GetEnemyFaction()
    {
        return enemyFaction;
    }

    public string GetEnemyID()
    {
        return enemyIDInput.text;
    }

    public void CreateEnemy()
    {
        echelonManager.CreateNewEnemy();
    }

    public void ResetInputField()
    {
        enemyIDInput.text = "";
    }
}
