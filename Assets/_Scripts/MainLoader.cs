using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class MainLoader
{
    //DoramaDataGoogleDocs
    private string DoramaDataURL = "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=csv";
    private string LanguagesURL = "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=csv&gid=2002448881";

    private string lineSplit = Environment.NewLine;
    private string columnSplit = ",";

    private static int minLimit = 16;

    public IEnumerator GetData(Action callback = null)
    {
        yield return GetDoramaData();
        yield return GetLanguagesData();

        callback?.Invoke();
    }
    public IEnumerator GetDoramaData()
    {
        string data;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(DoramaDataURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

            data = webRequest.downloadHandler.text;
        }

        if (string.IsNullOrEmpty(data))
            yield break;

        string[] rowsData = data.Split(lineSplit);

        string TitleRow = rowsData[0];
        string[] TitleRowData = TitleRow.Split(columnSplit);

        for (int column = 1; column < TitleRowData.Length; column++)
        {
            DoramaData doramaData = new();

            for (int row = 1; row < rowsData.Length; row++)
            {
                string[] rowData = rowsData[row].Split(columnSplit);

                string tag = rowData[0];

                if (string.IsNullOrEmpty(tag))
                    continue;

                bool result;
                switch (rowsData[row].Split(columnSplit)[column])
                {
                    case "yes":
                        result = true;
                        break;
                    case "no":
                        result = false;
                        break;
                    default:
                        continue;
                }

                doramaData.Add(tag, result);
            }

            if (doramaData.Count < minLimit)
                continue;

            DataBase.Doramas.Add(TitleRowData[column], doramaData);
        }
    }

    public IEnumerator GetLanguagesData()
    {
        string data;

        using (UnityWebRequest webRequest = UnityWebRequest.Get(LanguagesURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

            data = webRequest.downloadHandler.text;
        }

        if (string.IsNullOrEmpty(data))
            yield break;

        string[] rowsData = data.Split(lineSplit);
        string[] LanguagesColumnData = rowsData[0].Split(columnSplit);

        for (int column = 0; column < LanguagesColumnData.Length; column++)
        {
            LanguagesData languagesData = new();

            for (int rowIndex = 1; rowIndex < rowsData.Length; rowIndex++)
            {
                string[] languagesRow = rowsData[rowIndex].Split(columnSplit);

                if (languagesData.ContainsKey(languagesRow[0]))
                    continue;
                languagesData.Add(key: languagesRow[0], value: languagesRow[column]);
            }

            DataBase.Languages.Add(LanguagesColumnData[column], languagesData);
        }

        LanguageTranslator.Initialization();
    }
}

public static class DataBase
{
    public static Dictionary<string, DoramaData> Doramas = new();
    public static Dictionary<string, LanguagesData> Languages = new();
}

public class LanguagesData : Dictionary<string, string>
{}

public class DoramaData : Dictionary<string, bool>
{}

