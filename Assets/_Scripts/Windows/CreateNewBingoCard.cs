using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Threading.Tasks;

public class CreateNewBingoCard : MonoBehaviour
{
    [SerializeField] private WindowManager gameManager;

    [SerializeField] private TMP_Dropdown cardSize;
    [SerializeField] private DoramaPreviewComponent doramaPreviewPrefab;
    [SerializeField] private RectTransform doramaPreviewParent;
    [SerializeField] private Button GoButton;

    private TMP_Dropdown.OptionData default4x4 = new("4x4");
    private TMP_Dropdown.OptionData default5x5 = new("5x5");
    private TMP_Dropdown.OptionData default6x6 = new("6x6");
    private string randomDorama = "Random k-drama";

    private string pickedDorama;

    private void Awake()
    {
        GoButton.onClick.AddListener(GenerateNewCard);
        Initialize();
    }

    public async void Initialize()
    {
        while (!DataBase.DoramaIsReady)
            await Task.Yield();

        foreach (string doramasKey in DataBase.Doramas.Keys)
        {
            string keyInstance = doramasKey;
            DoramaPreviewComponent newDoramaPreview = Instantiate(doramaPreviewPrefab, doramaPreviewParent);
            DataBase.Doramas[doramasKey].Poster = newDoramaPreview.Poster;
            newDoramaPreview.Text.text = doramasKey;
            newDoramaPreview.Button.onClick.AddListener(() => OnDoramaPick(keyInstance));
            newDoramaPreview.Text.gameObject.AddComponent<TranslatorTextElement>();
        }

        RandomDorama();
    }

    public void OnEnable()
    {
        if(!DataBase.PostersIsLoadet)
            StartCoroutine(MainLoader.GetPosters());
    }

    public void OnDoramaPick(string newPickedDorama)
    {
        if (newPickedDorama == randomDorama)
        {
            RandomDorama();
        }
        else
        {
            pickedDorama = newPickedDorama;
        }

        int keyCount = DataBase.Doramas[pickedDorama].Count;
        switch (keyCount)
        {
            case > 36:
                cardSize.options = new() { default4x4, default5x5, default6x6 };
                break;
            case > 25:
                cardSize.options = new() { default4x4, default5x5 };
                cardSize.value = Math.Min(cardSize.value, 1);
                break;
            case > 16:
                cardSize.options = new() { default4x4 };
                cardSize.value = 0;
                break;
            default:
                throw new Exception("Error when getting the number of tags of the drama!");
        }
    }

    private void RandomDorama()
    {
        int randomIndex = UnityEngine.Random.Range(0, DataBase.Doramas.Count);
        pickedDorama = DataBase.Doramas.ElementAt(randomIndex).Key;
    }

    private void GenerateNewCard()
    {
        BingoCard newCard = new();
        newCard.Dorama = pickedDorama;
        int size;
        switch (cardSize.value)
        {
            case 0:
                size = 4;
                break;
            case 1:
                size = 5;
                break;
            case 2:
                size = 6;
                break;
            default:
                throw new Exception("Unknown card size!");
        }
        newCard.Size = new Vector2Int(size, size);
        newCard.Cells = GenetareNewCells(newCard.Dorama, size);

        PlayerData.CurrentBingoCard = newCard;
        new SaveLoadManager().Save(newCard, SaveKey.CurrentCard);

        gameManager.OpenGameplayWindow();
    }
    private BingoCell[] GenetareNewCells(string dorama, int size)
    {
        int count = size * size;

        BingoCell[] result = new BingoCell[count];

        string[] tags = DataBase.Doramas[dorama].Keys.ToArray();

        GameMath.Shuffle(tags);

        for (int i = 0; i < count; i++)
        {
            result[i] = new() { Tag = tags[i], X = i % size, Y = i / size};
        }

        return result;
    }
    private void OnDestroy()
    {
        GoButton.onClick.RemoveAllListeners();
    }
}
