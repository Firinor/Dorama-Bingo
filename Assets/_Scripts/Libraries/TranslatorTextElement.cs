using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TranslatorTextElement : MonoBehaviour
{
    private TextMeshProUGUI text;
    private string key;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        key = text.text;
        LanguageTranslator.OnChangeLanguage += ChangeText;

        //if (DataBase.Languages.IsReady)
        //{
        //    string languageCode = Options.ApplicatonOptions.CurrentLanguageCode;
        //    ChangeText(LanguageTranslator.GetText(languageCode));
        //}
    }

    private void ChangeText(TMP_FontAsset font, LanguagesData newText)
    {
        text.font = font;
        text.text = newText[key];
    }
}
