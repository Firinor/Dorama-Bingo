using UnityEngine;

public enum SaveKey
{
    CurrentCard,
    Language,
    SavedCards
}

public class SaveLoadSystem
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
    public void SaveTexture(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToJPG();
        System.IO.File.WriteAllBytes(path, bytes);
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
    public Texture2D LoadTexture(string path)
    {
        if(!System.IO.File.Exists(path))
            return null;

        byte[] bytes = System.IO.File.ReadAllBytes(path);

        Texture2D texture2D = new(0, 0);//Size will be updated when loading data
        texture2D.LoadImage(bytes);

        return texture2D;

    }
}