using NUnit.Framework;
using System;
using UnityEngine;

namespace Gamble.Utils
{
    public static class UnityUtils
    {
        // Components

        public static Transform TryFindChildByName(this Transform parent, string name, bool includeInactive = true)
        {
            foreach (Transform t in parent.GetComponentsInChildren<Transform>(includeInactive))
            {
                if (t.name == name)
                    return t;
            }

            return null;
        }


        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            if (!go.TryGetComponent<T>(out T component))
            {
                component = go.AddComponent<T>();
                return component;
            }

            return component;
        }

        #region Vector Utils

        // Vector 2
        public static Vector2 WithY(this Vector2 v1, float y = 0)
        {
            return new Vector2(v1.x, y);
        }

        public static Vector2 WithX(this Vector2 v1, float x = 0)
        {
            return new Vector2(x, v1.y);
        }

        // Vector 3
        public static Vector3 WithX(this Vector3 v1, float x = 0)
        {
            return new Vector3(x, v1.y, v1.z);
        }

        public static Vector3 WithXY(this Vector3 v1, float x = 0, float y = 0)
        {
            return new Vector3(x, y, v1.z);
        }

        public static Vector3 WithXZ(this Vector3 v1, float x = 0, float z = 0)
        {
            return new Vector3(x, v1.y, z);
        }

        public static Vector3 WithY(this Vector3 v1, float y = 0)
        {
            return new Vector3(v1.x, y, v1.z);
        }

        public static Vector3 WithYZ(this Vector3 v1, float y = 0, float z = 0)
        {
            return new Vector3(v1.x, y, z);
        }

        public static Vector3 WithZ(this Vector3 v1, float z = 0)
        {
            return new Vector3(v1.x, v1.y, z);
        }

        #endregion
    }
}