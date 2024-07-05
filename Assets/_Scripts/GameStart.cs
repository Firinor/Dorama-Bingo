using UnityEngine;

public class GameStart : MonoBehaviour
{
    [ContextMenu("Start")]
    void Start()
    {
        var loader = new MainLoader();
        StartCoroutine(loader.GetLanguagesData());
    }
}
