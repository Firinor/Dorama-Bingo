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
