using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Soludus.Energy
{

    [CustomEditor(typeof(MapConfiguration)), CanEditMultipleObjects]
    public class MapConfigurationEditor : Editor
    {
        private const string m_energyTargetsField = "energyTargets";
        private static readonly string[] m_excluded =
        {
        "m_Script",
        m_energyTargetsField,
    };

        private ReorderableList m_energyTargets;

        private void OnEnable()
        {
            m_energyTargets = ReorderableListHelpers.CreateGenericExpandedReorderableList(serializedObject, serializedObject.FindProperty(m_energyTargetsField), 0, 16);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, m_excluded);
            m_energyTargets.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }

}