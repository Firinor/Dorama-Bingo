using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadButtonPrefab : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI doramaName;
    [SerializeField] private Image bingoCardScreen;
    [SerializeField] private TextMeshProUGUI dateText;

    public Button Main;
    public Button DeleteButton;

    public void Initialize(LoadBingoCardData data = null)
    {
        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        doramaName.text = data.DoramaName;
        bingoCardScreen.sprite = GetSprite(data.ScreenPath);
        dateText.text = data.Date.ToString(format: "YY-MM-dd HH:G");
    }

    private Sprite GetSprite(string texturePath)
    {
        Texture2D texture = new SaveLoadSystem().LoadTexture(texturePath);

        Rect rect = new Rect(0,0, texture.width, texture.height);
        Sprite result = Sprite.Create(texture, rect, new Vector2(.5f, .5f));
        return result;
    }
}
