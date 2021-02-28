using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject loadMapHolder;
    [SerializeField] private GameObject newMapHolder;
    [SerializeField] private Text loadMapCaption;
    [SerializeField] private Text newMapCaption;

    private void Awake()
    {
        if (!Directory.Exists(Application.dataPath + "/MapImages/"))
            Directory.CreateDirectory(Application.dataPath + "/MapImages/");
        if (!Directory.Exists(Application.dataPath + "/MapData/"))
            Directory.CreateDirectory(Application.dataPath + "/MapData/");

        loadMapCaption.text = "Source: " + Application.dataPath + "/MapData/";
        newMapCaption.text = "Source: " + Application.dataPath + "/MapImages/";
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void LoadMap()
    {
        mainMenu.SetActive(false);
        loadMapHolder.SetActive(true);
    }

    public void NewMap()
    {
        mainMenu.SetActive(false);
        newMapHolder.SetActive(true);
    }
}
