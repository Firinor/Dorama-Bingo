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
            if (!VersionCompare(data, DataManager.IsExists(fileName, true) ? DataManager.ReadData(fileName, true) : DataManager.ReadData(fileName, false)))
            {
                callback?.Invoke(fileName, data);
                _isNeedToUpdate = true;
            }
        }
    }

    private static bool VersionCompare(string baseData, string destData)
    {
        var destRows = destData.Split(Environment.NewLine);
        var baseRows = baseData.Split(Environment.NewLine);
        var destVersion = destRows[0].Split("\t")[0];
        var baseVersion = baseRows[0].Split("\t")[0];
        return destVersion.Equals(baseVersion);
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
