using UnityEditor;
using UnityEngine;

public class BuildLoader
{
    [MenuItem("BuildLoader/Load Data")]
    private static async void DownloadFiles()
    {
        await EthernetManager.DoramaRemoteDownload(false);
        await EthernetManager.LanguageRemoteDownload(false);
        await EthernetManager.PostersRemoteDownload(false);
    }
}
