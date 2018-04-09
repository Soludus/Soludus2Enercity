using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Soludus.Energy
{

    [CustomEditor(typeof(HazardConfiguration)), CanEditMultipleObjects]
    public class HazardConfigurationEditor : Editor
    {
        private const string m_hazardsField = "spawnedHazards";
        private static readonly string[] m_excluded =
        {
        "m_Script",
        m_hazardsField,
    };

        private ReorderableList m_hazards;

        private void OnEnable()
        {
            m_hazards = ReorderableListHelpers.CreateGenericExpandedReorderableList(serializedObject, serializedObject.FindProperty(m_hazardsField), 0, 16);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, m_excluded);
            m_hazards.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }

}