using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] private GameObject LanguageButtonPrefab;
    [SerializeField] private RectTransform LanguageButtonParent;

    [SerializeField] private CreateNewBingoCard CreateNewBingoCard;

    void Awake()
    {
        LoadAll();

        MainLoader loader = new();


        StartCoroutine(loader.GetData(callback: InitializeSceneObjects));

    }

    private void LoadAll()
    {
        SaveLoadSystem saveManager = new();
        PlayerData.CurrentBingoCard = saveManager.Load<BingoCard>(SaveKey.CurrentCard);
        ApplicationOptions.LanguageCode = saveManager.Load(SaveKey.Language);
        PlayerData.SavedBingoCards = saveManager.Load<LoadBingoCardData[]>(SaveKey.SavedCards);
    }

    private void InitializeSceneObjects()
    {
        UpdateLoader updateLoader = new();
        StartCoroutine(updateLoader.GetUpdate(UpdateReload));
        InitializeLanguageFlags();
    }

    private void InitializeLanguageFlags()
    {
        Sprite[] flags = Resources.LoadAll<Sprite>("Languages/Languages");

        foreach (string languageCode in DataBase.Languages.Keys)
        {
            Sprite currentFlag = Array.Find(flags, flag => flag.name.StartsWith(languageCode));

            if (currentFlag == null)
            {
                Debug.LogError("No flag " + languageCode);
                continue;
            }

            GameObject languageButton = Instantiate(LanguageButtonPrefab, LanguageButtonParent);

            languageButton.GetComponent<Image>().sprite = currentFlag;
            languageButton.GetComponent<Button>().onClick.AddListener(
                () => LanguageTranslator.ChangeLanguage(languageCode));
        }
    }

    public void Reload()
    {
        PlayerPrefs.DeleteAll();
        DataBase.Doramas = new();
        DataBase.DoramaIsReady = false;
        DataBase.PostersIsLoaded = false;
        DataBase.Languages = new();
        DataBase.LanguagesIsReady = false;
        SceneManager.LoadScene(0);
    }

    public void UpdateReload()
    {
        DataBase.Doramas = new();
        DataBase.DoramaIsReady = false;
        DataBase.PostersIsLoaded = false;
        DataBase.Languages = new();
        DataBase.LanguagesIsReady = false;
        SceneManager.LoadScene(0);
    }
}

