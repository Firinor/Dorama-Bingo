using UnityEditor;

public class BuildLoader
{
    [MenuItem("BuildLoader/Load Data")]
    private static async void DownloadFiles()
    {
        await EthernetManager.DoramaRemoteDownload(EthernetDataManager.SaveDoramaData);
        await EthernetManager.LanguageRemoteDownload(EthernetDataManager.SaveLanguageData);
        await EthernetManager.PostersRemoteDownload(EthernetDataManager.SavePostersData);
    }
}
