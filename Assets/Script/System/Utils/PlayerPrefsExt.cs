using UnityEngine;
using System.Security.Cryptography;

public static class PlayerPrefsExt
{
    private static AES128 aes128 = new AES128();

    /// <summary>
    /// Save the value with the key to the player preferences
    /// </summary>
    /// <typeparam name="T">the data type of value</typeparam>
    /// <param name="key">the key to find</param>
    /// <param name="value">the value to save</param>
    /// <param name="encryption">if true, encrypt the key and the value</param>
    public static void SetObject<T>(string key, T value, bool encryption = true)
    {
        string jsonValue = JsonUtility.ToJson(value);

        string savingKey = encryption ? aes128.Encrypt(key) : key;
        string savingValue = encryption ? aes128.Encrypt(jsonValue) : jsonValue;

        PlayerPrefs.SetString(savingKey, savingValue);
    }

    /// <summary>
    /// Get the value using the key from the player preferences
    /// </summary>
    /// <typeparam name="T">the data type of value</typeparam>
    /// <param name="originalKey">the key to find</param>
    /// <param name="defaultValue">default value</param>
    /// <param name="encryption">if true, encrypt the key and get value</param>
    /// <returns></returns>
    public static T GetObject<T>(string originalKey, T defaultValue = default(T), bool encryption = true)
    {
        string savedKey = encryption ? aes128.Encrypt(originalKey) : originalKey;
        string savedValue;

        if(!PlayerPrefs.HasKey(savedKey))
        {
            return defaultValue;
        }

        savedValue = PlayerPrefs.GetString(savedKey, "");
        string originalValue = encryption ? aes128.Decrypt(savedValue) : savedValue;

        if(originalValue == "") return defaultValue;

        return JsonUtility.FromJson<T>(originalValue);
    }

    public static bool HasKey(string key, bool encryption = true)
    {
        key = encryption ? aes128.Encrypt(key) : key;
        return PlayerPrefs.HasKey(key);
    }

    public static void DeleteKey(string key, bool encryption = true)
    {
        key = encryption ? aes128.Encrypt(key) : key;
        PlayerPrefs.DeleteKey(key);
    }
}
