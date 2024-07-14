using UnityEngine;

public enum SaveKey
{
    CurrentCard,
    Language,
    SavedCards
}

public class SaveLoadManager
{
    public void Save(string data, SaveKey key)
    {
        PlayerPrefs.SetString(key.ToString(), data);
    }
    public void Save<T>(T data, SaveKey key)
    {
        string jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key.ToString(), jsonData);
    }

    public string Load(SaveKey key)
    {
        if (!PlayerPrefs.HasKey(key.ToString()))
            return default;

        return PlayerPrefs.GetString(key.ToString());
    }
    public T Load<T>(SaveKey key)
    {
        if(!PlayerPrefs.HasKey(key.ToString()))
            return default;

        string jsonData = PlayerPrefs.GetString(key.ToString());

        return JsonUtility.FromJson<T>(jsonData);
    }
}