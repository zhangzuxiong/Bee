using System;
using UnityEngine;

public class LoadAssetBundleFromFile : MonoBehaviour
{
    private void Start()
    {
        AssetBundle bundle = AssetBundle.LoadFromFile("AssetBundles/main/go");
        GameObject go = bundle.LoadAsset<GameObject>("Capsule");
        Instantiate(go);
    }
}
