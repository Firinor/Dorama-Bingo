using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MainLoader
{
    private static readonly string lineSplit = Environment.NewLine;
    private static readonly string columnSplit = "\t";
    private static readonly int minLimit = 16;

    public IEnumerator GetData(Action callback = null)
    {
        yield return EthernetManager.ConnectionEstablish();
        yield return GetLocalData(callback);
    }

    public IEnumerator GetDoramaPosters(string data = null)
    {
        if(data != null)
        {
            yield return ParsePostersData(data);
            yield break;
        }
        yield return EthernetManager.PostersRemoteDownload();
        yield return ParsePostersData(DataManager.IsExists("_postersData", true) ? DataManager.ReadData("_postersData", true) : DataManager.ReadData("_postersData", false));

    }

    public IEnumerator GetDoramaData(string data = null)
    {
        if(data != null)
        {
            yield return ParseDoramaData(data);
            yield break;
        }
        yield return EthernetManager.DoramaRemoteDownload();
        yield return ParseDoramaData(DataManager.IsExists("_doramaData", true) ? DataManager.ReadData("_doramaData", true) : DataManager.ReadData("_doramaData", false));
    }

    public IEnumerator GetLanguagesData(string data = null)
    {
        if (data != null)
        {
            yield return ParseLanguageData(data);
            yield break;
        }
        yield return EthernetManager.LanguageRemoteDownload();
        yield return ParseLanguageData(DataManager.IsExists("_languageData", true) ? DataManager.ReadData("_languageData", true) : DataManager.ReadData("_languageData", false));
    }

    public IEnumerator ParsePostersData(string data)
    {
    #if UNITY_EDITOR
        Debug.Log("PostersData processed");
    #endif
        if (string.IsNullOrEmpty(data))
            yield break;

        string[] rowsData = data.Split(lineSplit);

        for (int row = 1; row < rowsData.Length; row++)
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
    #if UNITY_EDITOR
        Debug.Log("DoramaData processed");
    #endif
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
    #if UNITY_EDITOR
        Debug.Log("LanguageData processed");
    #endif
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
        if (!DataManager.IsExists("_postersData", false) || EthernetManager._isNeedToUpdate)
        {
            if (EthernetManager.ConnectionOn())
            {
                yield return GetDoramaPosters();
                yield break;
            }
            else
            {
        #if UNITY_EDITOR
                Debug.Log("Connection problem, _postersData not loaded");
                yield break;
        #endif
            }
        }
        else
        {
            yield return GetDoramaPosters(DataManager.IsExists("_postersData", true) ? DataManager.ReadData("_postersData", true) : DataManager.ReadData("_postersData", false));
        }
    }

    public IEnumerator GetDoramaDataLocal()
    {
        if (!DataManager.IsExists("_doramaData", false) || EthernetManager._isNeedToUpdate)
        {
            if (EthernetManager.ConnectionOn())
            {
                yield return GetDoramaData();
                yield break;
            }
            else
            {
        #if UNITY_EDITOR
                Debug.Log("Connection problem, _doramaData not loaded");
                yield break;
        #endif
            }
        } else
        {
            yield return GetDoramaData(DataManager.IsExists("_doramaData", true) ? DataManager.ReadData("_doramaData", true) : DataManager.ReadData("_doramaData", false));
        }
    }

    public IEnumerator GetLanguagesDataLocal()
    {
        if (!DataManager.IsExists("_languageData", false) || EthernetManager._isNeedToUpdate)
        {
            if (EthernetManager.ConnectionOn())
            {
                yield return GetLanguagesData();
                yield break;
            }
            else
            {
        #if UNITY_EDITOR
                Debug.Log("Connection problem, _languageData not loaded");
                yield break;
        #endif
            }
        } else
        {
            yield return GetLanguagesData(DataManager.IsExists("_languageData", true) ? DataManager.ReadData("_languageData", true) : DataManager.ReadData("_languageData", false));
        }
    }
}
