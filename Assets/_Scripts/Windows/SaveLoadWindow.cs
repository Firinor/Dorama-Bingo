using System;
using UnityEngine;

public class SaveLoadWindow : MonoBehaviour
{
    private static string TexturePath;

    public bool IsSavingMode;

    [SerializeField] private LoadButtonPrefab[] loadButtons;
    [SerializeField] private SaveLoadManager saveLoadManager;
    [SerializeField] private WindowManager windowManager;
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
            button.Initialize(playerSaves[i]);
            button.Main.onClick.AddListener(() => OnButtonPressed(index));
            button.DeleteButton.onClick.AddListener(() => OnDeletePressed(index));
            i++;
        }
    }

    private void UpdateBingoCard(int index) 
    {
        loadButtons[index].Initialize(playerSaves[index]);
        loadButtons[index].Main.onClick.AddListener(() => OnButtonPressed(index));
        loadButtons[index].DeleteButton.onClick.AddListener(() => OnDeletePressed(index));
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
            PlayerData.CurrentBingoCard = playerSaves[index].BingoCard;
            windowManager.LoadGameplayWindow(PlayerData.CurrentBingoCard);
        }
    }

    private void SaveBingoCard(int index)
    {
        BingoCard currentCard = PlayerData.CurrentBingoCard;
        LoadBingoCardData newSavedCard = new();
        newSavedCard.BingoCard = currentCard;
        newSavedCard.DoramaName = currentCard.Dorama;
        newSavedCard.Date = DateTime.Now;
        newSavedCard.ScreenPath = TexturePath + $"/savescrn{index}.jpg";
        playerSaves.SetValue(newSavedCard, index);
        new SaveLoadSystem().Save(playerSaves, SaveKey.SavedCards);
        saveLoadManager.SaveCurrentCardScreen(newSavedCard.ScreenPath, () => { UpdateBingoCard(index);});

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