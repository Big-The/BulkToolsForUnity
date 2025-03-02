using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BTools.SimpleEditorTools
{
    public class GameObjectArray : PopupWindowContent
    {
        private static Vector2 popupSize = new Vector2(200, 155);
        private static GameObject targetObject;
        private static bool open = false;
        private static bool applied = false;
        private Rect mainEditorWindow;

        private Vector3 arrayOffset = Vector3.zero;
        private Vector3 objectOffset = Vector3.one;
        private Vector3Int objectCount = Vector3Int.one;

        private Vector3 startingPosition = Vector3.zero;

        private List<GameObject> duplicates = new List<GameObject>();


        [MenuItem("GameObject/Simple Editor Tools/Array Duplicate Object")]
        public static void ArrayDuplicateObject(MenuCommand command) 
        {
            if (command.context != Selection.gameObjects[0]) { return; }
            targetObject = Selection.gameObjects[0];
            if (!open) 
            {
                PopupWindow.Show(new Rect(1, 1, 1, 1), new GameObjectArray());
            }
        }

        [MenuItem("GameObject/Simple Editor Tools/Array Duplicate Object", validate = true)]
        public static bool ArrayDuplicateObjectValidate() 
        {
            return Selection.gameObjects.Length == 1;
        }

        public override void OnGUI(Rect rect)
        {
            EditorGUIUtility.wideMode = true;
            EditorGUIUtility.labelWidth = 40;

            editorWindow.minSize = popupSize;
            editorWindow.maxSize = popupSize;
            editorWindow.position = new Rect(mainEditorWindow.x + 20,
                                            mainEditorWindow.y + mainEditorWindow.height - popupSize.y,
                                            popupSize.x,
                                            popupSize.y);

            EditorGUILayout.LabelField("Array Duplication", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Offsets:");
            objectOffset = EditorGUILayout.Vector3Field("Object", objectOffset);
            arrayOffset = EditorGUILayout.Vector3Field("Array", arrayOffset);
            EditorGUILayout.Space();

            objectCount = EditorGUILayout.Vector3IntField("Count", objectCount);
            Vector3IntMinLimiter(ref objectCount);
            EditorGUILayout.Space();

            if (EditorGUI.EndChangeCheck()) 
            {
                ApplyChanges();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply", GUILayout.Width(60)))
            {
                ApplyChanges();
                applied = true;
                editorWindow.Close();
            }
            if (GUILayout.Button("Cancel", GUILayout.Width(60)))
            {
                editorWindow.Close();
            }
            GUILayout.EndHorizontal();
            base.OnGUI(rect);
        }

        private void ApplyChanges() 
        {
            Vector3 sourcePosition = startingPosition + arrayOffset;
            targetObject.transform.position = sourcePosition;

            int totalDuplicateCount = objectCount.x * objectCount.y * objectCount.z - 1;
            if(totalDuplicateCount < duplicates.Count) 
            {
                for (int i = totalDuplicateCount; i < duplicates.Count; i++) 
                {
                    GameObject.DestroyImmediate(duplicates[i].gameObject);
                }
                duplicates.RemoveRange(totalDuplicateCount, duplicates.Count - totalDuplicateCount);
            }
            while (totalDuplicateCount > duplicates.Count) 
            {
                duplicates.Add(GameObject.Instantiate(targetObject, targetObject.transform.parent));
            }

            int duplicateIndex = 0;
            for (int x = 0; x < objectCount.x; x++)
            {
                for (int y = 0; y < objectCount.y; y++)
                {
                    for (int z = 0; z < objectCount.z; z++)
                    {
                        if(x == 0 && y == 0 && z == 0) { continue; }
                        Vector3 duplicatePosition = sourcePosition + new Vector3(x * objectOffset.x, y * objectOffset.y, z * objectOffset.z);
                        duplicates[duplicateIndex].transform.position = duplicatePosition;
                        duplicateIndex++;
                    }
                }
            }
            SceneView.RepaintAll();
        }

        private void Vector3IntMinLimiter(ref Vector3Int vector) 
        {
            if (vector.x <= 0) { vector.x = 1; }
            if (vector.y <= 0) { vector.y = 1; }
            if (vector.z <= 0) { vector.z = 1; }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            var sceneWindow = EditorWindow.GetWindow(typeof(SceneView));
            
            startingPosition = targetObject.transform.position;
            mainEditorWindow = sceneWindow.position;
            open = true;
            applied = false;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public override void OnClose()
        {
            base.OnClose();
            if (!applied)
            {
                targetObject.transform.position = startingPosition;
                foreach (var duplicate in duplicates)
                {
                    GameObject.DestroyImmediate(duplicate.gameObject);
                }
            }
            else 
            {
                targetObject.transform.position = startingPosition;
                Undo.RegisterCompleteObjectUndo(targetObject.transform, "ArrayDuplicate");
                foreach (var duplicate in duplicates) 
                {
                    Undo.RegisterCreatedObjectUndo(duplicate, "ArrayDuplicate");
                }
                targetObject.transform.position = startingPosition + arrayOffset;
            }
            targetObject = null;
            open = false;
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView view) 
        {
            Handles.color = Color.magenta;
            Vector3 sourcePosition = startingPosition + arrayOffset;
            for (int x = 0; x < objectCount.x; x++)
            {
                for (int y = 0; y < objectCount.y; y++)
                {
                    for (int z = 0; z < objectCount.z; z++)
                    {
                        Vector3 duplicatePosition = sourcePosition + new Vector3(x * objectOffset.x, y * objectOffset.y, z * objectOffset.z);
                        Handles.DrawLine(duplicatePosition, duplicatePosition + new Vector3(0, 0.1f, 0));
                    }
                }
            }

        }
    }
}