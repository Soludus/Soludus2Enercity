using UnityEngine;
using UnityEditor;

namespace Soludus
{

    [CustomPropertyDrawer(typeof(MinMaxSliderAttribute))]
    class MinMaxSliderDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                Vector2 range = property.vector2Value;
                float min = range.x;
                float max = range.y;
                MinMaxSliderAttribute attr = attribute as MinMaxSliderAttribute;

                float floatFieldWidth = position.width * 0.15f;

                EditorGUI.BeginChangeCheck();

                position = EditorGUI.PrefixLabel(position, label);

                min = EditorGUI.FloatField(
                    new Rect(position.x, position.y, floatFieldWidth, position.height),
                    min);

                EditorGUI.MinMaxSlider(
                    new Rect(position.x + 1 * (floatFieldWidth + 2), position.y, position.width - 2 * (floatFieldWidth + 2), position.height),
                    GUIContent.none, ref min, ref max, attr.min, attr.max);

                // rounding min max slider
                min = Mathf.Round(min * 100) / 100;
                max = Mathf.Round(max * 100) / 100;

                max = EditorGUI.FloatField(
                    new Rect(position.xMax - floatFieldWidth, position.y, floatFieldWidth, position.height),
                    max);

                if (EditorGUI.EndChangeCheck())
                {
                    range.x = min;
                    range.y = max;
                    property.vector2Value = range;
                }
            }
            else
            {
                EditorGUI.LabelField(position, label, "Use only with Vector2");
            }
        }
    }

}