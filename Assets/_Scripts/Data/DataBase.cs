using System.Collections.Generic;
using UnityEngine.UI;

public static class DataBase
{
    public static Dictionary<string, DoramaData> Doramas = new();
    public static bool DoramaIsReady = false;
    public static bool PostersIsLoadet = false;
    public static Dictionary<string, LanguagesData> Languages = new();
    public static bool LanguagesIsReady = false;
}

public class LanguagesData : Dictionary<string, string>
{ }

public class DoramaData : Dictionary<string, bool>
{
    public Image Poster;
    public bool IsLoadet;
}