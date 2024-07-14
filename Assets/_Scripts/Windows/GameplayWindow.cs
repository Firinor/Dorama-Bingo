using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayWindow : MonoBehaviour
{
    public TextMeshProUGUI DoramaName;
    private BingoCard bingoCard;

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


    private void OnEnable()
    {
        CreateBingoCard();
    }

    public void CreateBingoCard()
    {
        bool isNew = false;

        if (bingoCard != PlayerData.CurrentBingoCard)
        {
            bingoCard = PlayerData.CurrentBingoCard;
            FullHeart();
            isNew = true;
        }

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
                CellBingoCard cell = CellPool[i];
                cell.Initialize(bingoCard.Cells[i], PlayerPressedCell);
                if (isNew) cell.NeutralPressed();
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
            for (int i = bingoCard.Cells.Length; i < CellPool.Count; i++)
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

        if (DataBase.Doramas[PlayerData.CurrentBingoCard.Dorama][cell.bingoCell.Tag])
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

    private void FullHeart()
    {
        foreach (Image heart in HeartsImages)
            heart.sprite = FullHeartSprite;

        PlayerData.CurrentBingoCard.Hearts = 5;
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
