using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace BTools.Management.EditorScripts
{
    public static class BulkToolsConfig
    {
        [SettingsProvider]
        public static SettingsProvider CreateBulkToolsSettingsProvider()
        {
            var provider = new SettingsProvider("Project/BulkTools", SettingsScope.Project)
            {
                label = "Bulk Tools",
                guiHandler = (searchContext) =>
                {
                    BulkToolsManagementWindow.DrawGUI();
                }
            };

            return provider;
        }


        public static List<ModuleConfig> LoadSettings() 
        {
            string path = Application.dataPath.Remove(Application.dataPath.LastIndexOf('/'));
            path += "/ProjectSettings";
            if (!Directory.Exists(path)) 
            {
                return new List<ModuleConfig>();
            }
            path += "/BulkToolsConfig.json";
            if (!File.Exists(path)) 
            {
                return new List<ModuleConfig>();
            }

            string json = File.ReadAllText(path);
            try
            {
                StoredSettings stored = JsonUtility.FromJson<StoredSettings>(json);
                return stored.modules;
            }
            catch 
            {
                return new List<ModuleConfig>();
            }
        }

        public static void SaveSettings(List<ModuleConfig> config) 
        {
            string path = Application.dataPath.Remove(Application.dataPath.LastIndexOf('/'));
            path += "/ProjectSettings";
            if (!Directory.Exists(path))
            {
                Debug.LogError("ProjectSettings folder missing. Something went wrong.");
            }
            path += "/BulkToolsConfig.json";

            string json = JsonUtility.ToJson(new StoredSettings() { modules = config }, prettyPrint: true);

            File.WriteAllText(path, json);
        }
    }

    [System.Serializable]
    public class StoredSettings 
    {
        public List<ModuleConfig> modules = new List<ModuleConfig>();
    }

    [System.Serializable]
    public struct ModuleConfig 
    {
        public string moduleName;
        public bool moduleEnabled;
        public List<string> enabledPlatforms;
    }
}