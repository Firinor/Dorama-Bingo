using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

public static class DataManager
{
    public static readonly string DataPath = Application.dataPath;
    public static readonly string TempDataPath = Application.temporaryCachePath;

    public static void UpdateData(string fileName, string data)
    {
        using (StreamWriter sw = new(String.Format("{0}/{1}.txt", TempDataPath, fileName)))
        {
            sw.WriteLine(data);
        }
    }

    public static async Task DownloadImage(string data, bool temp)
    {
        if (string.IsNullOrEmpty(data))
            return;

        StringBuilder newData = new StringBuilder();
        string[] rowsData = data.Split(MainLoader.lineSplit);
        for(int row = 1; row < rowsData.Length; row++)
        {
            string[] cellData = rowsData[row].Split(MainLoader.columnSplit);

            Regex regex = new(@"\W", RegexOptions.Compiled);


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
                    await EthernetManager.PostersImageRemoteDownload(cellData[cell], (byte[] bytes) => { DataManager.SaveImage(textureName, bytes, temp); });
                    break;
                }
            }
        }
    }

    public static void SaveImage(string fileName, byte[] imageBytes, bool temp = true)
    {
        File.WriteAllBytes(String.Format("{0}/{1}.png", temp ? TempDataPath : DataPath + "/Resources", fileName.Replace(": ", "")), imageBytes);
    }

    public static Texture2D LoadImage(string fileName, bool temp = true)
    {
        if (!temp)
        {
            var res = (Texture2D)Resources.Load($"{fileName}");
            return res;
        }
        else
        {
            var bytes = File.ReadAllBytes(String.Format("{0}/{1}.png", TempDataPath, fileName.Replace(": ", "")));
            Texture2D texture = new Texture2D(128, 128);
            texture.LoadImage(bytes);
            return texture;
            
        }
    }

    public static void WriteData(string fileName, string data, bool temp = true)
    {
        using (StreamWriter sw = new(String.Format("{0}/{1}.txt", temp ? TempDataPath : DataPath + "/Resources", fileName)))
        {
            sw.WriteLine(data);
        }
    }

    public static string ReadData(string fileName, bool temp)
    {
        if (!temp)
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

    public static bool IsExists(string fileName, bool temp, bool image = false)
    {

        if (!temp)
        {
            var res = Resources.Load($"{fileName}");
            return res != null;
        } 
        else
        {
            return File.Exists(String.Format("{0}/{1}.{2}", TempDataPath, fileName, image ? "png" : "txt"));
        }
    }
}
