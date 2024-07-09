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
        //InitializeBingoCard(SaveLoadSystem.LoadData<BingoCard>(SaveKey.LastBingoCard));
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
        NewCardWindow.SetActive(false);
        LoadWindow.SetActive(false);
        MainWindow.SetActive(false);
        GameplayWindow.SetActive(true);
    }
}
