using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soludus
{

    public static class GameObjectExtensions
    {
        private static class GenericData<T>
        {
            public static readonly List<T> list = new List<T>();
        }


        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var c = go.GetComponent<T>();
            return c != null ? c : go.AddComponent<T>();
        }

        public static Component GetOrAddComponent(this GameObject go, Type type)
        {
            var c = go.GetComponent(type);
            return c != null ? c : go.AddComponent(type);
        }

        public static T GetOrAddComponent<T>(this Component co) where T : Component
        {
            return GetOrAddComponent<T>(co.gameObject);
        }

        public static Component GetOrAddComponent(this Component co, Type type)
        {
            return GetOrAddComponent(co.gameObject, type);
        }


        public static void AddComponentsToList<T>(this GameObject go, List<T> list)
        {
            go.GetComponents(GenericData<T>.list);
            list.AddRange(GenericData<T>.list);
        }

        public static void AddComponentsToList<T>(this Component co, List<T> list)
        {
            co.GetComponents(GenericData<T>.list);
            list.AddRange(GenericData<T>.list);
        }

        public static void AddComponentsInChildrenToList<T>(this GameObject go, bool includeInactive, List<T> list)
        {
            go.GetComponentsInChildren(includeInactive, GenericData<T>.list);
            list.AddRange(GenericData<T>.list);
        }

        public static void AddComponentsInChildrenToList<T>(this Component co, bool includeInactive, List<T> list)
        {
            AddComponentsInChildrenToList(co.gameObject, includeInactive, list);
        }

        public static void AddComponentsInParentToList<T>(this GameObject go, bool includeInactive, List<T> list)
        {
            go.GetComponentsInParent(includeInactive, GenericData<T>.list);
            list.AddRange(GenericData<T>.list);
        }

        public static void AddComponentsInParentToList<T>(this Component co, bool includeInactive, List<T> list)
        {
            AddComponentsInParentToList(co.gameObject, includeInactive, list);
        }


        public static void SetBehavioursEnabled<T>(this GameObject go, bool enabled) where T : Behaviour
        {
            foreach (var c in go.GetComponentsInChildren<T>(true))
            {
                c.enabled = enabled;
            }
        }

        public static void SetCollidersEnabled(this GameObject go, bool enabled)
        {
            foreach (var c in go.GetComponentsInChildren<Collider>(true))
            {
                c.enabled = enabled;
            }
        }

        public static void SetRenderersEnabled(this GameObject go, bool enabled)
        {
            foreach (var c in go.GetComponentsInChildren<Renderer>(true))
            {
                c.enabled = enabled;
            }
        }
    }

}