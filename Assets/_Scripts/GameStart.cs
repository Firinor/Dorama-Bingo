using System;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] private GameObject LanguageButtonPrefab;
    [SerializeField] private RectTransform LanguageButtonParent;

    [SerializeField] private CreateNewBingoCard CreateNewBingoCard;

    void Awake()
    {
        var loader = new MainLoader();

        StartCoroutine(loader.GetData(callback: InitializeSceneObjects));
    }

    private void InitializeSceneObjects()
    {
        InitializeLanguageFlags();
    }

    private void InitializeLanguageFlags()
    {
        int languageCount = DataBase.Languages.Count;
        Sprite[] flags = Resources.LoadAll<Sprite>("Languages/Languages");

        foreach (string languageCode in DataBase.Languages.Keys)
        {
            Sprite currentFlag = Array.Find(flags, flag => flag.name.ToLower() == languageCode.ToLower());

            GameObject languageButton = Instantiate(LanguageButtonPrefab, LanguageButtonParent);

            languageButton.GetComponent<Image>().sprite = currentFlag;
            languageButton.GetComponent<Button>().onClick.AddListener(
                () => LanguageTranslator.ChangeLanguage(languageCode));
        }
    }
}
