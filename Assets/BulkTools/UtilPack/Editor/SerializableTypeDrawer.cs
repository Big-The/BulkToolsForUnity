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
        //caches the results of the supported types so we don't have to do assembly searches each GUI update.
        private static Dictionary<Type, List<Type>> supportedTypesCache = new Dictionary<Type, List<Type>>();


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //Find the fieldInfo and attributes for the current property
            var field = property.serializedObject.targetObject.GetType().GetField(property.name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var attributes = Attribute.GetCustomAttributes(field, typeof(TypeRestrictionAttribute));

            if (attributes.Length == 0) //If there is no TypeRestrictionAttribute we can't show a dropdown so we skip the normal inspector.
            {
                EditorGUILayout.LabelField(label, new GUIContent("Missing TypeRestriction Attribute"));
                return;
            }

            List<Type> types;
            Type baseType = ((TypeRestrictionAttribute)attributes[0]).type;
            if (!supportedTypesCache.TryGetValue(baseType, out types)) //Try to get the type list from cache
            {
                //If the type list doesn't exist in the cache yet, do a fresh search and add the results to the cach.
                types = FindTypes(baseType);
                supportedTypesCache.Add(baseType, types);
            }

            //Create a names array from the types list.
            string[] typeNames = new string[types.Count];
            for (int i = 0; i < typeNames.Length; i++) 
            {
                typeNames[i] = types[i].Name;
            }


            bool changed = false; //Tracks if the new type selected is diferent from the old one
            int typeIndex; //Index of the current selected type
            var oldObject = field.GetValue(property.serializedObject.targetObject) as SerializableType; //The current SerializableType

            if (oldObject != null) //If there is already a type selected set the typeIndex to match
            {
                typeIndex = types.IndexOf(oldObject.SavedType);
                if (typeIndex < 0)//If the type selected is not in the allowed types default to the base type
                {
                    typeIndex = 0;
                    changed = true;
                }
            }
            else //If there is no type selected yet default it to the base type.
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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 0;
        }

        /// <summary>
        /// Finds all types that are or inherit from the baseType by searching the active assemblies. The base type will always be in the 0 index position
        /// </summary>
        /// <param name="baseType"></param>
        /// <returns></returns>
        private static List<Type> FindTypes(Type baseType)
        {
            List<Type> types = new List<Type>() { baseType };
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                string assemblyName = assembly.GetName().ToString();
                foreach (var type in assembly.GetTypes())
                {
                    if (type != baseType && baseType.IsAssignableFrom(type))
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
