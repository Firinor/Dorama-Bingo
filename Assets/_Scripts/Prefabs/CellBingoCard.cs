using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellBingoCard : MonoBehaviour
{
    public TextMeshProUGUI doramaTag;
    public Button button;

    public Image PressedImage;

    public BingoCell bingoCell;

    public void Initialize(BingoCell cell)
    {
        bingoCell = cell;

        if (doramaTag.TryGetComponent<TranslatorTextElement>(out var translator))
            Destroy(translator);
        doramaTag.text = cell.Tag;
        doramaTag.gameObject.AddComponent<TranslatorTextElement>();

        PressedImage.enabled = cell.IsPressed;

        gameObject.SetActive(true);
    }

    public void OnPressed()
    {
        bingoCell.IsPressed = !bingoCell.IsPressed;
        PressedImage.enabled = bingoCell.IsPressed;
    }
}
