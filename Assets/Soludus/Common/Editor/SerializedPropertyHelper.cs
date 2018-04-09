using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Soludus
{

    public static class SerializedPropertyHelper
    {
        public static void DrawPropertyChildren(Rect rect, SerializedProperty prop)
        {
            Rect pos = rect;
            var end = prop.GetEndProperty(true);
            prop.Next(true);
            do
            {
                pos.height = EditorGUI.GetPropertyHeight(prop, true);
                EditorGUI.PropertyField(pos, prop, true);
                pos.y += pos.height + 2;
            }
            while (prop.Next(false) && !SerializedProperty.EqualContents(prop, end));
        }

        public static float GetPropertyChildrenHeight(SerializedProperty prop)
        {
            float h = 0;
            var end = prop.GetEndProperty(true);
            prop.Next(true);
            do
            {
                h += EditorGUI.GetPropertyHeight(prop, true) + 2;
            }
            while (prop.Next(false) && !SerializedProperty.EqualContents(prop, end));
            return h;
        }
    }

}