using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayWindow : MonoBehaviour
{
    public TextMeshProUGUI DoramaName;
    [SerializeField]
    public BingoCard bingoCard;

    public GridLayoutGroup CellParent;
    public CellBingoCard CellPrefab;

    [SerializeField]
    private List<CellBingoCard> CellPool = new();
    [SerializeField]
    private GameObject WinLosePopup;
    [SerializeField]
    private Sprite Win;
    [SerializeField]
    private Sprite Lose;
    [SerializeField]
    private Sprite WinButton;
    [SerializeField]
    private Sprite RetryButton;

    [SerializeField]
    private Image[] HeartsImages;
    public TextMeshProUGUI ScoreText;
    [SerializeField]
    private Sprite FullHeartSprite;
    [SerializeField]
    private Sprite LoseHeartSprite;

    private bool _loadMode = false;

    private void Awake()
    {
        EventBus._loadBingoCardEvent.AddListener(() => _loadMode = true);
    }
    private void OnEnable()
    {
        CreateBingoCard();
    }

    public void SaveBingoCard() // When user press "ReturnButton", we saves the current card into PlayerData.CurrentBingoCard
    {
        PlayerData.CurrentBingoCard = bingoCard;
        new SaveLoadSystem().Save(PlayerData.CurrentBingoCard, SaveKey.CurrentCard);
    }

    private void ResetCurrentCard()
    {
        foreach (BingoCell cell in PlayerData.CurrentBingoCard.Cells)
        {
            cell.IsNeutral = true;
            cell.IsCorrect = false;
        }
        PlayerData.CurrentBingoCard.Hearts = 5;
        PlayerData.CurrentBingoCard.Scores = 0;
    }


    public void CreateBingoCard()
    {
        bool isNew = false;
        if ((bingoCard.Equals(PlayerData.CurrentBingoCard)) || _loadMode)
        {
            if (_loadMode)
            {
                bingoCard = PlayerData.CurrentBingoCard;
                _loadMode = false;
            }
            HeartLoad();
        }
        else
        {
            ResetCurrentCard();
            bingoCard = PlayerData.CurrentBingoCard;
            FullHeart();
            isNew = true;
        }

        LoadScores();

        if (TryGetComponent<TranslatorTextElement>(out var translator))
            Destroy(translator);

        DoramaName.text = bingoCard.Dorama;
        DoramaName.gameObject.AddComponent<TranslatorTextElement>();
        
        CellParent.constraintCount = bingoCard.Size.x;

        //enable bingo card cells
        for (int i = 0; i < bingoCard.Cells.Length; i++)
        {
            if(i < CellPool.Count)
            {
                BingoCell bingoCell = PlayerData.CurrentBingoCard.Cells[i];
                CellBingoCard cell = CellPool[i];
                cell.Initialize(bingoCell, PlayerPressedCell);
                if (isNew || bingoCell.IsNeutral) cell.NeutralPressed(); // If it's new card or in old card this was neutral
                if (!isNew && bingoCell.IsCorrect) cell.CorrectPressed(); // If not new and cell correct
                if (!isNew && !bingoCell.IsCorrect && !bingoCell.IsNeutral) cell.UncorrectPressed(); // If not new and cell not correct and neutral
            }
            else
            {
                CellBingoCard cell = Instantiate(CellPrefab, CellParent.transform);
                CellPool.Add(cell);
                cell.Initialize(bingoCard.Cells[i], PlayerPressedCell);
            }
        }
        //disable extra cells
        if (bingoCard.Cells.Length < CellPool.Count)
        {
            for (int i = PlayerData.CurrentBingoCard.Cells.Length; i < CellPool.Count; i++)
            {
                CellPool[i].gameObject.SetActive(false);
            }
        }
    }

    private void PlayerPressedCell(CellBingoCard cell)
    {
        if (!cell.bingoCell.IsNeutral)
            return;

        cell.bingoCell.IsNeutral = false;

        if (DataBase.Doramas[bingoCard.Dorama][cell.bingoCell.Tag])
        {
            cell.CorrectPressed();
            cell.bingoCell.IsCorrect = true;
            HeartAdd();
            AddScores();
        }
        else
        {
            cell.UncorrectPressed();
            cell.bingoCell.IsCorrect = false;
            HeartHit();
        }
    }

    private void HeartAdd()
    {
        ref int Hearts = ref PlayerData.CurrentBingoCard.Hearts;
        Hearts = Mathf.Min(5, Hearts + 1);
        HeartsImages[Hearts-1].sprite = FullHeartSprite;
    }

    private void AddScores()
    {
        PlayerData.CurrentBingoCard.Scores += PlayerData.CurrentBingoCard.Hearts * 100;
        ScoreText.text = $"$ {PlayerData.CurrentBingoCard.Scores}";
    }

    private void LoadScores()
    {
        ScoreText.text = $"$ {PlayerData.CurrentBingoCard.Scores}";
    }

    private void FullHeart()
    {
        foreach (Image heart in HeartsImages)
            heart.sprite = FullHeartSprite;

        PlayerData.CurrentBingoCard.Hearts = 5;
    }

    private void HeartLoad()
    {
        ref int Hearts = ref PlayerData.CurrentBingoCard.Hearts;

        // Fill all hearts
        foreach (Image heart in HeartsImages)
            heart.sprite = FullHeartSprite;

        // Hearts = 4; So we set LoseHeartSprite in range [4..HeartsImages.Length]
        for (int i = Hearts; i < HeartsImages.Length; i++)
        {
            HeartsImages[i].sprite = LoseHeartSprite;
        }
    }

    private void HeartHit()
    {
        ref int Hearts = ref PlayerData.CurrentBingoCard.Hearts;
        Hearts = Mathf.Max(0, Hearts - 1);
        HeartsImages[Hearts].sprite = LoseHeartSprite;
    }

    public void PlayerDone()
    {
        WinConditions conditions = new();

        //check
        bool IsPlayerWin = conditions.IsBINGO(PlayerData.CurrentBingoCard);

        Sprite popupSprite = IsPlayerWin ? Win : Lose;

        WinLosePopup.GetComponent<Image>().sprite = popupSprite;
        WinLosePopup.SetActive(true);
    }
}