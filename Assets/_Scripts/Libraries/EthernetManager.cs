using System.Collections;
using UnityEngine.Networking;

public static class EthernetManager
{
    public static CONNECTION_STATUS _status;
    public static IEnumerator ConnectionEstablish(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (!webRequest.isDone | webRequest.result == UnityWebRequest.Result.ConnectionError)
                _status = CONNECTION_STATUS.FAILED;

            _status = CONNECTION_STATUS.SUCCESS;
        }
    }

    public static bool ConnectionOn() => _status != CONNECTION_STATUS.FAILED;
}
