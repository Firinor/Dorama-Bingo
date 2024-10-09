using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellBingoCard : MonoBehaviour
{
    public TextMeshProUGUI doramaTag;
    public Button button;

    public Image BackgroundImage;
    public Image PressedImage;

    public BingoCell bingoCell;

    public void Initialize(BingoCell cell, Action<CellBingoCard> action)
    {
        bingoCell = cell;

        if (doramaTag.TryGetComponent<TranslatorTextElement>(out var translator))
            Destroy(translator);

        doramaTag.text = cell.Tag;
        doramaTag.gameObject.AddComponent<TranslatorTextElement>();
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => action.Invoke(this));

        gameObject.SetActive(true);
    }

    public void NeutralPressed()
    {
        BackgroundImage.color = Color.white;
        PressedImage.enabled = false;
    }

    public void CorrectPressed()
    {
        BackgroundImage.color = Color.green;
        PressedImage.enabled = true;
    }

    public void UncorrectPressed()
    {
        BackgroundImage.color = Color.red;
        PressedImage.enabled = false;
    }
}
