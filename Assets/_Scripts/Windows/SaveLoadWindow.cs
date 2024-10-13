using System;
using UnityEngine;

public class SaveLoadWindow : MonoBehaviour
{
    private static string TexturePath => Application.persistentDataPath; 

    public bool IsSavingMode;

    [SerializeField] private LoadButtonPrefab[] loadButtons;
    [SerializeField] private SaveScreenManager saveLoadManager;
    [SerializeField] private WindowManager windowManager;
    [SerializeField] private RectTransform objectToScreen;

    private LoadBingoCardData[] playerSaves;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        int i = 0;

        if (PlayerData.SavedBingoCards == null)
        {
            playerSaves = new LoadBingoCardData[8] { null, null, null, null, null, null, null, null };
            PlayerData.SavedBingoCards = playerSaves;
        }
        else
        {
            playerSaves = PlayerData.SavedBingoCards;
        }

        foreach (LoadButtonPrefab button in loadButtons)
        {
            int index = i;
            ButtonInitialize(button, index);
            i++;
        }
    }

    private void ButtonInitialize(LoadButtonPrefab loadButtonPrefab, int index)
    {
        loadButtonPrefab.Main.onClick.RemoveAllListeners();
        loadButtonPrefab.DeleteButton.onClick.RemoveAllListeners();
        loadButtonPrefab.Initialize(playerSaves[index]);
        loadButtonPrefab.Main.onClick.AddListener(() => OnButtonPressed(index));
        loadButtonPrefab.DeleteButton.onClick.AddListener(() => OnDeletePressed(index));
    }

    private void UpdateBingoCard(int index) 
    {
        ButtonInitialize(loadButtons[index], index);
    }

    public void OpenWindow(bool savingMode)
    {
        IsSavingMode = savingMode;
        gameObject.SetActive(true);
    }

    public void OnButtonPressed(int index)
    {
        if (IsSavingMode)
            SaveBingoCard(index);
        else
            LoadBingoCard(index);
    }

    private void LoadBingoCard(int index)
    {
        if (!String.IsNullOrEmpty(playerSaves[index]?.DoramaName))
        {
            /*
                We need it, because in Initializion method, our PlayerData.SavedBingoCards ref to playerSaves, but playerSaves stored in heap,
                so we need to reload PlayerData.SavedBingoCard from our PC, otherwise here's gonna be bug
             */
            var loadedCards = new SaveLoadSystem().LoadArray<LoadBingoCardData>(SaveKey.SavedCards); 

            if (loadedCards != null)
                PlayerData.SavedBingoCards = loadedCards;

            PlayerData.CurrentBingoCard = new BingoCard(PlayerData.SavedBingoCards[index].BingoCard);

            EventBus._loadBingoCardEvent?.Invoke();
            windowManager.OpenGameplayWindow();
        }
    }

    private void SaveBingoCard(int index)
    {
        BingoCard currentCard = new BingoCard(PlayerData.CurrentBingoCard);
        LoadBingoCardData newSavedCard = new()
        {
            BingoCard = currentCard,
            DoramaName = currentCard.Dorama,
            Date = DateTime.Now.Ticks,
            ScreenPath = TexturePath + $"/savescrn{index}.jpg"
        };
        playerSaves[index] = newSavedCard;
        new SaveLoadSystem().Save(playerSaves, SaveKey.SavedCards);
        saveLoadManager.SaveCurrentCardScreen(newSavedCard.ScreenPath, objectToScreen, () => { UpdateBingoCard(index);});

        windowManager.OpenGameplayWindow();
    }

    private void OnDeletePressed(int index)
    {
        if (playerSaves[index] == null && index < playerSaves.Length)
            return;

        playerSaves[index] = null;
        new SaveLoadSystem().DeleteTexture(TexturePath + $"/savescrn{index}.jpg");
        new SaveLoadSystem().Save(playerSaves, SaveKey.SavedCards);
        UpdateBingoCard(index);
    }
}