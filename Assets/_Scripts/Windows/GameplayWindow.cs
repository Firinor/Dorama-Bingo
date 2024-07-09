using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayWindow : MonoBehaviour
{
    public TextMeshProUGUI DoramaName;

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


    private void OnEnable()
    {
        CreateBingoCard();
    }

    public void CreateBingoCard()
    {
        BingoCard bingoCard = PlayerData.CurrentBingoCard;

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
                cell.Initialize(bingoCard.Cells[i]);
            }
            else
            {
                CellBingoCard cell = Instantiate(CellPrefab, CellParent.transform);
                CellPool.Add(cell);
                cell.Initialize(bingoCard.Cells[i]);
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

    public void PlayerDone()
    {
        //check
        bool IsPlayerWin = false;

        Sprite popupSprite = IsPlayerWin ? Win : Lose;

        WinLosePopup.GetComponent<Image>().sprite = popupSprite;
        WinLosePopup.SetActive(true);
    }
}
