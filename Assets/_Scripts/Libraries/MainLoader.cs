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
            yield return updateLoader.UpdateCheck();

        yield return ParseLocalData();
        callback?.Invoke();
    }

    /// <summary>
    /// Check if data exists & Not need to update
    /// </summary>
    public IEnumerator ParseLocalData()
    {
        yield return ParseDoramaData(DataManager.ReadData("_doramaData", resources: !DataManager.IsExists("_doramaData", resources: false)));
        yield return ParseLanguageData(DataManager.ReadData("_languageData", resources: !DataManager.IsExists("_languageData", resources: false)));
        yield return ParsePostersData(DataManager.ReadData("_postersData", resources: !DataManager.IsExists("_postersData", resources: false)));
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
            byte[] textureBytes = null;

            if (EthernetManager.ConnectionOn)
            {
                if (!DataManager.IsExists(textureName, resources: true, image: true) && !DataManager.IsExists(textureName, resources: false, image: true))
                {
                    await EthernetManager.PostersImageRemoteDownload(textureURL, (byte[] bytes) => 
                    {
                        DataManager.SaveImage(textureName, bytes, resources: false);
                        textureBytes = bytes; 
                    });
                }
            }

            Texture2D image = new(0, 0);
            if (textureBytes == null)
            {
                image = DataManager.LoadImage(textureName, resources: !DataManager.IsExists(textureName, resources: false, image: true));
            } 
            else
            {
                image.LoadImage(textureBytes);
            }
            Sprite sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector3(.5f, .5f)); 
            Debug.Log($"Image downloaded: {textureName}");
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
}
