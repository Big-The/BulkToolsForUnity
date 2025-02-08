using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public static class AssetReplacement
{
    //Asset Type, File Filters
    private static Dictionary<Type, string[]> assetFileFilters = new Dictionary<Type, string[]>() 
    {
        { 
            typeof(Texture2D),
            new string[]
            {
                "Image Files",
                "png,jpg,jpeg"
            }
        }
    };

    [MenuItem("Assets/Replace/Keep Old Name")]
    public static void ReplaceAsset(MenuCommand command)
    {
        TryReplaceAsset(Selection.activeObject.GetType(), AssetDatabase.GetAssetPath(Selection.activeObject), true);
    }

    [MenuItem("Assets/Replace/Use New Name")]
    public static void ReplaceAssetNewName(MenuCommand command)
    {
        TryReplaceAsset(Selection.activeObject.GetType(), AssetDatabase.GetAssetPath(Selection.activeObject), false);
    }

    [MenuItem("Assets/Replace/Keep Old Name", validate = true)]
    [MenuItem("Assets/Replace/Use New Name", validate = true)]
    public static bool ReplaceAssetValidate()
    {
        return Selection.count == 1 && Selection.count == Selection.assetGUIDs.Length && AssetDatabase.IsMainAsset(Selection.activeObject) && assetFileFilters.ContainsKey(Selection.activeObject.GetType());
    }

    //TODO: Currently only replaces the bytes of the asset when in keepOldName mode (Unity is fine with this?). Should be changing the file extension as well in this case.
    //TODO: Add support for doing this in mass? Not sure how I'll do that yet but I would like it to be possible somehow.
    public static void TryReplaceAsset(Type assetType, string assetPath, bool keepOldName) 
    {
        if(!assetFileFilters.ContainsKey(assetType)) 
        {
            return;
        }

        string projectRoot = Application.dataPath.Remove(Application.dataPath.LastIndexOf('/'));
        string realAssetPath = projectRoot + "/" + assetPath;

        string replacementPath = EditorUtility.OpenFilePanelWithFilters("Pick a file to replace the target with.", projectRoot, assetFileFilters[assetType]);

        if(replacementPath == null || replacementPath == string.Empty) { return; }

        if (keepOldName)
        {
            File.WriteAllBytes(realAssetPath, File.ReadAllBytes(replacementPath));
        }
        else 
        {
            string assetHomePath = realAssetPath.Remove(realAssetPath.LastIndexOf("/"));
            string realAssetMetaPath = realAssetPath + ".meta";

            string replacementFileName = replacementPath.Substring(replacementPath.LastIndexOf("/") + 1);
            string replacementAssetPath = assetHomePath + "/" + replacementFileName;
            string replacementMetaPath = assetHomePath + "/" + replacementFileName + ".meta";

            File.WriteAllBytes(replacementAssetPath, File.ReadAllBytes(replacementPath));
            File.WriteAllText(replacementMetaPath, File.ReadAllText(realAssetMetaPath));
            if (realAssetPath != replacementAssetPath) 
            {
                File.Delete(realAssetPath);
                File.Delete(realAssetMetaPath);
            }
        }

        AssetDatabase.Refresh();
    }
}
