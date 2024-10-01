// Class to hold PlayerPref data

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class PlayerPrefData
{
    public string Key { get; }
    public string Value { get; }
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
    public enum PlayerPrefType
    {
        String,
        Int,
        Float
    }

    private const string PLAYER_PREFS_KEYS = "PlayerPrefs_Keys"; // Key to store the list of PlayerPrefs keys

    public static void SetValue(string key, object value, PlayerPrefType type)
    {
        if (!IsNewKey(key) && GetKeyType(key) != type)
        {
            Debug.LogError($"Key <b>'{key}'</b> already exists with a different type. Please delete the key or use a different key.");
            return;
        }

        switch (type)
        {
            case PlayerPrefType.String:
                PlayerPrefs.SetString(key, value.ToString());
                break;
            case PlayerPrefType.Int:
                PlayerPrefs.SetInt(key, int.Parse(value.ToString()));
                break;
            case PlayerPrefType.Float:
                PlayerPrefs.SetFloat(key, float.Parse(value.ToString()));
                break;
        }

        Debug.Log($"PlayerPref <b>'{key}'</b> saved with value: <b>'{value}'</b> - type: <b>'{type}'</b>");

        UpdateTimestamps(key);
        AddKeyToList(key);
        SetKeyType(key, type);
        PlayerPrefs.Save();
    }

    private static bool IsNewKey(string key)
    {
        return !GetAllKeys().Contains(key);
    }

    public static object GetValue(string key)
    {
        var type = GetKeyType(key);
        return type switch
               {
                   PlayerPrefType.String => PlayerPrefs.GetString(key, "N/A"),
                   PlayerPrefType.Int => PlayerPrefs.GetInt(key, 0),
                   PlayerPrefType.Float => PlayerPrefs.GetFloat(key, 0f),
                   _ => null
               };
    }

    public static PlayerPrefType GetKeyType(string key)
    {
        var typeString = PlayerPrefs.GetString($"Type_{key}", PlayerPrefType.String.ToString());
        return (PlayerPrefType)Enum.Parse(typeof(PlayerPrefType), typeString);
    }

    public static List<string> GetAllKeys()
    {
        var keysString = PlayerPrefs.GetString(PLAYER_PREFS_KEYS, string.Empty);
        var keys = new List<string>(keysString.Split(';'));

        // Remove any empty keys
        keys.RemoveAll(string.IsNullOrEmpty);
        return keys;
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey($"Created_{key}");
        PlayerPrefs.DeleteKey($"Modified_{key}");
        PlayerPrefs.DeleteKey($"Type_{key}");
        RemoveKeyFromList(key);
        PlayerPrefs.Save();
    }

    public static void ClearAll()
    {
        var keys = GetAllKeys();
        foreach (var key in keys)
        {
            DeleteKey(key);
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
            var valueAsString = GetValue(key).ToString();
            if (key.Contains(query) || valueAsString.Contains(query))
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
            PlayerPrefs.SetString($"Created_{key}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
        }

        PlayerPrefs.SetString($"Modified_{key}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
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

    private static void SetKeyType(string key, PlayerPrefType type)
    {
        PlayerPrefs.SetString($"Type_{key}", type.ToString());
    }

    private static void RemoveKeyFromList(string key)
    {
        var keys = GetAllKeys();
        keys.Remove(key);
        PlayerPrefs.SetString(PLAYER_PREFS_KEYS, string.Join(";", keys));
    }
}