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
    }
}
