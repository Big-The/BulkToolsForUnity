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

        //Cached module and window state data
        private static List<ModuleMetaData> modules = new List<ModuleMetaData>();
        private static List<bool> modulesShown = new List<bool>();
        private static List<bool> moduleEnabled = new List<bool>();
        private static List<bool> platformsShown = new List<bool>();
        private static List<List<bool>> enabledPlatforms = new List<List<bool>>();
        private Vector2 scrollPos = Vector2.zero;


        [MenuItem("BulkTools/Module Manager", priority = 0)]
        public static void OpenManagementWindow()
        {
            BulkToolsManagementWindow window = (BulkToolsManagementWindow)GetWindow(typeof(BulkToolsManagementWindow));
            window.titleContent = new GUIContent("Bulk Tools Module Manager");
        }

        public void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            //Window description
            EditorGUILayout.LabelField("This is the module manager. Use this window to pick what modules you want enabled and on what platform.", EditorStyles.wordWrappedLabel);

            ApplyRevertButtons();

            if (!ready)
            {
                DrawLine(horizontal: true);
                GUILayout.Label("Loading");
                DrawLine(horizontal: true);
            }
            else
            {
                DrawLine(horizontal: true);
                for (int i = 0; i < modules.Count; i++)
                {
                    DrawModuleGUI(i);
                    DrawLine(horizontal: true);
                }
            }

            ApplyRevertButtons();
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draws the apply and revert/refresh buttons in a horizontal group.
        /// </summary>
        private void ApplyRevertButtons() 
        {
            EditorGUILayout.Space(5);
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!ready);//Disable the buttons if not ready
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
        }

        /// <summary>
        /// Draws a 1 unit seprator line
        /// </summary>
        private void DrawLine(bool horizontal = true) 
        {
            if(horizontal) 
            {
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Height(1)), Color.grey);
            }
            else 
            {
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Width(1)), Color.grey);
            }
        }

        private void DrawModuleGUI(int moduleIndex) 
        {
            var module = modules[moduleIndex];

            //Create title GUIContent. Adds experimental tag, and disclaimer in tool tip.
            GUIContent titleContent = null;
            if (module.experimental)
            {
                titleContent = new GUIContent(module.moduleName + " (Experimental)", "Experimental modules should not be expected to always work correctly on all platforms and/or may be missing features.\n\n" + module.description);
            }
            else 
            {
                titleContent = new GUIContent(module.moduleName, module.description);
            }

            //Module Title foldout. Exit early if foldout closed
            modulesShown[moduleIndex] = EditorGUILayout.Foldout(modulesShown[moduleIndex], titleContent);
            EditorGUI.indentLevel++;
            if (!modulesShown[moduleIndex]) 
            {
                EditorGUI.indentLevel--;
                return;
            }

            //Module description
            EditorGUILayout.LabelField(module.description, EditorStyles.wordWrappedLabel);

            bool dependedOn = CheckDependedOn(moduleIndex, out List<string> platformsDependedOn);

            EditorGUI.BeginChangeCheck();

            //Module enabled in project state:
            EditorGUI.BeginDisabledGroup(dependedOn);//If this module is depended on it can't be disabled
            moduleEnabled[moduleIndex] = EditorGUILayout.Toggle("Enabled in Project", moduleEnabled[moduleIndex]);
            EditorGUI.EndDisabledGroup();

            //Platform specific enabled states:
            platformsShown[moduleIndex] = EditorGUILayout.Foldout(platformsShown[moduleIndex], new GUIContent("Include on Platforms:", "Platforms that this module should be included on."));
            EditorGUI.BeginDisabledGroup(!moduleEnabled[moduleIndex]);//If the whole module is disabled it's platforms can't be modified
            if (platformsShown[moduleIndex]) 
            {
                EditorGUI.indentLevel++;
           
                for(int i = 0; i < enabledPlatforms[moduleIndex].Count; i++) 
                {
                    EditorGUI.BeginDisabledGroup(platformsDependedOn.Contains(module.supportedPlatforms[i]));//If this module is depended on on this platform it can't be changed here.
                    enabledPlatforms[moduleIndex][i] = EditorGUILayout.Toggle(new GUIContent(module.supportedPlatforms[i], " " + module.supportedPlatforms[i] + " "), enabledPlatforms[moduleIndex][i]);
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();

            //Update dependencies if this module has been updated
            if (EditorGUI.EndChangeCheck() && moduleEnabled[moduleIndex]) 
            {
                UpdateDependencies(moduleIndex);
            }
            
            //Show this module's dependencies if there are any
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

        /// <summary>
        /// Get depended on requirements. (If this module is depended on at all and what platforms it's required on)
        /// </summary>
        /// <param name="moduleIndex"></param>
        /// <param name="platformsRequired"></param>
        /// <returns></returns>
        private bool CheckDependedOn(int moduleIndex, out List<string> platformsRequired) 
        {
            bool dependedOn = false;
            platformsRequired = new List<string>();
            string moduleName = modules[moduleIndex].name;
            for(int otherModuleIndex = 0; otherModuleIndex < modules.Count; otherModuleIndex++) //Loop through the other modules
            {
                //If the other module is enabled and has the current module as a dependency
                if (moduleEnabled[otherModuleIndex] && modules[otherModuleIndex].dependencies.Contains(moduleName)) 
                {
                    dependedOn = true;
                    for (int platformIndex = 0; platformIndex < modules[otherModuleIndex].supportedPlatforms.Count; platformIndex++)//Loop through the other module's platform states
                    {
                        //If the other module has a platform required that isn't already tracked by platformsRequired
                        if (enabledPlatforms[otherModuleIndex][platformIndex] && !platformsRequired.Contains(modules[otherModuleIndex].supportedPlatforms[platformIndex]))
                        {
                            platformsRequired.Add(modules[otherModuleIndex].supportedPlatforms[platformIndex]);//Add the platform to the platformsRequired list
                        }
                    }
                }
            }
            return dependedOn;
        }

        /// <summary>
        /// Updates the states of all dependencies recursively.
        /// </summary>
        /// <param name="moduleIndex"></param>
        private void UpdateDependencies(int moduleIndex) 
        {
            if (!moduleEnabled[moduleIndex]) { return; } //If this module isn't enabled then it's dependencies don't matter exit early
            foreach (var dependency in modules[moduleIndex].dependencies) //Loop through the dependency names
            {
                for(int dependencyIndex = 0; dependencyIndex < modules.Count; dependencyIndex++) //Loop through all modules
                {
                    if (modules[dependencyIndex].name == dependency) //If the names match this module is a dependency
                    {
                        moduleEnabled[dependencyIndex] = true;//Force this module enabled

                        //loop over platforms and force any required platforms to enabled
                        for (int platformIndex = 0; platformIndex < modules[moduleIndex].supportedPlatforms.Count; platformIndex++) 
                        {
                            if (enabledPlatforms[moduleIndex][platformIndex]) 
                            {
                                enabledPlatforms[dependencyIndex][modules[dependencyIndex].supportedPlatforms.IndexOf(modules[moduleIndex].supportedPlatforms[platformIndex])] = true;
                            }
                        }

                        //Call this recursively until there are no more dependencies.
                        UpdateDependencies(dependencyIndex);
                    }
                }
            }
        }

        /// <summary>
        /// Reverts current settings to what is currently configured in the assemdefs by reinitializing the cache
        /// </summary>
        private void RevertRefresh() 
        {
            ready = false;
            EditorApplication.delayCall += UpdatePackageMode;
        }

        /// <summary>
        /// Updates InPackageMode based on if the package is loaded as a package or asset (In Packages or Assets folder)
        /// </summary>
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

        /// <summary>
        /// Initializes module meta and module assemdef settings caches
        /// </summary>
        private static void PostCompileReload() 
        {
            LoadAllPossibleAssemblyTargets();
            GetEnabledStates();

            ready = true;
        }

        /// <summary>
        /// Loads all module metas and their structure into the cache
        /// </summary>
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
                modulesShown.Add(false);
                platformsShown.Add(false);
            }
        }

        /// <summary>
        /// Coppies the current assemdef configurations to the settings cache
        /// </summary>
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

        /// <summary>
        /// Applys current settings to the assembly definitions
        /// </summary>
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
