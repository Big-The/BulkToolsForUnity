using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BTools.SimpleEditorTools 
{
    public class GameObjectRandomness : PopupWindowContent
    {
        private static Vector2 popupSize = new Vector2(200, 310);
        private static List<Transform> transforms = new List<Transform>();
        private static bool open = false;
        private static bool applied = false;
        private Rect mainEditorWindow;

        private Vector3 translateMin = Vector3.zero;
        private Vector3 translateMax = Vector3.zero;
        private Vector3 rotateMin = Vector3.zero;
        private Vector3 rotateMax = Vector3.zero;
        private Vector3 scaleMin = Vector3.one;
        private Vector3 scaleMax = Vector3.one;
        private int scaleMode = 1;

        private List<(Vector3 position, Vector3 roation, Vector3 scale)> startingPositions = new List<(Vector3 position, Vector3 roation, Vector3 scale)>();

        private int randomSeed = 0;
        private System.Random random;

        [MenuItem("GameObject/Simple Editor Tools/Randomize Objects")]
        public static void RandomizeObjects(MenuCommand command)
        {
            if (command.context != Selection.gameObjects[0]) { return; }
            transforms.Clear();
            foreach (GameObject go in Selection.gameObjects) 
            {
                if (!PrefabUtility.IsPartOfPrefabAsset(go)) 
                {
                    transforms.Add(go.transform);
                }
            }
            if(!open) 
            {
                PopupWindow.Show(new Rect(1, 1, 1, 1), new GameObjectRandomness());
            }
        }


        [MenuItem("GameObject/Simple Editor Tools/Randomize Objects", validate = true)]
        public static bool RandomActionValidate()
        {
            return Selection.gameObjects.Length > 0;
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

            EditorGUILayout.LabelField("Object Randomization", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            randomSeed = EditorGUILayout.IntField("Seed", randomSeed);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Translation Offset:");
            translateMin = EditorGUILayout.Vector3Field("Min", translateMin);
            translateMax = EditorGUILayout.Vector3Field("Max", translateMax);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Rotation Offset:");
            rotateMin = EditorGUILayout.Vector3Field("Min", rotateMin);
            rotateMax = EditorGUILayout.Vector3Field("Max", rotateMax);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Scale Multiplier:");
            scaleMode = EditorGUILayout.Popup(scaleMode, new string[] { "Sepreate Scalars", "Shared Scalar" });
            if (scaleMode == 0)
            {
                scaleMin = EditorGUILayout.Vector3Field("Min", scaleMin);
                scaleMax = EditorGUILayout.Vector3Field("Max", scaleMax);
            }
            else 
            {
                scaleMin.x = EditorGUILayout.FloatField("Min", scaleMin.x);
                scaleMax.x = EditorGUILayout.FloatField("Max", scaleMax.x);
            }
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
            random = new System.Random(randomSeed);
            for (int transformIndex = 0; transformIndex < transforms.Count; transformIndex++)
            {
                var translation = GetRandomVectorInRange(translateMin, translateMax);
                var rotation = GetRandomVectorInRange(rotateMin, rotateMax);
                var scale = GetRandomVectorInRange(scaleMin, scaleMax);

                var t = transforms[transformIndex];
                var oldTData = startingPositions[transformIndex];

                t.position = oldTData.position + translation;
                t.eulerAngles = oldTData.roation + rotation;
                if (scaleMode == 0)
                {
                    t.localScale = new Vector3(oldTData.scale.x * scale.x, oldTData.scale.y * scale.y, oldTData.scale.z * scale.z);
                }
                else 
                {
                    t.localScale = new Vector3(oldTData.scale.x * scale.x, oldTData.scale.y * scale.x, oldTData.scale.z * scale.x);
                }
            }
        }

        private Vector3 GetRandomVectorInRange(Vector3 min, Vector3 max) 
        {
            return new Vector3(
                GetRandomFloatInRange(min.x, max.x),
                GetRandomFloatInRange(min.y, max.y),
                GetRandomFloatInRange(min.z, max.z)
                );
        }

        private float GetRandomFloatInRange(float min, float max) 
        {
            float range = max - min;
            return range * (float)random.NextDouble() + min;
        }

        public override void OnOpen()
        {
            base.OnOpen();
            var sceneWindow = EditorWindow.GetWindow(typeof(SceneView));
            mainEditorWindow = sceneWindow.position;
            Undo.RegisterCompleteObjectUndo(transforms.ToArray(), "RandomizeObjects");
            startingPositions = new List<(Vector3 position, Vector3 roation, Vector3 scale)>(transforms.Count);
            for (int transformIndex = 0; transformIndex < transforms.Count; transformIndex++)
            {
                var t = transforms[transformIndex];
                startingPositions.Add((t.position, t.eulerAngles, t.localScale));
            }
            open = true;
            applied = false;
        }

        public override void OnClose() 
        {
            base.OnClose();
            if (!applied) 
            {
                for (int transformIndex = 0; transformIndex < transforms.Count; transformIndex++)
                {
                    var t = transforms[transformIndex];
                    var oldTData = startingPositions[transformIndex];
                    t.position = oldTData.position;
                    t.eulerAngles = oldTData.roation;
                    t.localScale = oldTData.scale;
                }
            }
            transforms.Clear();
            open = false;
        }
    }
}
