using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wattle.UI;
using Wattle.Utils;

namespace Wattle.Infrastructure
{
    public class SceneManager : Singleton<SceneManager>
    {
        public event Action<Scene> OnSceneChanged;

        public override IEnumerator Initalise()
        {
            initialised = true;

            yield return base.Initalise();
        }

        /// <summary>
        /// Load a new scene
        /// </summary>
        /// <param name="sceneName"> The name of the scene. </param>
        /// <param name="loadMode"> The load mode to use. </param>
        /// <param name="onComplete"> Action to fire when loading is completed. </param>
        public void LoadScene(string sceneName, LoadSceneMode loadMode, Action onComplete = null)
        {
            // Open the loading the UI
            UILoading.ShowScreen(async () =>
            {
                LOG.Log($"Loading Scene: {sceneName}", LOG.Type.SCENE);

                await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, loadMode);

                LOG.Log($"Scene Loaded: {sceneName}", LOG.Type.SCENE);

                Scene loadedScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);

                OnSceneChanged?.Invoke(loadedScene);

                onComplete?.Invoke();

                // Hide the loading screen
                UILoading.HideScreen();
            });
        }
    }
}