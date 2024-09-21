using System.Threading.Tasks;
using UnityEngine;

public static class EthernetDataManager
{
    public static async void SaveLanguageData(string data)
    {
        await DataManager.WriteData("_languageData", data, resources: true);
    }

    public static async void SaveDoramaData(string data)
    {
        await DataManager.WriteData("_doramaData", data, resources: true);
    }

    public static async void SavePostersData(string data)
    {
        await DataManager.WriteData("_postersData", data, resources: true);
        var task = DataManager.DownloadImage(data, resources: true);
        await task;
    }
}
