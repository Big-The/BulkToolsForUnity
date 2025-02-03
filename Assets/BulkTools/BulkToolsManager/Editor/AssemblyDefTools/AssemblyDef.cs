using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace BTools.Management.Editor
{
    [System.Serializable]
    public class AssemblyDef
    {
        public string name = "Default";
        public List<string> references = new List<string>();
        public List<string> includePlatforms = new List<string>();
        public List<string> excludePlatforms = new List<string>();
        public bool overrideReferences = false;
        public List<string> precompiledReferences = new List<string>();
        public bool autoReferenced = true;
        public List<string> defineConstraints = new List<string>();

        [System.Serializable]
        public struct VersionDefine 
        {
            public string name;
            public string expression;
            public string define;
        }
        public List<VersionDefine> versionDefines = new List<VersionDefine>();

        public bool noEngineReferences = false;

        public static List<string> DisabledExcludePlatforms 
        {
            get 
            {
                return new List<string>
                {
                    "Android",
                    "Editor",
                    "EmbeddedLinux",
                    "GameCoreScarlett",
                    "GameCoreXboxOne",
                    "iOS",
                    "LinuxStandalone64",
                    "CloudRendering",
                    "macOSStandalone",
                    "PS4",
                    "PS5",
                    "QNX",
                    "Stadia",
                    "Switch",
                    "tvOS",
                    "WSA",
                    "VisionOS",
                    "WebGL",
                    "WindowsStandalone32",
                    "WindowsStandalone64",
                    "XboxOne"
                };
            }
        }


        public static AssemblyDef Load(string path) 
        {
            string assemAsString = File.ReadAllText(path);
            return JsonUtility.FromJson<AssemblyDef>(assemAsString);
        }

        public static void Write(string path, AssemblyDef assembly) 
        {
            File.WriteAllText(path, GetAsJSONString(assembly));
        }

        public static string GetAsJSONString(AssemblyDef assemblyDef) 
        {
            return assemblyDef.GetAsJSONString();
        }

        public void Write(string path) 
        {
            Write(path, this);
        }

        public string GetAsJSONString(bool readable = false) 
        {
            return JsonUtility.ToJson(this, readable);
        }

        public override string ToString()
        {
            return $"AssemblyDef:\n{GetAsJSONString(readable: true)}";
        }
    }
}
//Possible platforms
//"includePlatforms": [
//        "Android",
//        "Editor",
//        "EmbeddedLinux",
//        "GameCoreScarlett",
//        "GameCoreXboxOne",
//        "iOS",
//        "LinuxStandalone64",
//        "CloudRendering",
//        "macOSStandalone",
//        "PS4",
//        "PS5",
//        "QNX",
//        "Stadia",
//        "Switch",
//        "tvOS",
//        "WSA",
//        "VisionOS",
//        "WebGL",
//        "WindowsStandalone32",
//        "WindowsStandalone64",
//        "XboxOne"
//    ],