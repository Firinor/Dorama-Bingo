using UnityEngine;

public class BuildLoader : MonoBehaviour
{
    public void DownloadFiles()
    {
        StartCoroutine(EthernetManager.DoramaRemoteDownload(false));
        StartCoroutine(EthernetManager.LanguageRemoteDownload(false));
        StartCoroutine(EthernetManager.PostersRemoteDownload(false));
    }
}
