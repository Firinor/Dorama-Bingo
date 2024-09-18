using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public class MainLoader
{
    public static readonly string lineSplit = Environment.NewLine;
    public static readonly string columnSplit = "\t";
    private static readonly int minLimit = 16;

    /// <summary>
    /// There's 3 steps what it does. 
    /// 1. Check internet connection. 
    /// 2. Check updates. 
    /// 3. Check is local data exist, if exists parse it, else download & parse
    /// </summary>
    /// <param name="callback">Any method that you want to run after all steps</param>
    /// <param name="updateLoader">UpdateLoader module for second step (not necessary)</param>
    public IEnumerator GetData(Action callback = null, UpdateLoader updateLoader = null)
    {
        yield return EthernetManager.ConnectionEstablish();

        if (updateLoader != null)
            yield return updateLoader.GetUpdate();

        yield return GetLocalData(callback);
    }

    /// <summary>
    /// Check if postersData exists on device
    /// </summary>
    /// <param name="data">postersData</param>
    public IEnumerator GetDoramaPosters(string data = null)
    {
        if(data != null)
        {
            yield return ParsePostersData(data);
            yield break;
        } 
        else
        {
            yield return ParsePostersData(DataManager.ReadData("_postersData", temp: DataManager.IsExists("_postersData", temp: true)));
        }
    }

    /// <summary>
    /// Check if doramaData exists on device
    /// </summary>
    /// <param name="data">doramaData</param>
    public IEnumerator GetDoramaData(string data = null)
    {
        if(data != null)
        {
            yield return ParseDoramaData(data);
            yield break;
        }
        else
        {
            yield return ParseDoramaData(DataManager.ReadData("_doramaData", temp: DataManager.IsExists("_doramaData", temp: true)));
        }
    }

    /// <summary>
    /// Check if languageData exists on device
    /// </summary>
    /// <param name="data">languageData</param>
    public IEnumerator GetLanguagesData(string data = null)
    {
        if (data != null)
        {
            yield return ParseLanguageData(data);
            yield break;
        }
        else
        {
            yield return ParseLanguageData(DataManager.ReadData("_languageData", temp: DataManager.IsExists("_languageData", temp: true)));
        }
    }

    /// <summary>
    /// Parse images to textures
    /// </summary>
    /// <param name="data">postersData</param>
    public async Task ParsePostersData(string data)
    {
    #if UNITY_EDITOR
        Debug.Log("PostersData processed");
    #endif
        if (string.IsNullOrEmpty(data))
            return;

        string[] rowsData = data.Split(lineSplit);

        for (int row = 1; row < rowsData.Length; row++)
        {
            string[] cellData = rowsData[row].Split(columnSplit);

            if (!DataBase.Doramas.ContainsKey(cellData[0]))
                continue;

            if (DataBase.Doramas[cellData[0]].IsLoaded)
                continue;

            string textureNameRaw = "";
            string textureURL = "";

            for (int cell = 1; cell < cellData.Length; cell++)
            {
                if (cellData[cell].StartsWith("https:"))
                {
                    textureURL = cellData[cell];
                    textureNameRaw = cellData[0];
                    break;
                }
            }

            if (string.IsNullOrEmpty(textureNameRaw))
                continue;
            
            Regex regex = new(@"\W", RegexOptions.Compiled);
            string textureName = regex.Replace(textureNameRaw, "");

            if (EthernetManager.ConnectionOn)
            {
                if (!DataManager.IsExists(textureName, temp: true, image: true) && !DataManager.IsExists(textureName, temp: false, image: true))
                {
                    await EthernetManager.PostersImageRemoteDownload(textureURL, (byte[] bytes) => { DataManager.SaveImage(textureName, bytes, temp: true); });
                }
            }

            Texture2D image = new(0, 0);
            image = DataManager.LoadImage(textureName, temp: DataManager.IsExists(textureName, temp: true, image: true));
            Sprite sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector3(.5f, .5f)); 
            Debug.Log($"Загружена картинка: {textureName}");
            DataBase.Doramas[cellData[0]].Poster.sprite = sprite;
            DataBase.Doramas[cellData[0]].IsLoaded = true;
    }

        DataBase.PostersIsLoaded = true;
    }

    /// <summary>
    /// Parse doramaData
    /// </summary>
    /// <param name="data">doramaData</param>
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

    /// <summary>
    /// Parse languageData
    /// </summary>
    /// <param name="data">languageData</param>
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

    /// <summary>
    /// Check if data exists & Not need to update
    /// </summary>
    public IEnumerator GetLocalData(Action callback = null)
    {
        yield return GetLanguagesDataLocal();
        yield return GetDoramaDataLocal();
        yield return GetDoramaPostersLocal();
        callback?.Invoke();
    }

    public IEnumerator GetDoramaPostersLocal()
    {
        if (UpdateLoader._isNeedToUpdate || !DataManager.IsExists("_postersData", temp: DataManager.IsExists("_postersData", temp: true)))
        {
            if (!EthernetManager.ConnectionOn)
            {
        #if UNITY_EDITOR
                Debug.Log("Connection problem, _postersData not loaded");
        #endif
            }
            yield return GetDoramaPosters();
            yield break;
        }
        else
        {
            yield return GetDoramaPosters(DataManager.ReadData("_postersData", temp: DataManager.IsExists("_postersData", temp: true)));
        }
    }

    public IEnumerator GetDoramaDataLocal()
    {
        if (UpdateLoader._isNeedToUpdate || !DataManager.IsExists("_doramaData", temp: DataManager.IsExists("_doramaData", temp: true)))
        {
            if (!EthernetManager.ConnectionOn)
            {
        #if UNITY_EDITOR
                Debug.Log("Connection problem, _doramaData not loaded");
        #endif
            }
            yield return GetDoramaData();
            yield break;
        } 
        else
        {
            yield return GetDoramaData(DataManager.ReadData("_doramaData", temp: DataManager.IsExists("_doramaData", temp: true)));
        }
    }

    public IEnumerator GetLanguagesDataLocal()
    {
        if (UpdateLoader._isNeedToUpdate || !DataManager.IsExists("_languageData", temp: DataManager.IsExists("_languageData", temp: true)))
        {
            if (!EthernetManager.ConnectionOn)
            {
        #if UNITY_EDITOR
                Debug.Log("Connection problem, _languageData not loaded");
        #endif
            }
            yield return GetLanguagesData();
            yield break;
        } 
        else
        {
            yield return GetLanguagesData(DataManager.ReadData("_languageData", temp: DataManager.IsExists("_languageData", temp: true)));
        }
    }
}
