using System;
using System.Collections;
using UnityEngine;

public class SaveScreenManager : MonoBehaviour
{
    private RectTransform objectToScreen;
    private Coroutine _cor;
    public void SaveCurrentCardScreen(string screenPath, RectTransform objToScreen, Action callback = null)
    {
        objectToScreen = objToScreen;
        _cor ??= StartCoroutine(SaveCurrentCardScreenCoroutine(screenPath, callback));
    }

    private IEnumerator SaveCurrentCardScreenCoroutine(string screenPath, Action callback = null)
    {
        if (System.IO.File.Exists(screenPath))
            System.IO.File.Delete(screenPath);


        yield return new WaitForEndOfFrame();

        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, objectToScreen.position);
        Vector2 screenSize = new Vector2(objectToScreen.rect.width, objectToScreen.rect.height);

        Rect captureRect = new Rect(screenPosition.x - screenSize.x / 2,
                            screenPosition.y - screenSize.y / 2,
                            screenSize.x,
                            screenSize.y);

        Texture2D texture = new(((int)captureRect.width), ((int)captureRect.height));
        texture.ReadPixels(captureRect, 0, 0);
        texture.Apply();

        new SaveLoadSystem().SaveTexture(texture, screenPath);
        Destroy(texture);

        yield return null;
        
        _cor = null;
        callback?.Invoke();
    }
}
