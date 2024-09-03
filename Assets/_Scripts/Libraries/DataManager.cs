using System;
using System.IO;
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
        } else
        {
            using (StreamReader sr = new(String.Format("{0}/{1}.txt", TempDataPath, fileName)))
            {
                return sr.ReadToEnd();
            }
        }
    }

    public static bool IsExists(string fileName, bool temp)
    {

        if (!temp)
        {
            var res = Resources.Load($"{fileName}");
            return res != null;
        } else
        {
            return File.Exists(String.Format("{0}/{1}", TempDataPath, fileName));
        }
    }
}
