using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wattle.Utils
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
                        LOG.LogError("There is more than one " + typeof(T).Name + " in the scene.", LOG.Type.STARTUP);
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
        protected bool initialised = false;

        private readonly List<Action> initialisationCallbacks = new List<Action>();

        public void NotifyOnInitialised(Action onComplete)
        {
            if (initialised)
                onComplete.Invoke();
            else
                initialisationCallbacks.Add(onComplete);
        }

        public virtual IEnumerator Initalise()
        {
            yield return new WaitUntil(() => initialised);

            LOG.Log($"{GetType().Name} Initialised!", LOG.Type.STARTUP);

            NotifyListeners();
        }

        private void NotifyListeners()
        {
            if (initialisationCallbacks != null)
            {
                foreach (Action listener in initialisationCallbacks)
                {
                    listener.Invoke();
                }

                initialisationCallbacks.Clear();
            }
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