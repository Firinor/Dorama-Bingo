using System.Collections;
using UnityEngine;

public class UpdateLoader
{
    public static bool _isNeedToUpdate;

    public IEnumerator UpdateCheck()
    {
        if (!EthernetManager.ConnectionOn)
            yield break;

        yield return EthernetManager.UpdateRemoteCheck(EthernetManager.DoramaDataURL, "_doramaData", UpdateData);
        yield return EthernetManager.UpdateRemoteCheck(EthernetManager.LanguagesURL, "_languageData", UpdateData);
        yield return EthernetManager.UpdateRemoteCheck(EthernetManager.PostersURL, "_postersData", UpdateData);
        UpdateComplete();
    }

    private void UpdateData(string fileName,string data)
    {
        DataManager.WriteData(fileName, data);
    }

    private void UpdateComplete()
    {
        if (!_isNeedToUpdate)
            return;

    #if UNITY_EDITOR
        Debug.Log("Updated");
    #endif

        _isNeedToUpdate = false;
    }
}
