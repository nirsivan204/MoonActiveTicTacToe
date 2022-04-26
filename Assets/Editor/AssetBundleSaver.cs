using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class AssetBundleSaver : MonoBehaviour
{

    public static void SaveAssetBundle(string assetBundleName, Texture2D XImage, Texture2D OImage, Texture2D backgroundImage)
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
        BuildAssetsBundle(buildMap);
    }

    private static void BuildAssetsBundle(AssetBundleBuild[] buildMap)
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, buildMap, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }

}
