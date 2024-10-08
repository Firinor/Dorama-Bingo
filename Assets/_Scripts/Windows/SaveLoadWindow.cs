using System;
using UnityEngine;

public class SaveLoadWindow : MonoBehaviour
{
    private static string TexturePath;

    public bool IsSavingMode;

    [SerializeField] private LoadButtonPrefab[] loadButtons;
    [SerializeField] private SaveScreenManager saveLoadManager;
    [SerializeField] private WindowManager windowManager;
    [SerializeField] private RectTransform objectToScreen;

    private LoadBingoCardData[] playerSaves;

    private void Awake()
    {
        TexturePath = Application.persistentDataPath;
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
            //PlayerData.CurrentBingoCard = playerSaves[index].BingoCard;
            PlayerData.CurrentBingoCard = new BingoCard(playerSaves[index].BingoCard);
            foreach(var bingoCell in playerSaves[index].BingoCard.Cells)
            {
                Debug.Log(bingoCell.IsNeutral);
            }
            windowManager.OpenGameplayWindow();
        }
    }

    private void SaveBingoCard(int index)
    {
        BingoCard currentCard = new BingoCard(PlayerData.CurrentBingoCard);
        LoadBingoCardData newSavedCard = new();
        newSavedCard.BingoCard = currentCard;
        newSavedCard.DoramaName = currentCard.Dorama;
        newSavedCard.Date = DateTime.Now.Ticks;
        newSavedCard.ScreenPath = TexturePath + $"/savescrn{index}.jpg";
        playerSaves.SetValue(newSavedCard, index);
        new SaveLoadSystem().Save(playerSaves, SaveKey.SavedCards);
        saveLoadManager.SaveCurrentCardScreen(newSavedCard.ScreenPath, objectToScreen, () => { UpdateBingoCard(index);});

        PlayerData.CurrentBingoCard = new BingoCard(currentCard);
        new SaveLoadSystem().Save(PlayerData.CurrentBingoCard, SaveKey.CurrentCard);
        windowManager.OpenGameplayWindow();
    }

    public void OnDeletePressed(int index)
    {
        playerSaves[index] = null;
        new SaveLoadSystem().DeleteTexture(TexturePath + $"/savescrn{index}.jpg");
        new SaveLoadSystem().Save(playerSaves, SaveKey.SavedCards);
        UpdateBingoCard(index);
    }
}