using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PackageBuildingTools : MonoBehaviour
{
    [MenuItem("Package Building Tools/Create Module Meta Data")]
    public static void CreateModuleMetaData() 
    {
        string folderPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        if (folderPath.Contains("."))
        { 
            folderPath = folderPath.Remove(folderPath.LastIndexOf('/'));
        }
        string path = System.IO.Path.Combine(folderPath, "New Module Meta.asset");
        var newModule = new BTools.Management.EditorScripts.ModuleMetaData();
        newModule.supportedPlatforms = new List<string>
        {
            "Android",
            "iOS",
            "LinuxStandalone64",
            "PS4",
            "PS5",
            "Switch",
            "WebGL",
            "WindowsStandalone64",
            "XboxOne"
        };
        AssetDatabase.CreateAsset(newModule, path);
        
        Debug.Log("Created new module meta at:" + path);
    }
}
