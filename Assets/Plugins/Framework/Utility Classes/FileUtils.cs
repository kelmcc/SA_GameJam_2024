using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework
{

    public static class FileUtils
    {
        public static string SanitizeFilePath(string path)
        {
            return path.Replace('\\', '/').Trim('/');
        }

        public static string GetFileName(string path, bool includeExtension = false)
        {
            path = SanitizeFilePath(path);


            int start = path.LastIndexOf('/') + 1;

            if (includeExtension) return path.Substring(Mathf.Max(0, start));

            int end = path.LastIndexOf('.');
            if (end - start <= 0) return path;
            return path.Substring(start, end - start);

        }

        public static string GetFolderPath(string filePath)
        {
            filePath = SanitizeFilePath(filePath);
            int index = filePath.LastIndexOf('/');
            return index >= 0 ? filePath.Substring(0, index) : filePath;
        }

        public static string GetFileExtension(string path)
        {
            path = SanitizeFilePath(path);
            return path.Substring(path.LastIndexOf('.') + 1).ToLower();
        }

        public static string GetLocalPath(string absolouteAssetPath)
        {
            string path = SanitizeFilePath(absolouteAssetPath);
            return path.Substring(path.IndexOf("Assets/"));
        }

        public static string GetResourcePath(string absolouteAssetPath)
        {
            string path = SanitizeFilePath(absolouteAssetPath);

            path = path.Substring(path.IndexOf("Assets/Resources/") + 17);
            int dotIndex = path.LastIndexOf('.');

            return dotIndex >= 0 ? path.Substring(0, dotIndex) : path;
        }

        public static string GetAbsolutePath(string localAssetPath)
        {
            string path = SanitizeFilePath(localAssetPath);

            if (path.StartsWith("Assets/"))
            {
                path = path.Substring(path.IndexOf("/Assets/") + 8);
            }

            return Application.dataPath + "/" + path;
        }

#if UNITY_EDITOR

        public static DateTime GetLastModifiedTimeIncludingMetaFile(string localPath)
        {
            DateTime time = File.GetLastWriteTime(GetAbsolutePath(localPath));
            string metaFilePath = GetAbsolutePath(localPath + ".meta");

            if (File.Exists(metaFilePath))
            {
                DateTime metaTime = File.GetLastWriteTime(metaFilePath);
                if (metaTime > time) return metaTime;
            }

            return time;
        }

        public static Object[] GetAllAssets(Type type)
        {
            List<Object> assets = new List<Object>();
            string[] guids = AssetDatabase.FindAssets("t:" + type.Name);
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                Object asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets.ToArray();
        }

        public static T[] GetAllAssets<T>() where T : Object
        {
            return (T[])GetAllAssets(typeof(T));
        }

        public static T[] GetAssetsInFolder<T>(string folderPath, bool includeChildDirectories) where T : Object
        {
            List<T> assets = new List<T>();

            if (AssetDatabase.IsValidFolder(folderPath))
            {
                string[] paths = Directory.GetFiles(Application.dataPath.Substring(0, Application.dataPath.Length - 6) + folderPath, "*", includeChildDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                for (var i = 0; i < paths.Length; i++)
                {
                    string path = paths[i].Substring(Application.dataPath.Length - 6);
                    T asset = AssetDatabase.LoadAssetAtPath<T>(path);

                    if (asset != null)
                    {
                        assets.Add(asset);
                    }
                }
            }

            return assets.ToArray();
        }





        public static void CreateAssetFolders(string fileOrFolderPath)
        {
            if (fileOrFolderPath == "Assets") return;

            fileOrFolderPath = SanitizeFilePath(fileOrFolderPath);

            if (fileOrFolderPath.Contains('.'))
            {
                fileOrFolderPath = fileOrFolderPath.Substring(0, fileOrFolderPath.LastIndexOf('/'));
            }

            int numFolders = fileOrFolderPath.CountOccurencesOf('/');
            string lastPath = "";

            for (int i = 0; i < numFolders; i++)
            {
                string path = fileOrFolderPath.Substring(0, fileOrFolderPath.NthIndexOf(i + 1, '/'));
                string folderName = path.Substring(path.LastIndexOf('/') + 1);

                if (i != 0 || folderName != "Assets")
                {
                    if (!AssetDatabase.IsValidFolder(path))
                    {
                        AssetDatabase.CreateFolder(lastPath, folderName);
                    }
                }

                lastPath = path;
            }

            if (!AssetDatabase.IsValidFolder(fileOrFolderPath))
            {
                AssetDatabase.CreateFolder(lastPath, fileOrFolderPath.Substring(fileOrFolderPath.LastIndexOf('/') + 1));
            }
        }

        /// <summary>
        /// Creates/changes a text file and imports it.
        /// </summary>
        /// <param name="path">The path of the file to create</param>
        /// <param name="text">The contents of the text file</param>
        public static void CreateTextFile(string localPath, string text)
        {
            bool fileExists = FileUtils.AssetFileExists(localPath.Substring(localPath.LastIndexOf("/") + 1));

            File.WriteAllText(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/')) + "/" + localPath, text.Replace('\r', '\n').Replace("\n", "\r\n"));

            AssetDatabase.Refresh();
            UnityEngine.Debug.Log((fileExists ? "Updating" : "Creating") + " file: " + localPath, AssetDatabase.LoadMainAssetAtPath(localPath));
        }

        /// <summary>
        /// Checks wether a file with a certain name exists in the Assets directory, outs the path if it does.
        /// </summary>
        /// <param name="filename">The file name to check</param>
        /// <param name="filepath">The local file path (if a match is found, otherwise null)</param>
        /// <returns>Whether or not a file with that name was found in the Assets directory</returns>
        public static bool GetAssetFilePath(string filename, out string filepath)
        {
            string[] files = Directory.GetFiles(Application.dataPath, filename, SearchOption.AllDirectories);
            if (files.Length > 0)
            {
                filepath = files[0].Replace('\\', '/');
                filepath = "Assets/" + filepath.Replace(Application.dataPath + "/", String.Empty);
                return true;
            }

            filepath = null;
            return false;
        }

        public static bool AssetFileExists(string filename)
        {
            return Directory.GetFiles(Application.dataPath, filename, SearchOption.AllDirectories).Length > 0;
        }
#endif
    }
}
