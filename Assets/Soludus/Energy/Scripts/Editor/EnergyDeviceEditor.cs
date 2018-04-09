using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Soludus.Energy
{

    public class EnergyDeviceEditor
    {
        [MenuItem("GameObject/Soludus/Energy Device", priority = 10)]
        public static void CreateDeviceGO()
        {
            var prefab = Resources.Load<GameObject>("Energy Device Editor Template");
            var go = Object.Instantiate(prefab);
            go.name = "Energy Device";
            EditorHelpers.SetUpNewGameObjectInScene(go);
        }

        [MenuItem("GameObject/Soludus/Energy Device Slot", priority = 10)]
        public static void CreateSlotGO()
        {
            var prefab = Resources.Load<GameObject>("Energy Device Slot Editor Template");
            var go = Object.Instantiate(prefab);
            go.name = "Energy Device Slot";
            EditorHelpers.SetUpNewGameObjectInScene(go);
        }
    }

}