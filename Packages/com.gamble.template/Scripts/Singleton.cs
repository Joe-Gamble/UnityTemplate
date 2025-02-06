using System.Collections;
using UnityEngine;

namespace Gamble.Utils
{
    public abstract class Singleton<T> : Singleton where T : Component
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    T[] objs = FindObjectsByType(typeof(T), FindObjectsSortMode.None) as T[];

                    if (objs.Length > 0)
                        instance = objs[0];
                    if (objs.Length > 1)
                    {
                        Debug.LogError("There is more than one " + typeof(T).Name + " in the scene.");
                    }
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }

    public abstract class Singleton : MonoBehaviour
    {
        public virtual IEnumerator Initalise()
        {
            yield return null;
        }
    }

    [System.Serializable]
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(Instance);

                OnAwake();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public virtual void OnAwake() { }

    }
}