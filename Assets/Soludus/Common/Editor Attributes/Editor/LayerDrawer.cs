using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Soludus
{

    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }

}