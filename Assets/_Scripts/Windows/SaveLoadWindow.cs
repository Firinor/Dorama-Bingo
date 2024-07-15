using System;
using UnityEngine;

public class SaveLoadWindow : MonoBehaviour
{
    public bool IsSavingMode;

    [SerializeField] private LoadButtonPrefab[] loadButtons;

    private void Awake()
    {
        int i = 0;
        LoadBingoCardData[] playerSaves = PlayerData.SavedBingoCards;

        if (playerSaves == null)
            playerSaves = new LoadBingoCardData[8] { null, null, null, null, null, null, null, null };

        foreach (LoadButtonPrefab button in loadButtons)
        {
            int index = i;//new instance
            button.Initialize(playerSaves[i]);
            button.Main.onClick.AddListener(() => OnButtonPressed(index));
            button.DeleteButton.onClick.AddListener(() => OnDeletePressed(index));
            i++;
        }
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
        throw new NotImplementedException();
    }

    private void SaveBingoCard(int index)
    {
        BingoCard currentCard = PlayerData.CurrentBingoCard;
        LoadBingoCardData savedCard = PlayerData.SavedBingoCards[index];

        LoadBingoCardData newSavedCard = new();

        newSavedCard.BingoCard = currentCard;
        newSavedCard.DoramaName = currentCard.Dorama;
        newSavedCard.Date = DateTime.Now;
        newSavedCard.ScreenPath = Application.persistentDataPath + $"/savescrn{index}.jpg";
        SaveCurrentCardScreen(newSavedCard.ScreenPath);

        new SaveLoadSystem().Save(PlayerData.SavedBingoCards, SaveKey.SavedCards);
    }

    private void SaveCurrentCardScreen(string screenPath)
    {
        Texture2D texture = new Texture2D(Screen.width, Screen.height);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        new SaveLoadSystem().SaveTexture(texture, screenPath);
    }

    public void OnDeletePressed(int index)
    {
        //playerSaves[index]
    }
}
