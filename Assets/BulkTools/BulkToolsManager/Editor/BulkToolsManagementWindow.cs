using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.PackageManager;

namespace BTools.Management.EditorScripts
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
        private static List<bool> defineSettingsShown = new List<bool>();
        private static List<List<bool>> enabledPlatforms = new List<List<bool>>();
        private static List<List<bool>> enabledDefineSettings = new List<List<bool>>();
        private static Vector2 scrollPos = Vector2.zero;
        private static List<string> installedPackages = new List<string>();


        [MenuItem("BulkTools/Module Manager", priority = 0)]
        public static void OpenManagementWindow()
        {
            BulkToolsManagementWindow window = (BulkToolsManagementWindow)GetWindow(typeof(BulkToolsManagementWindow));
            window.titleContent = new GUIContent("Bulk Tools Module Manager");
        }

        public void OnGUI()
        {
            DrawGUI();
        }

        public static void DrawGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.Space();
            //Window description
            EditorGUILayout.LabelField("This is the module manager. Use this window to pick what modules you want enabled and on what platform.", EditorStyles.wordWrappedLabel);

            EditorGUILayout.LabelField("Enabled Modules", EditorStyles.boldLabel);
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
        private static void ApplyRevertButtons()
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
        private static void DrawLine(bool horizontal = true)
        {
            if (horizontal)
            {
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Height(1)), Color.grey);
            }
            else
            {
                EditorGUI.DrawRect(EditorGUILayout.GetControlRect(GUILayout.Width(1)), Color.grey);
            }
        }

        private static void DrawModuleGUI(int moduleIndex)
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
            modulesShown[moduleIndex] = EditorGUILayout.Foldout(modulesShown[moduleIndex], titleContent, true);
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
            if (module.normalAssembly.Length > 0 || module.editorAssembly.Length > 0)//If the module has any assembly attactched
            {
                EditorGUI.BeginDisabledGroup(dependedOn);//If this module is depended on it can't be disabled
                var content = new GUIContent("Enabled in Project", dependedOn ? "Module is depended on, and can't be disabled." : "");
                moduleEnabled[moduleIndex] = EditorGUILayout.Toggle(content, moduleEnabled[moduleIndex]);
                EditorGUI.EndDisabledGroup();
            }
            else //The module has no assembly attached and can't be disabled because of it
            {
                EditorGUI.BeginDisabledGroup(true);
                var content = new GUIContent("Enabled in Project", "Module has no assembly defined, and can't be disabled.");
                EditorGUILayout.Toggle(content, true);
                EditorGUI.EndDisabledGroup();
            }

            //Platform specific enabled states:
            if (module.normalAssembly.Length > 0)
            {
                platformsShown[moduleIndex] = EditorGUILayout.Foldout(platformsShown[moduleIndex], new GUIContent("Include on Platforms:", "Platforms that this module should be included on."));
                EditorGUI.BeginDisabledGroup(!moduleEnabled[moduleIndex]);//If the whole module is disabled it's platforms can't be modified
                if (platformsShown[moduleIndex])
                {
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < enabledPlatforms[moduleIndex].Count; i++)
                    {
                        EditorGUI.BeginDisabledGroup(platformsDependedOn.Contains(module.supportedPlatforms[i]));//If this module is depended on on this platform it can't be changed here.
                        enabledPlatforms[moduleIndex][i] = EditorGUILayout.Toggle(new GUIContent(module.supportedPlatforms[i], " " + module.supportedPlatforms[i] + " "), enabledPlatforms[moduleIndex][i]);
                        EditorGUI.EndDisabledGroup();
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUILayout.LabelField("Module does not include a runtime assembly, target platforms are irrelevant.");
            }

            //Define Settings
            if (module.defineSettings.Count > 0 && (module.normalAssembly.Length > 0 || module.editorAssembly.Length > 0))
            {
                defineSettingsShown[moduleIndex] = EditorGUILayout.Foldout(defineSettingsShown[moduleIndex], new GUIContent("Global Settings:"));
                if (defineSettingsShown[moduleIndex])
                {
                    EditorGUI.indentLevel++;
                    for (int defineIndex = 0; defineIndex < module.defineSettings.Count; defineIndex++)
                    {
                        enabledDefineSettings[moduleIndex][defineIndex] = EditorGUILayout.Toggle(new GUIContent(module.defineSettings[defineIndex], " " + module.defineSettings[defineIndex] + " "), enabledDefineSettings[moduleIndex][defineIndex]);
                    }
                    EditorGUI.indentLevel--;
                }
            }

            //Update dependencies if this module has been updated
            if (EditorGUI.EndChangeCheck() && moduleEnabled[moduleIndex])
            {
                UpdateDependencies(moduleIndex);
            }

            //Show this module's dependencies if there are any
            if (module.internalDependencies.Count > 0 || module.externalDependencies.Count > 0)
            {
                EditorGUILayout.LabelField("Dependencies");
                foreach (var dependency in module.internalDependencies)
                {
                    EditorGUILayout.LabelField(" - " + dependency);
                }
                foreach (var dependency in module.externalDependencies)
                {
                    EditorGUILayout.LabelField(" - " + dependency.displayName + "(" + dependency.name + ")");
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
        private static bool CheckDependedOn(int moduleIndex, out List<string> platformsRequired)
        {
            bool dependedOn = false;
            platformsRequired = new List<string>();
            string moduleName = modules[moduleIndex].name;
            for (int otherModuleIndex = 0; otherModuleIndex < modules.Count; otherModuleIndex++) //Loop through the other modules
            {
                //If the other module is enabled and has the current module as a dependency
                if (moduleEnabled[otherModuleIndex] && modules[otherModuleIndex].internalDependencies.Contains(moduleName))
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
        private static void UpdateDependencies(int moduleIndex)
        {
            if (!moduleEnabled[moduleIndex]) { return; } //If this module isn't enabled then it's dependencies don't matter exit early
            foreach (var dependency in modules[moduleIndex].internalDependencies) //Loop through the dependency names
            {
                for (int dependencyIndex = 0; dependencyIndex < modules.Count; dependencyIndex++) //Loop through all modules
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
        private static void RevertRefresh()
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
                if (pack.name == packageName)
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
            LoadDefines();
            CompareToProjectSettings();
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
            for (int i = 0; i < modules.Count; i++)
            {
                modulesShown.Add(false);
                platformsShown.Add(false);
                defineSettingsShown.Add(false);
            }
        }

        /// <summary>
        /// Coppies the current assemdef configurations to the settings cache
        /// </summary>
        private static void GetEnabledStates()
        {
            moduleEnabled = new List<bool>(modules.Count);
            enabledDefineSettings = new List<List<bool>>(modules.Count);
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


                //Define Settings:
                var moduleDefines = new List<bool>();

                foreach (var defineName in module.defineSettings)
                {
                    moduleDefines.Add(false);
                }

                enabledDefineSettings.Add(moduleDefines);
            }
        }

        private static void CompareToProjectSettings()
        {
            var settings = BulkToolsConfig.LoadSettings();
            foreach (var moduleConfig in settings)
            {
                for (int moduleIndex = 0; moduleIndex < modules.Count; moduleIndex++)
                {
                    if (modules[moduleIndex].name == moduleConfig.moduleName)
                    {
                        moduleEnabled[moduleIndex] = moduleConfig.moduleEnabled;
                        for (int platformIndex = 0; platformIndex < modules[moduleIndex].supportedPlatforms.Count; platformIndex++)
                        {
                            enabledPlatforms[moduleIndex][platformIndex] = moduleConfig.enabledPlatforms.Contains(modules[moduleIndex].supportedPlatforms[platformIndex]);
                        }
                    }
                }
            }
            ApplyEnabledStates();
        }

        private static void ApplyToProjectSettings()
        {
            var settings = new List<ModuleConfig>();

            for (int moduleIndex = 0; moduleIndex < modules.Count; moduleIndex++)
            {
                ModuleConfig moduleConfig = new ModuleConfig()
                {
                    moduleName = modules[moduleIndex].name,
                    moduleEnabled = moduleEnabled[moduleIndex],
                    enabledPlatforms = new List<string>()
                };
                for (int platformIndex = 0; platformIndex < modules[moduleIndex].supportedPlatforms.Count; platformIndex++)
                {
                    if (enabledPlatforms[moduleIndex].Count > platformIndex && enabledPlatforms[moduleIndex][platformIndex])
                    {
                        moduleConfig.enabledPlatforms.Add(modules[moduleIndex].supportedPlatforms[platformIndex]);
                    }
                }
                settings.Add(moduleConfig);
            }
            BulkToolsConfig.SaveSettings(settings);
        }

        /// <summary>
        /// Applys current settings to all the assembly definitions
        /// </summary>
        private static void ApplyEnabledStates()
        {
            ConfirmExternalDependencies();
            for (int moduleIndex = 0; moduleIndex < modules.Count; moduleIndex++)
            {
                var module = modules[moduleIndex];
                string rootPath = module.GetModuleRootPath();
                if (module.editorAssembly != "")
                {
                    string editorAssemPath = Path.Combine(rootPath, module.editorAssembly);
                    string finalEditorAssemPath = Path.Combine(Application.dataPath.Remove(Application.dataPath.LastIndexOf('/')), editorAssemPath);
                    var editorAssem = AssemblyDef.Load(finalEditorAssemPath);
                    ApplyAssemblySettings(
                        assemPath: finalEditorAssemPath,
                        assembly: editorAssem,
                        moduleMeta: module,
                        isEditorOnly: true,
                        enabled: moduleEnabled[moduleIndex],
                        curEnabledPlatforms: enabledPlatforms[moduleIndex]
                        );
                }

                if (module.normalAssembly != "")
                {
                    string normalAssemPath = Path.Combine(rootPath, module.normalAssembly);
                    string finalNormalAssemPath = Path.Combine(Application.dataPath.Remove(Application.dataPath.LastIndexOf('/')), normalAssemPath);
                    var normalAssem = AssemblyDef.Load(finalNormalAssemPath);
                    ApplyAssemblySettings(
                        assemPath: finalNormalAssemPath,
                        assembly: normalAssem,
                        moduleMeta: module,
                        isEditorOnly: true,
                        enabled: moduleEnabled[moduleIndex],
                        curEnabledPlatforms: enabledPlatforms[moduleIndex]
                        );
                }
            }
            ApplyToProjectSettings();
            ApplyDefines();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Applys settings to a single assembly definition
        /// </summary>
        private static void ApplyAssemblySettings(string assemPath, AssemblyDef assembly, ModuleMetaData moduleMeta, bool isEditorOnly, bool enabled, List<bool> curEnabledPlatforms)
        {
            List<string> oldIncludedPlatforms = assembly.includePlatforms;
            List<string> oldExcludedPlatforms = assembly.excludePlatforms;

            if (enabled)
            {
                List<string> platformsToInclude = new List<string>();
                platformsToInclude.Add("Editor");
                if (!isEditorOnly)
                {
                    for (int platformIndex = 0; platformIndex < curEnabledPlatforms.Count; platformIndex++)
                    {
                        if (curEnabledPlatforms[platformIndex])
                        {
                            platformsToInclude.Add(moduleMeta.supportedPlatforms[platformIndex]);
                        }
                    }
                }
                assembly.excludePlatforms = new List<string>();
                assembly.includePlatforms = platformsToInclude;
            }
            else
            {
                assembly.includePlatforms = new List<string>();
                assembly.excludePlatforms = AssemblyDef.DisabledExcludePlatforms;
            }

            //Modifed check
            if (AreListsDifferent(assembly.includePlatforms, oldIncludedPlatforms) || AreListsDifferent(assembly.excludePlatforms, oldExcludedPlatforms))
            {
                assembly.Write(assemPath);
            }
        }

        private static bool AreListsDifferent(List<string> a, List<string> b)
        {
            if (a.Count != b.Count) { return true; }

            for (int i = 0; i < a.Count; i++)
            {
                if (a[i] != b[i])
                {
                    return true;
                }
            }

            return false;
        }


        private static void ConfirmExternalDependencies()
        {
            UpdateCachedPackageList();
            for (int moduleIndex = 0; moduleIndex < modules.Count; moduleIndex++)
            {
                if (!moduleEnabled[moduleIndex]) { continue; }
                var module = modules[moduleIndex];

                foreach (var packageDependency in module.externalDependencies)
                {
                    InstallPackage(packageDependency);
                }
            }
        }

        private static void UpdateCachedPackageList() 
        {
            var request = Client.List();
            while (!request.IsCompleted) { }
            installedPackages = new List<string>();
            foreach (var package in request.Result) 
            {
                installedPackages.Add(package.name);
            }
        }

        private static void InstallPackage(ModuleMetaData.ExternalPackage packageInfo) 
        {
            if (installedPackages.Contains(packageInfo.name)) { return; }
            var request = Client.Add(packageInfo.source);
            while (!request.IsCompleted) { }
            if (request.Status == StatusCode.Success)
            {
                Debug.Log("Package Installed:" + packageInfo.name);
            }
            else 
            {
                Debug.Log(request.Error.message);
            }
        }

        private static void LoadDefines() 
        {
            var curBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string curDefinesStr = PlayerSettings.GetScriptingDefineSymbols(curBuildTarget);
            List<string> defines = new List<string>(curDefinesStr.Split(';'));
            for (int moduleIndex = 0; moduleIndex < modules.Count; moduleIndex++)
            {
                for (int defineIndex = 0; defineIndex < modules[moduleIndex].defineSettings.Count; defineIndex++)
                {
                    enabledDefineSettings[moduleIndex][defineIndex] = defines.Contains(modules[moduleIndex].defineSettings[defineIndex]);
                }
            }
        }

        private static void ApplyDefines() 
        {
            var curBuildTarget = NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            string curDefinesStr = PlayerSettings.GetScriptingDefineSymbols(curBuildTarget);
            List<string> defines = new List<string>(curDefinesStr.Split(';'));
            bool modified = false;
            for (int moduleIndex = 0; moduleIndex < modules.Count; moduleIndex++)
            {
                for (int defineIndex = 0; defineIndex < modules[moduleIndex].defineSettings.Count; defineIndex++)
                {
                    if (defines.Contains(modules[moduleIndex].defineSettings[defineIndex]) && !enabledDefineSettings[moduleIndex][defineIndex])
                    {
                        defines.Remove(modules[moduleIndex].defineSettings[defineIndex]);
                        modified = true;
                    }
                    else if (!defines.Contains(modules[moduleIndex].defineSettings[defineIndex]) && enabledDefineSettings[moduleIndex][defineIndex])
                    {
                        defines.Add(modules[moduleIndex].defineSettings[defineIndex]);
                        modified = true;
                    }
                }
            }
            if (modified)
            {
                string newDefines = "";
                for (int i = 0; i < defines.Count; i++) 
                {
                    if (defines[i] == string.Empty) { continue; }
                    newDefines += defines[i];
                    if (i != defines.Count - 1) 
                    {
                        newDefines += ";";
                    }
                }
                PlayerSettings.SetScriptingDefineSymbols(curBuildTarget, newDefines);
            }
        }
    }
}
