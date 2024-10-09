using System;
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
        StartCoroutine(loader.GetData(callback: InitializeSceneObjects, updateLoader: new UpdateLoader()));
    }

    private void LoadAll()
    {
        SaveLoadSystem saveManager = new();
        PlayerData.CurrentBingoCard = saveManager.Load<BingoCard>(SaveKey.CurrentCard);
        ApplicationOptions.LanguageCode = saveManager.Load(SaveKey.Language);
        PlayerData.SavedBingoCards = saveManager.LoadArray<LoadBingoCardData>(SaveKey.SavedCards);
    }

    private void InitializeSceneObjects()
    {
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

    public void DataReload()
    {
        PlayerPrefs.DeleteAll();
        SceneReload();
    }

    public void SceneReload()
    {
        DataBase.Doramas = new();
        DataBase.DoramaIsReady = false;
        DataBase.PostersIsLoaded = false;
        DataBase.Languages = new();
        DataBase.LanguagesIsReady = false;
        SceneManager.LoadScene(0);
    }
}