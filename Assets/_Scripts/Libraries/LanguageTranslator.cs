using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class LanguageTranslator
{
    public static Action<TMP_FontAsset, LanguagesData> OnChangeLanguage;

    public static Dictionary<string, TMP_FontAsset> fonts;

    public static void Initialization()
    {
        fonts = new();
        foreach (string data in DataBase.Languages.Keys)
        {
            TMP_FontAsset fontAsset = Resources.Load("Languages/" + data) as TMP_FontAsset;
            fonts.Add(data, fontAsset);
        }
    }

    public static void ChangeLanguage(string LanguageCode)
    {
        OnChangeLanguage?.Invoke(fonts[LanguageCode], DataBase.Languages[LanguageCode]);
        new SaveLoadManager().Save(LanguageCode, SaveKey.Language);
    }

    public static TranslateData GetText(string LanguageCode)
    {
        return new()
        {
            Font = fonts[LanguageCode],
            LanguagesData = DataBase.Languages[LanguageCode],
        };
    }
}

public class TranslateData
{
    public TMP_FontAsset Font;
    public LanguagesData LanguagesData;
}