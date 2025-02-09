using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BTools.BTags.Data;
using BTools.BTags;

namespace BTools.BTags.EditorScripts
{
    public class TagListWindow : EditorWindow
    {
        private static TagListAsset cachedTagList;
        private static Editor tagListEditor;

        public static void InitWindow(TagListAsset tagList)
        {
            TagListWindow window = (TagListWindow)EditorWindow.GetWindow(typeof (TagListWindow));
            window.titleContent = new GUIContent("Tag List");
            SetTagList(tagList);
        }

        public static void SetTagList(TagListAsset tagList) 
        {
            cachedTagList = tagList;
            SetInspector();
        }

        [SettingsProvider]
        public static SettingsProvider CreateTagListSettingsProvider() 
        {
            var provider = new SettingsProvider("Project/BulkTools/Better Tags", SettingsScope.Project)
            {
                label = "Better Tags: Tag List",
                guiHandler = (searchContext) =>
                {
                    DrawGUI(false);
                }
            };

            return provider;
        }

        public void OnGUI() 
        {
            DrawGUI(true);
        }

        private static void DrawGUI(bool standaloneWindowMode) 
        {
            if (cachedTagList)
            {
                tagListEditor.OnInspectorGUI();
            }
            else
            {
                if (TagListEditor.OpenTagListCheck())
                {
                    if (standaloneWindowMode)
                    {
                        TagListEditor.OpenTagList();
                    }
                    else 
                    {
                        SetTagList(TagListEditor.LoadTagList());
                    }
                }
                else
                {
                    GUILayout.Label("Missing Tag List");
                    if (TagListEditor.CreateNewTagListCheck())
                    {
                        if (GUILayout.Button("Create new tag list")) TagListEditor.CreateNewTagList();
                    }
                }
            }
        }

        private static void SetInspector()
        {
            if (tagListEditor) { DestroyImmediate(tagListEditor); }
            if (cachedTagList) { tagListEditor = Editor.CreateEditor(cachedTagList); }
        }

        public void OnDisable() 
        {
            if (tagListEditor) { DestroyImmediate(tagListEditor); }
        }

    }
}
