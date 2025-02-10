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

        [Tooltip("Dependencies of other modules")]
        public List<string> internalDependencies = new List<string>();

        [System.Serializable]
        public struct ExternalPackage 
        {
            public string displayName;
            public string name;
            public string source;
        }

        [Tooltip("Dependencies of other packages")]
        public List<ExternalPackage> externalDependencies = new List<ExternalPackage>();


        [System.Serializable]
        public struct DefineSetting 
        {
            public string defineSymbol;
            [TextArea]
            public string description;
        }

        public List<DefineSetting> defineSettings = new List<DefineSetting>();

        public string GetModuleRootPath() 
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            return assetPath.Remove(assetPath.LastIndexOf('/'));
        }

        public bool experimental = false;
    }
}
