using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [SerializeField] private GameObject MainWindow;
    [SerializeField] private GameObject NewCardWindow;
    [SerializeField] private SaveLoadWindow LoadWindow;
    [SerializeField] private GameObject GameplayWindow;

    private BingoCard bingoCard;

    public void ToMainWindow()
    {
        NewCardWindow.SetActive(false);
        LoadWindow.gameObject.SetActive(false);
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
        LoadWindow.OpenWindow(savingMode: false);
    }

    public void OpenLoadBingoCardInSavingMode()
    {
        GameplayWindow.SetActive(false);
        LoadWindow.OpenWindow(savingMode: true);
    }

    public void OpenGameplayWindow()
    {
        NewCardWindow.SetActive(false);
        LoadWindow.gameObject.SetActive(false);
        MainWindow.SetActive(false);
        GameplayWindow.SetActive(true);
    }
}
