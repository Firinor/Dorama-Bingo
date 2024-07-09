using UnityEngine;

public static class SaveLoadSystem
{
    public static T LoadData<T>(string key)
    {
        string loadData = PlayerPrefs.GetString(key);
        T result = JsonUtility.FromJson<T>(loadData);
        return result;
    }

    public static void SaveData<T>(string key, T data)
    {
        string saveData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, saveData);
    }
}
