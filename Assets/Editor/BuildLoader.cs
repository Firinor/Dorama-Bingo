using UnityEditor;
using UnityEngine;

public class BuildLoader
{
    [MenuItem("BuildLoader/Load Data")]
    private static async void DownloadFiles()
    {
        Debug.Log("Something");
        await EthernetManager.DoramaRemoteDownload(false);
        await EthernetManager.LanguageRemoteDownload(false);
        await EthernetManager.PostersRemoteDownload(false);
        //StartCoroutine(EthernetManager.DoramaRemoteDownload(false));
        //StartCoroutine(EthernetManager.LanguageRemoteDownload(false));
        //StartCoroutine(EthernetManager.PostersRemoteDownload(false));
    }
}
