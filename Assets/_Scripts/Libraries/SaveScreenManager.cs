using System;
using System.Collections;
using UnityEngine;

public class SaveScreenManager : MonoBehaviour
{
    private Coroutine _cor;
    public void SaveCurrentCardScreen(string screenPath, Action callback = null)
    {
        _cor ??= StartCoroutine(SaveCurrentCardScreenCoroutine(screenPath, callback));
    }

    private IEnumerator SaveCurrentCardScreenCoroutine(string screenPath, Action callback = null)
    {
        if (System.IO.File.Exists(screenPath))
            System.IO.File.Delete(screenPath);

        yield return new WaitForEndOfFrame();
        Texture2D texture = new(Screen.width, Screen.height);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        new SaveLoadSystem().SaveTexture(texture, screenPath);
        callback?.Invoke();
        yield return null;
        _cor = null;
    }
}
