using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject ContinueButton;
    [SerializeField] private GameObject LoadButton;

    [SerializeField] private GameObject MainWindow;
    [SerializeField] private GameObject NewCardWindow;
    [SerializeField] private GameObject LoadWindow;
    [SerializeField] private GameObject GameplayWindow;

    private BingoCard bingoCard;

    private void Awake()
    {
        bool isLast = PlayerPrefs.HasKey(SaveKey.LastBingoCard);
        ContinueButton.SetActive(isLast);
        bool isCartsList = PlayerPrefs.HasKey(SaveKey.BingoCardsList);
        LoadButton.SetActive(isCartsList);
    }

    public void ToMainWindow()
    {
        NewCardWindow.SetActive(false);
        LoadWindow.SetActive(false);
        GameplayWindow.SetActive(false);
        MainWindow.SetActive(true);
    }

    public void OpenNewBingoCard()
    {
        MainWindow.SetActive(false);
        NewCardWindow.SetActive(true);
    }

    public void OpenLoadBingoCard()
    {
        MainWindow.SetActive(false);
        LoadWindow.SetActive(true);
    }

    public void OpenGameplayWindow()
    {
        OpenGameplayWindow(null);
    }

    public void OpenGameplayWindow(BingoCard card = null)
    {
        if (card != null)
            InitializeBingoCard(card);

        if (bingoCard != null)
            return;
        else
            InitializeBingoCard(SaveLoadSystem.LoadData<BingoCard>(SaveKey.LastBingoCard));

        NewCardWindow.SetActive(false);
        LoadWindow.SetActive(false);
        MainWindow.SetActive(false);
        GameplayWindow.SetActive(true);
    }

    private void InitializeBingoCard(BingoCard card)
    {
        GameObject cellPrefab = Resources.Load<GameObject>("Cell");

        throw new NotImplementedException();
    }
}

[Serializable]
public class BingoCard
{
    internal string Dorama;
    internal Vector2 Size;
    internal BingoCell[] Cells;
}

public static class SaveLoadSystem
{
    public static T LoadData<T>(string key)
    {
        string loadData = PlayerPrefs.GetString(key);
        T result = JsonUtility.FromJson<T>(loadData);
        return result;
    }

    public static void SaveData<T>(string key, T data)
    {
        string saveData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, saveData);
    }
}

public static class SaveKey
{
    public static readonly string LastBingoCard = "LastBingoCard";
    public static readonly string BingoCardsList = "BingoCardsList";
}