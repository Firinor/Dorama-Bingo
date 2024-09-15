using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;

public static class DataManager
{
    public static readonly string DataPath = Application.dataPath;
    public static readonly string TempDataPath = Application.temporaryCachePath;



    public static void UpdateData(string fileName, string data)
    {
        using (StreamWriter sw = new(String.Format("{0}/{1}.csv", TempDataPath, fileName)))
        {
            sw.WriteLine(data);
        }
    }

    public static IEnumerator DownloadImage(string data, bool temp)
    {
        if (string.IsNullOrEmpty(data))
            yield break;

        StringBuilder newData = new StringBuilder();
        string[] rowsData = data.Split(MainLoader.lineSplit);
        for(int row = 1; row < rowsData.Length; row++)
        {
            string[] cellData = rowsData[row].Split(MainLoader.columnSplit);

            //if (!DataBase.Doramas.ContainsKey(cellData[0]))
            //    continue;

            //if (DataBase.Doramas[cellData[0]].IsLoaded)
            //    continue;

            Debug.Log(row);
            for (int cell = 1; cell < cellData.Length; cell++)
            {
                if (cellData[cell].StartsWith("https:"))
                {
                    yield return EthernetManager.PostersImageRemoteDownload(cellData[cell], (byte[] bytes) => { DataManager.SaveImage(cellData[0], bytes, temp); });
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
        using (StreamWriter sw = new(String.Format("{0}/{1}.csv", temp ? TempDataPath : DataPath + "/Resources", fileName)))
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
            using (StreamReader sr = new(String.Format("{0}/{1}.csv", TempDataPath, fileName)))
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
            return File.Exists(String.Format("{0}/{1}.{2}", TempDataPath, fileName, image ? "png" : "csv"));
        }
    }
}
