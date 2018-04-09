using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Soludus;
using UnityEditorInternal;

[CustomEditor(typeof(VR_Splash)), CanEditMultipleObjects]
public class VR_SplashEditor : Editor
{
    const string LogosPropName = "logos";

    const string SpritePropName = "sprite";
    const string DurationPropName = "duration";
    const string FadeInPropName = "fadeIn";
    const string FadeOutPropName = "fadeOut";
    const string ColorPropName = "color";
    const string ScalePropName = "scale";

    private ReorderableList m_logos = null;

    private static readonly string[] Exclude =
    {
        "m_Script",
        LogosPropName
    };

    private void OnEnable()
    {
        const float optionsHeight = (16 + 16 + 16 + 8 + 8);
        m_logos = ReorderableListHelpers.CreateGenericReorderableList(serializedObject, serializedObject.FindProperty(LogosPropName));
        m_logos.elementHeightCallback = null;
        m_logos.elementHeight = optionsHeight + 30;
        m_logos.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var prop = m_logos.serializedProperty.GetArrayElementAtIndex(index);
            var sprite = prop.FindPropertyRelative(SpritePropName);
            var duration = prop.FindPropertyRelative(DurationPropName);
            var fadeIn = prop.FindPropertyRelative(FadeInPropName);
            var fadeOut = prop.FindPropertyRelative(FadeOutPropName);
            var color = prop.FindPropertyRelative(ColorPropName);
            var scale = prop.FindPropertyRelative(ScalePropName);

            var position = rect;
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 0.5f * labelWidth;

            {
                var spriteRect = position;
                spriteRect.width = spriteRect.height;
                spriteRect.min += new Vector2(2, 2);
                spriteRect.max -= new Vector2(2, 2);
                position.xMin += spriteRect.width + 10;

                var logo = sprite.objectReferenceValue as Sprite;

                if (logo == PlayerSettings.SplashScreenLogo.unityLogo)
                {
                    DrawSprite(spriteRect, logo, color.colorValue);
                    scale.floatValue = 1/3f;
                }
                else
                {
                    EditorGUI.ObjectField(spriteRect, sprite, typeof(Sprite), GUIContent.none);
                    scale.floatValue = 1.0f;
                }
            }

            position.yMin += (position.height - optionsHeight) / 2;

            {
                var durationRect = position;
                durationRect.height = 16;
                position.yMin += durationRect.height + 8;
                EditorGUI.PropertyField(durationRect, duration);
            }

            {
                var fadePosition = position;
                fadePosition.height = 16;
                position.yMin += fadePosition.height + 8;

                {
                    var fadeInRect = fadePosition;
                    fadeInRect.width /= 2;
                    fadePosition.xMin += fadeInRect.width + 8;
                    EditorGUI.PropertyField(fadeInRect, fadeIn);
                }

                {
                    var fadeOutRect = fadePosition;
                    EditorGUI.PropertyField(fadeOutRect, fadeOut);
                }
            }

            {
                var colorRect = position;
                colorRect.height = 16;
                EditorGUI.PropertyField(colorRect, color);
            }

            EditorGUIUtility.labelWidth = labelWidth;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, Exclude);
        m_logos.DoLayoutList();

        if (GUILayout.Button("Add Unity logo"))
        {
            var logos = m_logos.serializedProperty;
            logos.InsertArrayElementAtIndex(logos.arraySize);
            var logo = logos.GetArrayElementAtIndex(logos.arraySize - 1);
            logo.FindPropertyRelative(SpritePropName).objectReferenceValue = PlayerSettings.SplashScreenLogo.unityLogo;
        }

        serializedObject.ApplyModifiedProperties();
    }


    private static Rect ScaleRect(Rect rect, Vector2 scale)
    {
        rect.xMin *= scale.x;
        rect.xMax *= scale.x;
        rect.yMin *= scale.y;
        rect.yMax *= scale.y;
        return rect;
    }

    private static void DrawSprite(Rect position, Sprite sprite, Color color)
    {
        var guiColor = GUI.color;
        GUI.color = color;
        GUI.DrawTextureWithTexCoords(position, sprite.texture, ScaleRect(sprite.rect, sprite.texture.texelSize));
        GUI.color = guiColor;
    }

}
