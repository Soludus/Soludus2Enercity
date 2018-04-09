using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Soludus
{

    public static class ReorderableListHelpers
    {
        public static ReorderableList CreateGenericReorderableList(SerializedObject serializedObject, SerializedProperty elements)
        {
            var list = new ReorderableList(serializedObject, elements, true, true, true, true);

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                EditorGUI.PropertyField(rect, list.serializedProperty.GetArrayElementAtIndex(index), GUIContent.none, true);
            };

            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, list.serializedProperty.displayName);
            };

            list.elementHeightCallback = (int index) =>
            {
                return EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index), true);
            };

            return list;
        }

        public static ReorderableList CreateGenericExpandedReorderableList(SerializedObject serializedObject, SerializedProperty elements, float leftMargin = 0, float elementHeightOffset = 0)
        {
            var list = new ReorderableList(serializedObject, elements, true, true, true, true);

            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, list.serializedProperty.displayName);
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.xMin += leftMargin;
                var prop = list.serializedProperty.GetArrayElementAtIndex(index);
                SerializedPropertyHelper.DrawPropertyChildren(rect, prop);
            };

            list.elementHeightCallback = (int index) =>
            {
                var prop = list.serializedProperty.GetArrayElementAtIndex(index);
                return SerializedPropertyHelper.GetPropertyChildrenHeight(prop) + elementHeightOffset;
            };

            return list;
        }

        public static void DrawSingleLineElement(Rect rect, SerializedProperty element)
        {
            rect.y += 2;
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                element, GUIContent.none);
        }

        public static void DoRemoveButtonNotLeavingNull(ReorderableList list)
        {
            if (list.serializedProperty != null)
            {
                var elem = list.serializedProperty.GetArrayElementAtIndex(list.index);
                if (elem.propertyType == SerializedPropertyType.ObjectReference)
                    elem.objectReferenceValue = null;
                else if (elem.propertyType == SerializedPropertyType.ExposedReference)
                    elem.exposedReferenceValue = null;
            }
            ReorderableList.defaultBehaviours.DoRemoveButton(list);
        }
    }

}