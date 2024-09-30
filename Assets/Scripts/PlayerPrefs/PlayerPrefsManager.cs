// Class to hold PlayerPref data

using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefData
{
    public string Key { get; }
    public string Value { get; set; }
    public string CreatedTime { get; }
    public string ModifiedTime { get; }

    public PlayerPrefData(string key, string value, string createdTime, string modifiedTime)
    {
        Key = key;
        Value = value;
        CreatedTime = createdTime;
        ModifiedTime = modifiedTime;
    }
}

// Custom PlayerPrefsManager class with additional methods for key management, search functionality, and timestamps
public static class PlayerPrefsManager
{
    private const string PlayerPrefsKeys = "PlayerPrefs_Keys"; // Key to store the list of PlayerPrefs keys

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
        UpdateTimestamps(key);
        AddKeyToList(key);
        PlayerPrefs.Save();
    }

    public static string GetString(string key)
    {
        return PlayerPrefs.GetString(key);
    }

    public static List<string> GetAllKeys()
    {
        string keysString = PlayerPrefs.GetString(PlayerPrefsKeys, string.Empty);
        List<string> keys = new List<string>(keysString.Split(';'));

        // Remove any empty keys
        keys.RemoveAll(k => string.IsNullOrEmpty(k));
        return keys;
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey($"Created_{key}");
        PlayerPrefs.DeleteKey($"Modified_{key}");
        RemoveKeyFromList(key);
        PlayerPrefs.Save();
    }

    public static void ClearAll()
    {
        List<string> keys = GetAllKeys();
        foreach (var key in keys)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.DeleteKey($"Created_{key}");
            PlayerPrefs.DeleteKey($"Modified_{key}");
        }
        PlayerPrefs.DeleteKey(PlayerPrefsKeys);
        PlayerPrefs.Save();
    }

    public static List<string> SearchPlayerPrefs(string query)
    {
        List<string> allKeys = GetAllKeys();
        List<string> filteredKeys = new List<string>();

        foreach (var key in allKeys)
        {
            if (key.Contains(query) || GetString(key).Contains(query))
            {
                filteredKeys.Add(key);
            }
        }

        return filteredKeys;
    }

    // Update timestamps for created and modified time
    private static void UpdateTimestamps(string key)
    {
        if (!PlayerPrefs.HasKey($"Created_{key}"))
        {
            PlayerPrefs.SetString($"Created_{key}", System.DateTime.Now.ToString());
        }
        PlayerPrefs.SetString($"Modified_{key}", System.DateTime.Now.ToString());
    }

    public static string GetCreatedTime(string key)
    {
        return PlayerPrefs.GetString($"Created_{key}", "N/A");
    }

    public static string GetModifiedTime(string key)
    {
        return PlayerPrefs.GetString($"Modified_{key}", "N/A");
    }

    private static void AddKeyToList(string key)
    {
        List<string> keys = GetAllKeys();
        if (!keys.Contains(key))
        {
            keys.Add(key);
            PlayerPrefs.SetString(PlayerPrefsKeys, string.Join(";", keys));
        }
    }

    private static void RemoveKeyFromList(string key)
    {
        List<string> keys = GetAllKeys();
        keys.Remove(key);
        PlayerPrefs.SetString(PlayerPrefsKeys, string.Join(";", keys));
    }
}