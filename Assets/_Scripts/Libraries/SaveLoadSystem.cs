using System;
using System.IO;
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
        PlayerPrefs.Save();
    }

    public void Save<T>(T data, SaveKey key)
    {
        string jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key.ToString(), jsonData);
        PlayerPrefs.Save();
    }

    public void Save<T>(T[] data, SaveKey key)
    {
        Wrapper<T> wrapper = new();
        wrapper.Items = data;
        string jsonData = JsonUtility.ToJson(wrapper, true);
        PlayerPrefs.DeleteKey(key.ToString());
        PlayerPrefs.SetString(key.ToString(), jsonData);
        PlayerPrefs.Save();
    }

    public void SaveTexture(Texture2D texture, string path)
    {
        byte[] bytes = texture.EncodeToJPG();

        using (FileStream fs = File.Create(path))
        {
            fs.Write(bytes, 0, bytes.Length);
        }
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

    public T[] LoadArray<T>(SaveKey key)
    {
        if (!PlayerPrefs.HasKey(key.ToString()))
            return default;

        string jsonData = PlayerPrefs.GetString(key.ToString());

        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(jsonData);
        return wrapper.Items;
    }

    public Texture2D LoadTexture(string path)
    {
        if(!System.IO.File.Exists(path))
            return null;

        byte[] bytes;
        using (FileStream fs = File.OpenRead(path))
        {
            bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
        }
        Texture2D texture2D = new(0, 0);
        texture2D.LoadImage(bytes);

        return texture2D;
    }

    public void DeleteTexture(string path)
    {
        if (!System.IO.File.Exists(path))
            return;

        System.IO.File.Delete(path);
    }
    
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}