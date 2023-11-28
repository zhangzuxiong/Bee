using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Editor.Tools
{
    public abstract class ReplaceSpecialCharTool
    {

        private static readonly Dictionary<string, string> SpecialChar = new Dictionary<string, string>()
        {
            {" ","_"},
        };

        [MenuItem("Assets/CheckSpecialChar")]
        public static void CheckSpecialChar()
        {
            var assets = Selection.assetGUIDs;
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                CheckAllSpecialChar(path,false);
            }
        }

        [MenuItem("Assets/ReplaceSpecialChar")]
        public static void ReplaceSpecialChar()
        {
            Debug.Log("===========start===========");
            var assets = Selection.assetGUIDs;
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                CheckAllSpecialChar(path, true);
            }
            Debug.Log($"=========== end ===========");
        }

        private static string FullPathToUnityPath(string path)
        {
            int index = path.IndexOf("Assets", StringComparison.Ordinal);
            if(index > -1)
                path = path.Substring(index);
            return path;
        }
        private static void CheckAllSpecialChar(string path,bool needReplace)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (CheckHasSpecialChar(directoryInfo.Name))
                {
                    path = FullPathToUnityPath(path);
                    if (needReplace)
                    {
                        path = Rename(path);
                        directoryInfo = new DirectoryInfo(path);
                    }
                    else
                    {
                        Debug.Log(path);
                    }
                }

                FileInfo[] fileInfos = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly);
                foreach (var fileInfo in fileInfos)
                {
                    if (!fileInfo.FullName.Contains("meta"))
                    {
                        CheckAllSpecialChar(FullPathToUnityPath(fileInfo.FullName),needReplace);
                    }
                }

                DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
                foreach (var dir in directoryInfos)
                {
                    CheckAllSpecialChar(FullPathToUnityPath(dir.FullName),needReplace);
                }
            }
            else
            {
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                string name = obj.name;
                if (CheckHasSpecialChar(name))
                {
                    path = FullPathToUnityPath(path);
                    if (needReplace)
                    {
                        Rename(path);
                    }
                    else
                    {
                        Debug.Log(path);
                    }
                }
            }
        }

        private static string Rename(string path)
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            string name = obj.name;
            if (CheckHasSpecialChar(name))
            {
                Replace(ref name);
                AssetDatabase.RenameAsset(path, name);
            }
            return AssetDatabase.GetAssetPath(obj);
        }

        private static bool CheckHasSpecialChar(string str)
        {
            bool flag = false;
            foreach (var pair in SpecialChar)
            {
                flag = flag || str.Contains(pair.Key);
            }

            return flag;
        }
        private static void Replace(ref string name)
        {
            foreach (var pair in SpecialChar)
            {
                name = name.Replace(pair.Key, pair.Value);
            }
        }
    }
}
