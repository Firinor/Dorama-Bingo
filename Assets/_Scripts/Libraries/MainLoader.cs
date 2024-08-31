using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MainLoader
{
    //DoramaDataGoogleDocs
    private static readonly string DoramaDataURL =
        "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=tsv&gid=1984545305";
    private static readonly string LanguagesURL =
        "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=tsv&gid=2002448881";
    private static readonly string PostersURL =
        "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=tsv&gid=1844463073";

    private static readonly string lineSplit = Environment.NewLine;
    private static readonly string columnSplit = "\t";

    private static readonly int minLimit = 16;

    public IEnumerator GetData(Action callback = null)
    {
        yield return EthernetManager.ConnectionEstablish(DoramaDataURL);
        yield return GetLocalData(callback);
    }



    public IEnumerator GetDoramaPosters(string data = null)
    {
        if(data != null)
        {
            yield return ParsePostersData(data);
            yield break;
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Get(PostersURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

            Debug.Log("Posters Downloading...");
            data = webRequest.downloadHandler.text;
            PlayerPrefs.SetString("_postersData", data);
            yield return ParsePostersData(data);
        }
    }

    public IEnumerator GetDoramaData(string data = null)
    {
        if(data != null)
        {
            yield return ParseDoramaData(data);
            yield break;
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Get(DoramaDataURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
            {
                yield break;
            }

            data = webRequest.downloadHandler.text;
            PlayerPrefs.SetString("_doramaData", data);
            yield return ParseDoramaData(data);
        }
    }

    public IEnumerator GetLanguagesData(string data = null)
    {
        if (data != null)
        {
            yield return ParseLanguageData(data);
            yield break;
        }
        using (UnityWebRequest webRequest = UnityWebRequest.Get(LanguagesURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

            data = webRequest.downloadHandler.text;
            PlayerPrefs.SetString("_languageData", data);
            yield return ParseLanguageData(data);
        }

    }

    public IEnumerator ParsePostersData(string data)
    {
        Debug.Log("PostersData processed");
        if (string.IsNullOrEmpty(data))
            yield break;

        string[] rowsData = data.Split(lineSplit);

        for (int row = 0; row < rowsData.Length; row++)
        {
            string[] cellData = rowsData[row].Split(columnSplit);
            
            if (!DataBase.Doramas.ContainsKey(cellData[0]))
                continue;


            if (DataBase.Doramas[cellData[0]].IsLoaded)
                continue;

            string TextureURL = "";

            for (int cell = 1; cell < cellData.Length; cell++)
            {
                if (cellData[cell].StartsWith("https:"))
                {
                    TextureURL = cellData[cell];
                    break;
                }
            }

            if (string.IsNullOrEmpty(TextureURL))
                continue;

            using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(TextureURL))
            {

                yield return www.SendWebRequest();

                if (!www.isDone)
                    yield break;

                Texture2D posterTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(
                    posterTexture, new Rect(0, 0, posterTexture.width, posterTexture.height), new Vector2(.5f, .5f));
                DataBase.Doramas[cellData[0]].Poster.sprite = sprite;
                DataBase.Doramas[cellData[0]].IsLoaded = true;
                yield return null;
            }
        }

        DataBase.PostersIsLoaded = true;
    }

    public IEnumerator ParseDoramaData(string data)
    {
        Debug.Log("DoramaData processed");
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

                switch (rowData[column].Trim())
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
        DataBase.DoramaIsReady = true;
    }

    public IEnumerator ParseLanguageData(string data)
    {
        Debug.Log("LanguageData processed");
        if (string.IsNullOrEmpty(data))
            yield break;

        string[] rowsData = data.Split(lineSplit);
        string[] LanguagesColumnData = rowsData[0].Split(columnSplit);

        for (int column = 1; column < LanguagesColumnData.Length; column++)
        {
            if (string.IsNullOrEmpty(LanguagesColumnData[column]))
                continue;

            LanguagesData languagesData = new();

            for (int rowIndex = 1; rowIndex < rowsData.Length; rowIndex++)
            {
                string[] languagesRow = rowsData[rowIndex].Split(columnSplit);

                if (languagesData.ContainsKey(languagesRow[0]))
                    continue;
                languagesData.Add(key: languagesRow[0], value: languagesRow[column]);
            }

            DataBase.Languages.Add(LanguagesColumnData[column].Trim(), languagesData);
        }

        LanguageTranslator.Initialization();

        DataBase.LanguagesIsReady = true;
    }

    public IEnumerator GetLocalData(Action callback = null)
    {
        yield return GetLanguagesDataLocal();
        yield return GetDoramaDataLocal();
        callback?.Invoke();
        yield return GetDoramaPostersLocal();
    }

    public IEnumerator GetDoramaPostersLocal()
    {
        if (!PlayerPrefs.HasKey("_postersData"))
            if (EthernetManager.ConnectionOn())
            {
                yield return GetDoramaPosters();
                yield break;
            }
            else
            {
                Debug.Log("Connection problem, _postersData not loaded");
            }
        yield return GetDoramaPosters(PlayerPrefs.GetString("_postersData"));
    }

    public IEnumerator GetDoramaDataLocal()
    {
        if (!PlayerPrefs.HasKey("_doramaData"))
            if (EthernetManager.ConnectionOn())
            {
                yield return GetDoramaData();
                yield break;
            }
            else
            {
                Debug.Log("Connection problem, _doramaData not loaded");
            }
        yield return GetDoramaData(PlayerPrefs.GetString("_doramaData"));
    }

    public IEnumerator GetLanguagesDataLocal()
    {
        if (!PlayerPrefs.HasKey("_languageData"))
            if (EthernetManager.ConnectionOn())
            {
                yield return GetLanguagesData();
                yield break;
            }
            else
            {
                Debug.Log("Connection problem, _languageData not loaded");
            }

        yield return GetLanguagesData(PlayerPrefs.GetString("_languageData"));
    }
}
