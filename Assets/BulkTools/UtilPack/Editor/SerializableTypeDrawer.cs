using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

namespace BTools.UtilPack.EditorScripts 
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        private static Dictionary<Type, List<Type>> supportedTypesCache = new Dictionary<Type, List<Type>>();


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var field = property.serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var attributes = Attribute.GetCustomAttributes(field, typeof(TypeRestrictionAttribute));
            if (attributes.Length > 0)
            {
                List<Type> types;
                Type baseType = ((TypeRestrictionAttribute)attributes[0]).type;
                if (!supportedTypesCache.TryGetValue(baseType, out types)) 
                {
                    types = FindTypes(baseType);
                    supportedTypesCache.Add(baseType, types);
                }
                string[] typeNames = new string[types.Count];
                for (int i = 0; i < typeNames.Length; i++) 
                {
                    typeNames[i] = types[i].Name;
                }

                bool changed = false;
                int typeIndex;

                var oldObject = field.GetValue(property.serializedObject.targetObject) as SerializableType;
                if (oldObject != null)
                {
                    typeIndex = types.IndexOf(oldObject.SavedType);
                    if (typeIndex < 0)
                    {
                        typeIndex = 0;
                        changed = true;
                    }
                }
                else 
                {
                    typeIndex = 0;
                    changed = true;
                }
                

                //Display popup:
                int newIndex = EditorGUILayout.Popup(label, typeIndex, typeNames);
                if(newIndex != typeIndex) { changed = true; }

                if (changed)//Set new value if changed
                {
                    field.SetValue(property.serializedObject.targetObject, new SerializableType(types[newIndex]));
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                }
            }
            else 
            {
                EditorGUILayout.LabelField(label, new GUIContent("Missing TypeRestriction Attribute"));
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }
        public static List<Type> FindTypes(Type baseType)
        {
            List<Type> types = new List<Type>();
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                string assemblyName = assembly.GetName().ToString();
                foreach (var type in assembly.GetTypes())
                {
                    if (baseType.IsAssignableFrom(type))
                    {
                        types.Add(type);
#if !DisableSafetySerializedType
                        if (types.Count >= 200) 
                        {
                            Debug.LogWarning($"SerializedType with restriction ({baseType.Name}) found more than 200 matching types. Was this intentional?");
                            return types;
                        }
#endif
                    }
                }
            }
            return types;
        }
    }
}
