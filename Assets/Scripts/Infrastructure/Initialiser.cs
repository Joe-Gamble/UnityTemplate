using Wattle.Utils;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Wattle.Infrastructure
{
    public enum GameState
    {
        MainMenu,
        Paused,
    }

    public class Initialiser : PersistentSingleton<Initialiser>
    {
        public static event Action<GameState> OnGameStateChanged;
        private static GameState gameState;

        [Header("Singleton Manager")]
        [SerializeField] private SingletonManager singletonManager;

        private void OnEnable()
        {
            InitialiseApplication();
        }

        public void InitialiseApplication()
        {
            singletonManager.InitialiseSingletons(() =>
            {

            });
        }

        private void LoadSandbox()
        {
            LoadGame();
        }

        public static void LoadGame()
        {

        }
        public void EndGame()
        {

        }

        public static void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void ChangeGamestate(GameState state)
        {
            gameState = state;
            OnGameStateChanged?.Invoke(gameState);
        }
    }
}