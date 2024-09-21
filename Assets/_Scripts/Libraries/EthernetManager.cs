using System;
using System.Collections;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class EthernetManager
{
    public static readonly string DoramaDataURL =
    "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=tsv&gid=1984545305";
    public static readonly string LanguagesURL =
        "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=tsv&gid=2002448881";
    public static readonly string PostersURL =
        "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=tsv&gid=1844463073";
    
    public static HttpStatusCode _status;
    public static bool ConnectionOn => _status == HttpStatusCode.OK;

    private static readonly string rowDelimiter = "\t";
    private static readonly string versionDelimiter = ".";

    public static IEnumerator ConnectionEstablish()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(DoramaDataURL))
        {
            yield return webRequest.SendWebRequest();

            _status = (HttpStatusCode)webRequest.responseCode;
        
            Debug.Log(_status);
        }
    }

    public static IEnumerator UpdateRemoteCheck(string url, string fileName, Action<string, string> onFinish)
    {
        string data;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

            data = webRequest.downloadHandler.text;
            if (!VersionCompare(DataManager.ReadData(fileName, resources: !DataManager.IsExists(fileName, resources: false)), data))
            {
                UpdateLoader._isNeedToUpdate = true;
                onFinish.Invoke(fileName, data);
            }
        }
    }

    private static bool VersionCompare(string source, string destination)
    {
        string rowDestinationVersion;
        string rowSourceVersion;
        try
        {
            var destinationRows = destination.Split(Environment.NewLine);
            rowDestinationVersion = destinationRows[0]?.Split(rowDelimiter)[0];
            var sourceRows = source.Split(Environment.NewLine);
            rowSourceVersion = sourceRows[0]?.Split(rowDelimiter)[0];
            
        }
        catch (IndexOutOfRangeException e)
        {
        #if UNITY_EDITOR
            Debug.Log(e);
        #endif
            throw;
        }
        var destinationVersion = int.Parse(rowDestinationVersion?.Replace(versionDelimiter, ""));
        var sourceVersion = int.Parse(rowSourceVersion?.Replace(versionDelimiter, ""));
        return destinationVersion.Equals(sourceVersion);
    }

    public static async Task PostersImageRemoteDownload(string TextureURL, Action<byte[]> callback = null)
    {
        Texture2D posterTexture;
        
        using var webRequest = UnityWebRequestTexture.GetTexture(TextureURL);

        webRequest.timeout = 5;

        var operation = webRequest.SendWebRequest();

        while(!operation.isDone)
            await Task.Yield();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
#if UNITY_EDITOR 
            Debug.Log("Failed to Download Poster");
#endif
            return;
        }
        
        posterTexture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
        callback?.Invoke(posterTexture.EncodeToJPG());
    }

    public static async Task LanguageRemoteDownload(Action<string> callback = null)
    {
        string data;

        using var webRequest = UnityWebRequest.Get(LanguagesURL);

        webRequest.timeout = 5;

        var operation = webRequest.SendWebRequest();

        while(!operation.isDone)
        {
#if UNITY_EDITOR
            Debug.Log("Language Downloading...");
#endif
            await Task.Yield();
        }

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
#if UNITY_EDITOR
            Debug.Log("Failed to download LanguageData");
#endif
            return;
        }

        data = webRequest.downloadHandler.text;
            callback?.Invoke(data);
    }

    public static async Task PostersRemoteDownload(Action<string> callback = null)
    {
        string data;

        using var webRequest = UnityWebRequest.Get(PostersURL);

        webRequest.timeout = 5;

        var operation = webRequest.SendWebRequest();

        while(!operation.isDone)
        {
#if UNITY_EDITOR
            Debug.Log("Posters Downloading...");
#endif
            await Task.Yield();
        }

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
#if UNITY_EDITOR
            Debug.Log("Failed to download PostersData");
#endif
            return;
        }

        data = webRequest.downloadHandler.text;
        callback?.Invoke(data);
    }

    public static async Task DoramaRemoteDownload(Action<string> callback = null)
    {
        string data;

        using var webRequest = UnityWebRequest.Get(DoramaDataURL);

        webRequest.timeout = 5;

        var operation = webRequest.SendWebRequest();

        while (!operation.isDone)
        {

#if UNITY_EDITOR
            Debug.Log("Dorama Downloading...");
#endif
            await Task.Yield();
        }

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
#if UNITY_EDITOR
            Debug.Log("Failed to downnload DoramaData");
#endif
            return;
        }

        data = webRequest.downloadHandler.text;
        callback?.Invoke(data);
    }

}
