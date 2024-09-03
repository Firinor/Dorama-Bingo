using System.Collections;
using UnityEngine;
using System;


public class UpdateLoader
{
    private bool _isNeedToUpdate;

    public IEnumerator GetUpdate(Action callback = null)
    {
        yield return EthernetManager.ConnectionEstablish();
        yield return UpdateCheck();
        yield return UpdateScene(callback);
    }

    public IEnumerator UpdateCheck()
    {
        if (EthernetManager.ConnectionOn())
        {
            yield return EthernetManager.UpdateRemoteCheck(EthernetManager.LanguagesURL, "_languageData", UpdateData);
            yield return EthernetManager.UpdateRemoteCheck(EthernetManager.DoramaDataURL, "_doramaData", UpdateData);
            yield return EthernetManager.UpdateRemoteCheck(EthernetManager.PostersURL, "_postersData", UpdateData);
            _isNeedToUpdate = EthernetManager._isNeedToUpdate;
        }
    }

    private void UpdateData(string fileName,string data)
    {
        DataManager.WriteData(fileName, data);
    } 

    public IEnumerator UpdateScene(Action callback = null)
    {
        if (_isNeedToUpdate)
        {
        #if UNITY_EDITOR
            Debug.Log("Updating...");
        #endif
            EthernetManager._isNeedToUpdate = false;
            callback?.Invoke();
            yield return null;
        }
    }
}
