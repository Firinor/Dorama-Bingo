using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TranslatorTextElement : MonoBehaviour
{
    private TextMeshProUGUI text;
    private string key;
    
    private IEnumerator Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        key = text.text;
        LanguageTranslator.OnChangeLanguage += ChangeText;

        while (!DataBase.LanguagesIsReady)
            yield return null;

        if (string.IsNullOrEmpty(ApplicationOptions.LanguageCode))
            yield break;

        string languageCode = ApplicationOptions.LanguageCode;
        ChangeText(LanguageTranslator.GetText(languageCode));
    }

    private void ChangeText(TranslateData data)
    {
        ChangeText(data.Font, data.LanguagesData);
    }
    private void ChangeText(TMP_FontAsset font, LanguagesData newText)
    {
        if (!newText.ContainsKey(key))
            return;
        
        text.font = font;
        text.text = newText[key];
    }
}
