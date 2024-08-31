using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System;


public class UpdateLoader
{
    private static readonly string DoramaDataURL =
        "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=tsv&gid=1984545305";
    private static readonly string LanguagesURL =
        "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=tsv&gid=2002448881";
    private static readonly string PostersURL =
        "https://docs.google.com/spreadsheets/d/1MP8xPYdW64FKz-T09psy4t61p-u9GB_f/export?format=tsv&gid=1844463073";


    private bool _isNeedToUpdate = false;


    public IEnumerator GetUpdate(Action callback = null)
    {
        yield return EthernetManager.ConnectionEstablish(DoramaDataURL);
        yield return UpdateCheck();
        yield return UpdateScene(callback);
    }

    public IEnumerator UpdateCheck()
    {
        if (EthernetManager.ConnectionOn())
        {
            yield return DoramaUpdateCheck();
            yield return LanguageUpdateCheck();
            yield return PostersUpdateCheck();
        }
    }

    public IEnumerator DoramaUpdateCheck()
    {
        string data;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(DoramaDataURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

            data = webRequest.downloadHandler.text;
            if (!data.Equals(PlayerPrefs.GetString("_doramaData")))
                yield return DoramaUpdate();
        }
    }

    public IEnumerator LanguageUpdateCheck()
    {
        string data;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(LanguagesURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

            data = webRequest.downloadHandler.text;
            if (!data.Equals(PlayerPrefs.GetString("_languageData")))
                yield return LanguageUpdate();
        }
    }

    public IEnumerator PostersUpdateCheck()
    {
        string data;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(PostersURL))
        {
            webRequest.timeout = 5;

            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone)
                yield break;

            data = webRequest.downloadHandler.text;
            if (!data.Equals(PlayerPrefs.GetString("_postersData")))
                yield return PostersUpdate();
        }
    }


    public IEnumerator DoramaUpdate()
    {
        Debug.Log("Dorama Update");
        _isNeedToUpdate = true;
        yield return null;
    }

    public IEnumerator LanguageUpdate()
    {
        Debug.Log("Language Update");
        _isNeedToUpdate = true;
        yield return null;
    }

    public IEnumerator PostersUpdate()
    {
        Debug.Log("Posters Update");
        _isNeedToUpdate = true;
        yield return null;
    }

    public IEnumerator UpdateScene(Action callback = null)
    {
        if (_isNeedToUpdate)
        {
            callback?.Invoke();
            _isNeedToUpdate = false;
            yield break;
        }
    }
}
