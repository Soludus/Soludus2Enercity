using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Soludus
{

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            int newMask = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
            if (EditorGUI.EndChangeCheck())
                property.intValue = newMask;
            EditorGUI.EndProperty();
        }
    }

}