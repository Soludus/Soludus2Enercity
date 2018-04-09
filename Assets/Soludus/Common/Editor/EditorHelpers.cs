using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Soludus
{

    public static class EditorHelpers
    {
        public static void SetUpNewGameObjectInScene(GameObject go, bool moveToView = true)
        {
            go.transform.SetParent(Selection.activeTransform);
            Selection.activeTransform = go.transform;
            if (moveToView)
                SceneView.lastActiveSceneView.MoveToView();

            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        }
    }

}