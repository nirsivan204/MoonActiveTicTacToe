using UnityEditor;
using UnityEngine;
using System.IO;
public class CustomEditorWindow : EditorWindow
{
    string assetBundleName = "My Asset Bundle";
    Texture2D backgroundImage;
    Texture2D XImage;
    Texture2D OImage;

    [MenuItem("Window/AssetBundle")]
    public static void ShowWindow()
    {
        GetWindow<CustomEditorWindow>("AssetBundle");
    }
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Assets Bundle Name: ");
        assetBundleName = EditorGUILayout.TextField(assetBundleName);
        GUILayout.EndHorizontal();
        XImage = (Texture2D)EditorGUILayout.ObjectField("XImage", XImage, typeof(Texture2D), false);
        OImage = (Texture2D)EditorGUILayout.ObjectField("OImage", OImage, typeof(Texture2D), false);
        backgroundImage = (Texture2D)EditorGUILayout.ObjectField("Background Image", backgroundImage, typeof(Texture2D), false);

        if (GUILayout.Button("Create Asset Bundle"))
        {
            AssetBundleSaver.SaveAssetBundle(assetBundleName, XImage,OImage,backgroundImage);
        }
    }
}
