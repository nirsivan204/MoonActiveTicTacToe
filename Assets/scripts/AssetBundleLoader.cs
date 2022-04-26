using UnityEngine;
using System.IO;
using UnityEditor;

public static class AssetBundleLoader
{
    public static Texture2D[] LoadAssetBundle(string bundleName)
    {
        if(bundleName == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return null;
        }
        AssetBundle localAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));
        if (localAssetBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return null;
        }
        return localAssetBundle.LoadAllAssets<Texture2D>();


    }
}