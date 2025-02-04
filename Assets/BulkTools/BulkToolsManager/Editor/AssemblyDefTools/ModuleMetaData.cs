using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BTools.Management.EditorScripts
{
    public class ModuleMetaData : ScriptableObject
    {
        public string moduleName = "Module";
        [TextArea]
        public string description = "";
        public string normalAssembly = "";
        public string editorAssembly = "";
        public List<string> supportedPlatforms = new List<string>();

        public List<string> dependencies = new List<string>();

        public string GetModuleRootPath() 
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            return assetPath.Remove(assetPath.LastIndexOf('/'));
        }

        public bool experimental = false;
    }
}
