using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BTools.MoreAttributes.EditorScripts
{
    [CustomPropertyDrawer(typeof(BitMaskAttribute))]
    public class BitMaskPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(property.enumNames.Length > 0) 
            {
                int passedToEditor = property.enumValueFlag;

                //an enum with the value 2^31 (1 in highest value bit) is negative and sorted into the first slot of the names
                if (property.enumDisplayNames.Length == 32)
                {
                    string[] names = new string[32];

                    names[31] = property.enumDisplayNames[0];
                    for (int i = 0; i < 31; i++) 
                    {
                        names[i] = property.enumDisplayNames[i + 1];
                    }

                    property.enumValueFlag = EditorGUI.MaskField(position, label, passedToEditor, names);
                }
                else 
                {
                    property.enumValueFlag = EditorGUI.MaskField(position, label, passedToEditor, property.enumDisplayNames);
                }

            }
            else 
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}