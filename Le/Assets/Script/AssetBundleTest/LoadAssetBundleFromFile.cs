using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadAssetBundleFromFile : MonoBehaviour
{

    public Image img;
    private void Start()
    {
        AssetBundle bundle = AssetBundle.LoadFromFile("AssetBundles/main/go");
        GameObject[] gos = bundle.LoadAllAssets<GameObject>();
        foreach (var go in gos)
        {
            Instantiate(go);
        }

        bundle = AssetBundle.LoadFromFile("AssetBundles/tex");
        Sprite[] sprites = bundle.LoadAllAssets<Sprite>();
        int index = 0;
        foreach (var sp in sprites)
        {
            Image tmp = Instantiate(img, img.transform.parent);
            tmp.rectTransform.anchoredPosition = img.rectTransform.anchoredPosition + new Vector2(150 * index, 0);
            index++;
            tmp.sprite = sp;
        }
    }
}
