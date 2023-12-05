using System.IO;
using UnityEditor;

namespace Editor.AssetBundle
{
    public class CreateAssetBundles
    {
        [MenuItem("AssetsBundle/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            string dir = "AssetBundles";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.iOS);
        }
    }
}