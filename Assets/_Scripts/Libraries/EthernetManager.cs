using System;
using System.Collections;
using System.Net;
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

    public static bool _isNeedToUpdate = false;

    private static readonly string rowDelimiter = "\t";
    private static readonly string versionDelimiter = ".";

    public static IEnumerator ConnectionEstablish()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(DoramaDataURL))
        {
            yield return webRequest.SendWebRequest();

            _status = (HttpStatusCode)webRequest.responseCode;
        }
    }

    public static IEnumerator UpdateRemoteCheck(string url, string fileName, Action<string, string> callback = null)
    {
        string data;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

            data = webRequest.downloadHandler.text;
            if (!VersionCompare(DataManager.ReadData(fileName, temp: DataManager.IsExists(fileName, temp: true)), data))
            {
                if(url.Equals(PostersURL))
                    yield return DataManager.DownloadImage(data, true);

                callback?.Invoke(fileName, data);
                _isNeedToUpdate = true;
            }
        }
    }

    private static bool VersionCompare(string source, string destination)
    {
        try
        {
            var destinationRows = destination.Split(Environment.NewLine);
            var rowDestinationVersion = destinationRows[0]?.Split(rowDelimiter)[0];
            var destinationVersion = int.Parse(rowDestinationVersion?.Replace(versionDelimiter, ""));
            var sourceRows = source.Split(Environment.NewLine);
            var rowSourceVersion = sourceRows[0]?.Split(rowDelimiter)[0];
            var sourceVersion = int.Parse(rowSourceVersion?.Replace(versionDelimiter, ""));
            return destinationVersion.Equals(sourceVersion);
        }
        catch (IndexOutOfRangeException e)
        {
        #if UNITY_EDITOR
            Debug.Log(e);
        #endif
            throw;
        }
    }

    public static IEnumerator LanguageRemoteDownload(bool temp = true)
    {
        string data;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(LanguagesURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

        #if UNITY_EDITOR
            Debug.Log("Language Downloading...");
        #endif

            data = webRequest.downloadHandler.text;
            DataManager.WriteData("_languageData", data, temp);
        }
    }

    public static IEnumerator PostersImageRemoteDownload(string TextureURL, Action<byte[]> callback = null)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(TextureURL))
        {

            yield return www.SendWebRequest();

            if (!www.isDone)
                yield break;

            Texture2D posterTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            callback?.Invoke(posterTexture.EncodeToJPG());
            yield return null;
        }
    }

    public static IEnumerator PostersRemoteDownload(bool temp = true)
    {
        string data;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(PostersURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

        #if UNITY_EDITOR
            Debug.Log("Posters Downloading...");
        #endif

            data = webRequest.downloadHandler.text;
            DataManager.WriteData("_postersData", data, temp);
            yield return DataManager.DownloadImage(data, temp);
        }
    }

    public static IEnumerator DoramaRemoteDownload(bool temp = true)
    {
        string data;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(DoramaDataURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

        #if UNITY_EDITOR
            Debug.Log("Dorama Downloading...");
        #endif

            data = webRequest.downloadHandler.text;
            DataManager.WriteData("_doramaData", data, temp);
        }
    }

    public static bool ConnectionOn() => _status == HttpStatusCode.OK;
}
