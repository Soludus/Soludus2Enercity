using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RendererSortingLayer))]
[CanEditMultipleObjects]
public class RendererSortingLayerEditor : Editor
{
    private SerializedObject m_rendererSerializedObject;

    private SerializedProperty m_SortingOrder;
    private SerializedProperty m_SortingLayerID;

    private static MethodInfo RenderSortingLayerFields_methodInfo;
    private object[] RenderSortingLayerFields_params;

    static RendererSortingLayerEditor()
    {
        var SortingLayerEditorUtility_type = typeof(Editor).Assembly.GetType("UnityEditor.SortingLayerEditorUtility");
        RenderSortingLayerFields_methodInfo = SortingLayerEditorUtility_type.GetMethod("RenderSortingLayerFields", BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(SerializedProperty), typeof(SerializedProperty) }, null);
    }

    private void OnEnable()
    {
        Object[] targetRenderers = System.Array.ConvertAll(targets, t =>
        {
            var rsl = (RendererSortingLayer)t;
            return rsl.GetComponent<Renderer>();
        });

        m_rendererSerializedObject = new SerializedObject(targetRenderers);

        m_SortingOrder = m_rendererSerializedObject.FindProperty("m_SortingOrder");
        m_SortingLayerID = m_rendererSerializedObject.FindProperty("m_SortingLayerID");

        RenderSortingLayerFields_params = new object[] { m_SortingOrder, m_SortingLayerID };
    }

    public override void OnInspectorGUI()
    {
        m_rendererSerializedObject.Update();

        RenderSortingLayerFields_methodInfo.Invoke(null, RenderSortingLayerFields_params);

        m_rendererSerializedObject.ApplyModifiedProperties();
    }
}