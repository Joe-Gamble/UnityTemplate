using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wattle.UI;
using Wattle.Utils;

namespace Wattle.Infrastructure
{
    public class SingletonManager : PersistentSingleton<SingletonManager>
    {
        [SerializeField] private List<Singleton> singletons = new();
        public static bool Initialised { get; private set; } = false;
        private Coroutine singletonCoroutine = null;

        private static readonly List<Action> initialisationCallbacks = new List<Action>();

        public void InitialiseSingletons(Action onComplete = null)
        {
            UILoading.ShowIndicator(() => Initialised);

            if (singletonCoroutine != null)
                StopCoroutine(singletonCoroutine);

            singletonCoroutine = StartCoroutine(InitialiseSingletons_Internal(onComplete));
        }

        public static void NotifyOnInitialised(Action onInitialised)
        {
            if (Initialised)
            {
                onInitialised.Invoke();
            }
            else
            {
                initialisationCallbacks.Add(onInitialised);
            }
        }

        private IEnumerator InitialiseSingletons_Internal(Action onComplete = null)
        {
            LOG.Log("Starting singleton initialization...", LOG.Type.STARTUP);

            foreach (Singleton singleton in singletons)
            {
                if (singleton != null)
                {
                    LOG.Log($"Initialising {singleton.GetType().Name}...", LOG.Type.STARTUP);
                    yield return singleton.Initalise();
                }
            }

            LOG.Log("All singletons initialized!", LOG.Type.STARTUP);

            Initialised = true;
            NotifyListeners();
            singletonCoroutine = null;

            onComplete?.Invoke();
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

        private void OnDestroy()
        {
            if (singletonCoroutine != null)
            {
                LOG.Log("SingletonManager destroyed, canceling initialisation", LOG.Type.GENERAL);
                StopCoroutine(singletonCoroutine);
            }
        }
    }
}