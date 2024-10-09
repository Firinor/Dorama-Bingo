using System.Collections;
using UnityEngine;

public class MainWindow : MonoBehaviour
{
    public GameObject continueButton;
    public GameObject loadButton;

    private void OnEnable()
    {
        StartCoroutine(ButtonsVisibility());
    }

    private IEnumerator ButtonsVisibility()
    {
        yield return null;

        continueButton.SetActive(PlayerData.CurrentBingoCard != null);
        loadButton.SetActive(PlayerData.SavedBingoCards != null);
    }
}