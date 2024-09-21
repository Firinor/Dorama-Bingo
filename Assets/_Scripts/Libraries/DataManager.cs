using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public static class DataManager
{
    public static readonly string DataPath = Application.dataPath;
    public static readonly string TempDataPath = Application.temporaryCachePath;
    static readonly Regex regex = new(@"\W", RegexOptions.Compiled);

    public static async Task DownloadImage(string data, bool resources)
    {
        List<Task> tasks = new();
        
        if (string.IsNullOrEmpty(data))
            return;

        string[] rowsData = data.Split(MainLoader.lineSplit);
        for(int row = 1; row < rowsData.Length; row++)
        {
            string[] cellData = rowsData[row].Split(MainLoader.columnSplit);
#if !UNITY_EDITOR
            if (!DataBase.Doramas.ContainsKey(cellData[0]))
                continue;

            if (DataBase.Doramas[cellData[0]].IsLoaded)
                continue;
#endif
#if UNITY_EDITOR
            Debug.Log(row);
#endif
            for (int cell = 1; cell < cellData.Length; cell++)
            {
                if (cellData[cell].StartsWith("https:"))
                {
                    string textureNameRaw = cellData[0];
                    string textureName = regex.Replace(textureNameRaw, "");
                    var task = EthernetManager.PostersImageRemoteDownload(cellData[cell], (byte[] bytes) => { SaveImage(textureName, imageBytes: bytes, resources: resources); });
                    tasks.Add(task);
                    break;
                }
            }
        }
        if(tasks.Count > 0)
        {
            await Task.WhenAll(tasks);
        }
    }

    public static void SaveImage(string fileName, byte[] imageBytes, bool resources = false)
    {
        File.WriteAllBytes(String.Format("{0}/{1}.png", resources ? DataPath + "/Resources" : TempDataPath, regex.Replace(fileName, "")), imageBytes);
    }

    public static Texture2D LoadImage(string fileName, bool resources = false)
    {
        if (resources)
        {
            var res = (Texture2D)Resources.Load($"{fileName}");
            return res;
        }
        else
        {
            var bytes = File.ReadAllBytes(String.Format("{0}/{1}.png", TempDataPath, regex.Replace(fileName, "")));
            Texture2D texture = new(0, 0);
            texture.LoadImage(bytes);
            return texture;
        }
    }

    public static async Task WriteData(string fileName, string data, bool resources = false)
    {
        using (StreamWriter sw = new(String.Format("{0}/{1}.txt", resources ? DataPath + "/Resources" : TempDataPath, fileName)))
        {
            await sw.WriteLineAsync(data);
        }
    }

    public static string ReadData(string fileName, bool resources)
    {
        if (resources)
        {
            var res = (TextAsset)Resources.Load($"{fileName}");
            
            return res.text;
        } 
        else
        {
            using (StreamReader sr = new(String.Format("{0}/{1}.txt", TempDataPath, fileName)))
            {
                return sr.ReadToEnd();
            }
        }
    }

    public static bool IsExists(string fileName, bool resources, bool image = false)
    {
        if (resources)
        {
            var res = Resources.Load($"{fileName}");
            return res != null;
        } 
        else
        {
            return File.Exists(String.Format("{0}/{1}.{2}", TempDataPath, image ? fileName : regex.Replace(fileName, ""), image ? "png" : "txt"));
        }
    }
}
