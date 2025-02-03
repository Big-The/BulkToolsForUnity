using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

namespace BTools.Management.Editor
{
    public class BulkToolsManagementWindow : EditorWindow
    {
        public const string packageName = "com.bigthe.bulktools";
        public const string packageRootFolder = "com.bigthe.bulktools";
        public const string assetsRootFolder = "BulkTools";

        public static bool InPackageMode { get; private set; }
        private static bool ready = false;

        private static List<ModuleMetaData> modules = new List<ModuleMetaData>();

        private static List<bool> modulesShown = new List<bool>();
        private static List<bool> moduleEnabled = new List<bool>();
        private static List<bool> platformsShown = new List<bool>();
        private static List<List<bool>> enabledPlatforms = new List<List<bool>>();

        private Vector2 scrollPos = Vector2.zero;

        [MenuItem("BulkTools/Management Window", priority = 0)]
        public static void OpenManagementWindow()
        {
            Debug.Log("Opening Management Window");
            BulkToolsManagementWindow window = (BulkToolsManagementWindow)GetWindow(typeof(BulkToolsManagementWindow));
            window.titleContent = new GUIContent("Bulk Tools Module Manager");
        }

        public void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.LabelField("This is the module manager. Use this window to pick what modules you want enabled and on what platform.", EditorStyles.wordWrappedLabel);
            EditorGUILayout.Space(5);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!ready);
            if (GUILayout.Button("Apply", EditorStyles.miniButtonLeft, GUILayout.Width(100)))
            {
                ApplyEnabledStates();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Revert/Refresh", EditorStyles.miniButtonRight, GUILayout.Width(100)))
            {
                RevertRefresh();
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space(5);

            if (!ready)
            {
                DrawLine();
                GUILayout.Label("Loading");
                DrawLine();
                EditorGUILayout.EndScrollView();
                return;
            }

            DrawLine();
            for (int i = 0; i < modules.Count; i++)
            {
                DrawModuleGUI(i);
                DrawLine();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawLine() 
        {
            EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Height(1)), Color.grey);
        }

        private void DrawModuleGUI(int moduleIndex) 
        {
            var module = modules[moduleIndex];

            GUIContent titleContent = null;
            if (module.experimental)
            {
                titleContent = new GUIContent(module.moduleName + " (Experimental)", "Experimental modules should not be expected to always work correctly on all platforms and/or may be missing features.\n\n" + module.description);
            }
            else 
            {
                titleContent = new GUIContent(module.moduleName, module.description);
            }

            modulesShown[moduleIndex] = EditorGUILayout.Foldout(modulesShown[moduleIndex], titleContent);
            EditorGUI.indentLevel++;
            if (!modulesShown[moduleIndex]) 
            {
                EditorGUI.indentLevel--;
                return;
            }

            EditorGUILayout.LabelField(module.description, EditorStyles.wordWrappedLabel);

            bool dependedOn = CheckDependedOn(moduleIndex, out List<string> platformsDependedOn);

            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(dependedOn);
            moduleEnabled[moduleIndex] = EditorGUILayout.Toggle("Enabled in Project", moduleEnabled[moduleIndex]);
            EditorGUI.EndDisabledGroup();
            platformsShown[moduleIndex] = EditorGUILayout.Foldout(platformsShown[moduleIndex], new GUIContent("Include on Platforms:", "Platforms that this module should be included on."));
            EditorGUI.BeginDisabledGroup(!moduleEnabled[moduleIndex]);
            if (platformsShown[moduleIndex]) 
            {
                EditorGUI.indentLevel++;
           
                for(int i = 0; i < enabledPlatforms[moduleIndex].Count; i++) 
                {
                    EditorGUI.BeginDisabledGroup(platformsDependedOn.Contains(module.supportedPlatforms[i]));
                    enabledPlatforms[moduleIndex][i] = EditorGUILayout.Toggle(new GUIContent(module.supportedPlatforms[i], " " + module.supportedPlatforms[i] + " "), enabledPlatforms[moduleIndex][i]);
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();

            if (EditorGUI.EndChangeCheck() && moduleEnabled[moduleIndex]) 
            {
                UpdateDependencies(moduleIndex);
            }

            if(module.dependencies.Count > 0) 
            {
                EditorGUILayout.LabelField("Module Dependencies");
                foreach (var dependency in module.dependencies) 
                {
                    EditorGUILayout.LabelField(" - " + dependency);
                }
            }

            EditorGUI.indentLevel--;
        }

        private bool CheckDependedOn(int moduleIndex, out List<string> platformsRequired) 
        {
            bool dependedOn = false;
            platformsRequired = new List<string>();
            string moduleName = modules[moduleIndex].name;
            for(int i = 0; i < modules.Count; i++) 
            {
                if (moduleEnabled[i]) 
                {
                    if (modules[i].dependencies.Contains(moduleName)) 
                    {
                        dependedOn = true;
                        for (int platformIndex = 0; platformIndex < modules[i].supportedPlatforms.Count; platformIndex++)
                        {
                            if (enabledPlatforms[i][platformIndex] && !platformsRequired.Contains(modules[i].supportedPlatforms[platformIndex]))
                            {
                                platformsRequired.Add(modules[i].supportedPlatforms[platformIndex]);
                            }
                        }
                    }
                }
            }
            return dependedOn;
        }

        private void UpdateDependencies(int moduleIndex) 
        {
            if (!moduleEnabled[moduleIndex]) { return; }
            foreach (var dependency in modules[moduleIndex].dependencies) 
            {
                for(int dependencyIndex = 0; dependencyIndex < modules.Count; dependencyIndex++) 
                {
                    if (modules[dependencyIndex].name == dependency) 
                    {
                        moduleEnabled[dependencyIndex] = true;
                        for(int platformIndex = 0; platformIndex < modules[moduleIndex].supportedPlatforms.Count; platformIndex++) 
                        {
                            if (enabledPlatforms[moduleIndex][platformIndex]) 
                            {
                                enabledPlatforms[dependencyIndex][modules[dependencyIndex].supportedPlatforms.IndexOf(modules[moduleIndex].supportedPlatforms[platformIndex])] = true;
                            }
                        }
                        UpdateDependencies(dependencyIndex);
                    }
                }
            }
        }

        private void RevertRefresh() 
        {
            ready = false;
            EditorApplication.delayCall += UpdatePackageMode;
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void UpdatePackageMode() 
        {
            ready = false;
            if (EditorApplication.isCompiling || EditorApplication.isUpdating) 
            {
                EditorApplication.delayCall += UpdatePackageMode;
                return;
            }
            var packList = Client.List();
            while (!packList.IsCompleted) { }
            InPackageMode = false;
            foreach (var pack in packList.Result) 
            {
                if(pack.name == packageName) 
                {
                    InPackageMode = true;
                    break;
                }
            }
            PostCompileReload();
        }

        private static void PostCompileReload() 
        {
            LoadAllPossibleAssemblyTargets();
            GetEnabledStates();

            ready = true;
        }

        private static void LoadAllPossibleAssemblyTargets()
        {
            modules = new List<ModuleMetaData>();
            string searchPath = (InPackageMode ? "Packages" : "Assets");
            searchPath = Path.Combine(searchPath, InPackageMode ? packageRootFolder : assetsRootFolder);
            searchPath.Replace(Path.PathSeparator, '/');
            string finalSearchPath = "";
            for (int i = 0; i < searchPath.Length; i++) 
            {
                if (searchPath[i] == '\\')
                {
                    finalSearchPath += '/';
                }
                else 
                {
                    finalSearchPath += searchPath[i];
                }

            }
            foreach (var moduleGUID in AssetDatabase.FindAssets("t:moduleMetaData", new string[] { finalSearchPath }))
            {
                modules.Add(AssetDatabase.LoadAssetAtPath<ModuleMetaData>(AssetDatabase.GUIDToAssetPath(moduleGUID)));
            }
            modulesShown = new List<bool>(modules.Count);
            for(int i = 0; i < modules.Count; i++) 
            {
                modulesShown.Add(true);
                platformsShown.Add(false);
            }
        }

        private static void GetEnabledStates()
        {
            moduleEnabled = new List<bool>(modules.Count);
            foreach (var module in modules)
            {
                string rootPath = module.GetModuleRootPath();
                bool enabledInEditor = false;
                if (module.editorAssembly != "")
                {
                    string editorAssemPath = Path.Combine(rootPath, module.editorAssembly);
                    var editorAssem = AssemblyDef.Load(Path.Combine(Application.dataPath.Remove(Application.dataPath.LastIndexOf('/')), editorAssemPath));
                    if (editorAssem.includePlatforms.Contains("Editor"))
                    {
                        enabledInEditor = true;
                    }
                }

                List<bool> supportedPlatformsEnabledStates = new List<bool>();
                if (module.normalAssembly != "")
                {
                    string normalAssemPath = Path.Combine(rootPath, module.normalAssembly);
                    var normalAssem = AssemblyDef.Load(Path.Combine(Application.dataPath.Remove(Application.dataPath.LastIndexOf('/')), normalAssemPath));
                    if (normalAssem.includePlatforms.Contains("Editor"))
                    {
                        enabledInEditor = true;
                    }
                    foreach (string supportedPlatform in module.supportedPlatforms)
                    {
                        supportedPlatformsEnabledStates.Add(normalAssem.includePlatforms.Contains(supportedPlatform));
                    }
                }

                enabledPlatforms.Add(supportedPlatformsEnabledStates);
                moduleEnabled.Add(enabledInEditor);
            }
        }

        private static void ApplyEnabledStates()
        {
            for (int i = 0; i < modules.Count; i++) 
            {
                var module = modules[i];
                string rootPath = module.GetModuleRootPath();
                if (module.editorAssembly != "")
                {
                    string editorAssemPath = Path.Combine(rootPath, module.editorAssembly);
                    string finalEditorAssemPath = Path.Combine(Application.dataPath.Remove(Application.dataPath.LastIndexOf('/')), editorAssemPath);
                    var editorAssem = AssemblyDef.Load(finalEditorAssemPath);
                    if (!(editorAssem.includePlatforms.Count == 1 && editorAssem.includePlatforms.Contains("Editor")) && moduleEnabled[i])
                    {
                        editorAssem.includePlatforms = new List<string> { "Editor" };
                        editorAssem.excludePlatforms = new List<string>();
                        editorAssem.Write(finalEditorAssemPath);
                    }
                    else if(editorAssem.includePlatforms.Count > 0 && !moduleEnabled[i])
                    {
                        editorAssem.includePlatforms = new List<string>();
                        editorAssem.excludePlatforms = AssemblyDef.DisabledExcludePlatforms;
                        editorAssem.Write(finalEditorAssemPath);
                    }
                }

                if (module.normalAssembly != "")
                {
                    string normalAssemPath = Path.Combine(rootPath, module.normalAssembly);
                    string finalNormalAssemPath = Path.Combine(Application.dataPath.Remove(Application.dataPath.LastIndexOf('/')), normalAssemPath);
                    var normalAssem = AssemblyDef.Load(finalNormalAssemPath);
                    List<string> newTargetPlatforms = new List<string>();
                    if (moduleEnabled[i])
                    {
                        newTargetPlatforms.Add("Editor");
                        for (int j = 0; j < module.supportedPlatforms.Count; j++) 
                        {
                            if (enabledPlatforms[i][j]) 
                            {
                                newTargetPlatforms.Add(module.supportedPlatforms[j]);
                            }   
                        }
                    }
                    bool modified = false;
                    if (newTargetPlatforms.Count != normalAssem.includePlatforms.Count)
                    {
                        normalAssem.includePlatforms = newTargetPlatforms;
                        modified = true;
                    }
                    else 
                    {
                        
                        for(int j = normalAssem.includePlatforms.Count - 1; j >= 0; j--) 
                        {
                            if (!newTargetPlatforms.Contains(normalAssem.includePlatforms[j])) 
                            {
                                normalAssem.includePlatforms.RemoveAt(j);
                                modified = true;
                            }
                        }
                        for (int j = 0; j < newTargetPlatforms.Count; j++) 
                        {
                            if (!normalAssem.includePlatforms.Contains(newTargetPlatforms[j]))
                            {
                                normalAssem.includePlatforms.Add(newTargetPlatforms[j]);
                                modified = true;    
                            }
                        }
                    }
                    if (moduleEnabled[i] && normalAssem.excludePlatforms.Count > 0)
                    {
                        normalAssem.excludePlatforms = new List<string>();
                        modified = true;
                    }
                    else if(!moduleEnabled[i] && normalAssem.excludePlatforms.Count == 0)
                    {
                        normalAssem.excludePlatforms = AssemblyDef.DisabledExcludePlatforms;
                        modified = true;
                    }
                    if (modified)
                    {
                        
                        normalAssem.Write(finalNormalAssemPath);
                    }
                }
            }
            AssetDatabase.Refresh();
        }
    }
}
