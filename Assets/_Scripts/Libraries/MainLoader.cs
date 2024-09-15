using System;
using System.Collections;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

public class MainLoader
{
    public static readonly string lineSplit = Environment.NewLine;
    public static readonly string columnSplit = "\t";
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
        else
        {
            yield return EthernetManager.PostersRemoteDownload();
            yield return ParsePostersData(DataManager.ReadData("_postersData", temp: DataManager.IsExists("_postersData", temp: true)));
        }
    }

    public IEnumerator GetDoramaData(string data = null)
    {
        if(data != null)
        {
            yield return ParseDoramaData(data);
            yield break;
        }
        else
        {
            yield return EthernetManager.DoramaRemoteDownload();
            yield return ParseDoramaData(DataManager.ReadData("_doramaData", temp: DataManager.IsExists("_doramaData", temp: true)));
        }
    }

    public IEnumerator GetLanguagesData(string data = null)
    {
        if (data != null)
        {
            yield return ParseLanguageData(data);
            yield break;
        }
        else
        {
            yield return EthernetManager.LanguageRemoteDownload();
            yield return ParseLanguageData(DataManager.ReadData("_languageData", temp: DataManager.IsExists("_languageData", temp: true)));
        }
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


            string textureName = "";
            string textureURL = "";

            for (int cell = 1; cell < cellData.Length; cell++)
            {
                if (cellData[cell].StartsWith("https:/"))
                {
                    textureURL = cellData[cell];
                    textureName = cellData[0].Replace(": ", "");
                    break;
                }
            }

            if (textureName == "")
                continue;

            if (EthernetManager.ConnectionOn())
            {
                if (!DataManager.IsExists(cellData[0].Replace(": ", ""), temp: true, image: true) && !DataManager.IsExists(cellData[0].Replace(": ", ""), temp: false, image: true))
                {
                    yield return EthernetManager.PostersImageRemoteDownload(textureURL, (byte[] bytes) => { DataManager.SaveImage(textureName, bytes, temp: true); });
                }
            }
            Texture2D image = new(128, 128);
            image = DataManager.LoadImage(textureName, temp: DataManager.IsExists(cellData[0].Replace(": ", ""), temp: true, image: true));
            Debug.Log(textureName);
            Sprite sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector3(.5f, .5f));
            DataBase.Doramas[cellData[0]].Poster.sprite = sprite;
            DataBase.Doramas[cellData[0]].IsLoaded = true;
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
        yield return GetDoramaPostersLocal();
        callback?.Invoke();
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
        #endif
            yield break;
            }
        }
        else
        {
            yield return GetDoramaPosters(DataManager.ReadData("_postersData", temp: DataManager.IsExists("_postersData", temp: true)));
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
        #endif
            yield break;
            }
        } 
        else
        {
            yield return GetDoramaData(DataManager.ReadData("_doramaData", temp: DataManager.IsExists("_doramaData", temp: true)));
        }
    }

    public IEnumerator GetLanguagesDataLocal()
    {
        if (!DataManager.IsExists("_languageData", temp: false) || EthernetManager._isNeedToUpdate)
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
        #endif
            yield break;
            }
        } 
        else
        {
            yield return GetLanguagesData(DataManager.ReadData("_languageData", temp: DataManager.IsExists("_languageData", temp: true)));
        }
    }
}
