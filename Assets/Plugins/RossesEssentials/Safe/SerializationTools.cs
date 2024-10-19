using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

public static class SerializationTools
{
#if UNITY_EDITOR
    private static PropertyInfo isSerializingProperty;

    static SerializationTools()
    {
        isSerializingProperty = typeof(EditorUtility).GetProperty("isSerializing", BindingFlags.NonPublic | BindingFlags.Static);
    }

    public static bool IsSerializing()
    {
        if (isSerializingProperty != null)
        {
            return (bool)isSerializingProperty.GetValue(null, null);
        }
        return false;
    }
#else
    public static bool IsSerializing()
    {
        return false; // In build, serialization check is not applicable
    }
#endif
    
#if UNITY_EDITOR
    public static bool CheckIfDataHasChanged(Object target)
    {
        string currentHashString = GetDataHashCode(target);

        string assetPath = AssetDatabase.GetAssetPath(target);
        string metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);
        List<string> currentMeta = new List<string>(File.ReadLines(metaPath));

        bool thisObjectHasChanged = true;
        if (currentMeta.Count > 0)
        {
            string lastLine = currentMeta[^1].Trim();
            if (lastLine.Contains("dataChangeHash:"))
            {
                string lastHash = lastLine.Replace("dataChangeHash:", "").Trim();;

                if (currentHashString == lastHash)
                {
                    thisObjectHasChanged = false;
                }
            }
        }
        return thisObjectHasChanged;
    }

    public static string GetDataHashCode(Object target)
    {
        var hash = new Hash128();
        hash.Append(JsonUtility.ToJson(target));
        return hash.ToString();
    }

    public static void RecordDataChange(Object target)
    {
        string currentHashString = GetDataHashCode(target);
        string assetPath = AssetDatabase.GetAssetPath(target);
        string metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);
        List<string> currentMeta = new (File.ReadLines(metaPath));

        bool thisObjectHasChanged = true;
        if (currentMeta.Count > 0)
        {
            string newLine = $"  dataChangeHash: {currentHashString}";
            string lastLine = currentMeta[^1].Trim();
            if (lastLine.Contains("dataChangeHash:"))
            {
                currentMeta[^1] = newLine;
            }
            else
            {
                currentMeta.Add(newLine);
            }
            File.WriteAllLines(metaPath, currentMeta);
        }
    }
    
#else
    public static bool CheckIfDataHasChanged(Object target)
    {
        //Data cant change in build.
        return false;
    }
    
    public static string GetDataHashCode(Object target)
    {
        return "0";
    }
    
    public static void RecordDataChange(Object target)
    {
        //Do nothing in build.
    }
#endif
    
}