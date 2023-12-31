using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools
{
    public abstract class CheckParticleNull
    {
        
        [MenuItem("Assets/检查特效材质丢失")]
        public static void CheckNull()
        {
            var assets = Selection.assetGUIDs;
            foreach (var asset in assets)
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                if (Directory.Exists(path))
                {
                    CheckAllPrefab(path);
                }
                else if (asset.Contains("prefab"))
                {
                    CheckParticlePrefabMatIsNull(path);
                }
            }
        }

        private static void CheckAllPrefab(string path)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            FileInfo[] fileInfos = directoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
            foreach (var fileInfo in fileInfos)
            {
                int index = fileInfo.FullName.IndexOf("Assets", StringComparison.Ordinal);
                string proPath = fileInfo.FullName.Substring(index);
                CheckParticlePrefabMatIsNull(proPath);
            }
        }

        private static string GetPathInChild(GameObject root, GameObject child)
        {
            string path = child.name;
            while (child != root)
            {
                child = child.transform.parent.gameObject;
                path = child.name + "/" + path;
            }

            return path;
        }
        private static void CheckParticlePrefabMatIsNull(string path)
        {
            Debug.Log("==============start==============");
            GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if(go == null) return;
            ParticleSystem[] ps = go.GetComponentsInChildren<ParticleSystem>();
            foreach (var item in ps)
            {
                var render = item.GetComponent<Renderer>();
                if (render.sharedMaterial == null)
                {
                    Debug.LogError($"{path} 材质丢失:{GetPathInChild(go, item.gameObject)}");
                }
            }
            Debug.Log("===============end==============");
        }
    }
}