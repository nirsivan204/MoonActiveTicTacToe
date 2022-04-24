using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public static class AssetBundleUtility
{

    public static void saveAssetBundle(string assetBundleName, Texture2D XImage, Texture2D OImage, Texture2D backgroundImage)
    {
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        buildMap[0].assetBundleName = assetBundleName;
        string[] imgAssets = new string[3];
        imgAssets[0] = AssetDatabase.GetAssetPath(XImage);
        imgAssets[1] = AssetDatabase.GetAssetPath(OImage);
        imgAssets[2] = AssetDatabase.GetAssetPath(backgroundImage);
        string[] addressableNames = new string[3];
        addressableNames[0] = "0";//"XImage";
        addressableNames[1] = "1";//"OImage";
        addressableNames[2] = "2";//"backgroundImage";
        buildMap[0].assetNames = imgAssets;
        buildMap[0].addressableNames = addressableNames;
        buildAssetsBundle(buildMap);
    }

    private static void buildAssetsBundle(AssetBundleBuild[] buildMap)
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildMap, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }

    public static Texture2D[] loadAssetBundle(string bundleName)
    {
        AssetBundle localAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));
        if (localAssetBundle == null)
        {
            Debug.LogError("Failed to load AssetBundle!");
            return null;
        }
        return localAssetBundle.LoadAllAssets<Texture2D>();


    }
}