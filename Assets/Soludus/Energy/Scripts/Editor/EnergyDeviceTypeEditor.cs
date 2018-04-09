using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Soludus.Energy
{

    [CustomEditor(typeof(EnergyDeviceType)), CanEditMultipleObjects]
    public class EnergyDeviceTypeEditor : Editor
    {
        private const string m_energyEffectsField = "energyEffects";
        private const string m_deviceEffectsField = "deviceEffects";
        private static readonly string[] m_excluded =
        {
        "m_Script",
        m_energyEffectsField,
        m_deviceEffectsField
    };

        private ReorderableList m_energyEffects;
        private ReorderableList m_deviceEffects;

        private void OnEnable()
        {
            m_energyEffects = ReorderableListHelpers.CreateGenericExpandedReorderableList(serializedObject, serializedObject.FindProperty(m_energyEffectsField), 0, 16);
            m_deviceEffects = ReorderableListHelpers.CreateGenericExpandedReorderableList(serializedObject, serializedObject.FindProperty(m_deviceEffectsField), 0, 16);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawPropertiesExcluding(serializedObject, m_excluded);
            m_energyEffects.DoLayoutList();
            m_deviceEffects.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }

}