using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [SerializeField] private GameObject MainWindow;
    [SerializeField] private CanvasGroup NewCardWindow;
    [SerializeField] private SaveLoadWindow LoadWindow;
    [SerializeField] private GameObject GameplayWindow;

    private int i = 1;

    private BingoCard bingoCard;

    public void NewCardWindowVisible(bool hide)
    {
        if (hide)
        {
            NewCardWindow.alpha = 0f;
            NewCardWindow.blocksRaycasts = false;
            NewCardWindow.interactable = false;
        } else
        {
            NewCardWindow.alpha = 1f;
            NewCardWindow.blocksRaycasts = true;
            NewCardWindow.interactable = true;
        }
    }

    public void ToMainWindow()
    {
        NewCardWindowVisible(true);
        LoadWindow.gameObject.SetActive(false);
        GameplayWindow.SetActive(false);
        MainWindow.SetActive(true);
    }

    public void OpenNewBingoCard()
    {
        MainWindow.SetActive(false);
        NewCardWindowVisible(false);
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
        NewCardWindowVisible(true);
        LoadWindow.gameObject.SetActive(false);
        MainWindow.SetActive(false);
        GameplayWindow.SetActive(true);
    }
}
