// Class to hold PlayerPref data

using System.Collections.Generic;
using System.Globalization;
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
    private const string PLAYER_PREFS_KEYS = "PlayerPrefs_Keys"; // Key to store the list of PlayerPrefs keys

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
        var keysString = PlayerPrefs.GetString(PLAYER_PREFS_KEYS, string.Empty);
        var keys = new List<string>(keysString.Split(';'));

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
        var keys = GetAllKeys();
        foreach (var key in keys)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.DeleteKey($"Created_{key}");
            PlayerPrefs.DeleteKey($"Modified_{key}");
        }

        PlayerPrefs.DeleteKey(PLAYER_PREFS_KEYS);
        PlayerPrefs.Save();
    }

    public static List<string> SearchPlayerPrefs(string query)
    {
        var allKeys = GetAllKeys();
        var filteredKeys = new List<string>();

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
            PlayerPrefs.SetString($"Created_{key}", System.DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        PlayerPrefs.SetString($"Modified_{key}", System.DateTime.Now.ToString(CultureInfo.InvariantCulture));
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
        var keys = GetAllKeys();
        if (keys.Contains(key)) return;
        keys.Add(key);
        PlayerPrefs.SetString(PLAYER_PREFS_KEYS, string.Join(";", keys));
    }

    private static void RemoveKeyFromList(string key)
    {
        var keys = GetAllKeys();
        keys.Remove(key);
        PlayerPrefs.SetString(PLAYER_PREFS_KEYS, string.Join(";", keys));
    }
}