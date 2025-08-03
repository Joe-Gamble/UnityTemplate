using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Wattle.UI;
using Wattle.Infrastructure;
using DG.Tweening;

namespace Wattle.Utils
{
    /// <summary>
    /// Helper class with utility / extension methods
    /// </summary>
    public static class HelperFunctions
    {
        public static string ToHexString(this Color color)
        {
            return
                ((byte)(color.r * 255)).ToString("X2") +
                ((byte)(color.g * 255)).ToString("X2") +
                ((byte)(color.b * 255)).ToString("X2") +
                ((byte)(color.a * 255)).ToString("X2");
        }

        public static bool HasElements<T>(this List<T> list)
        {
            if (list == null)
                return false;

            return list.Count > 0;
        }

        public static void SetActive(this Behaviour behaviour, bool enabled)
        {
            behaviour.gameObject.SetActive(enabled);
        }

        public static void ToggleParentActive(this Transform transform, bool enabled)
        {
            transform.parent.gameObject.SetActive(enabled);
        }

        public static void ToggleActive(this Transform transform, bool enabled)
        {
            transform.gameObject.SetActive(enabled);
        }

        public static bool IsActive(this Transform transform)
        {
            return transform.gameObject.activeInHierarchy;
        }

        /// <summary>
        /// Wrapping function for sealing an integer between two values
        /// </summary>
        /// <param name="value"> the value of the integer. </param>
        /// <param name="min">The minimum value. </param>
        /// <param name="max">The maximum value. </param>
        /// <returns> The wrapped integer. </returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static int Wrap(int value, int min, int max)
        {
            if (max < min)
                throw new System.ArgumentException("Max must be greater than min");

            int range = max - min + 1;

            return (value - min) % range + min;
        }


        // <summary>
        /// Config extension method to simplify saving configs.
        /// </summary>
        /// <param name="config"> The config to save. </param>
        /// <param name="onComplete"> Action to fire after saving has completed. (Happens on main thread) </param>
        public static void Save(this ISaveable config, Action onComplete = null)
        {
#if UNITY_WEBGL
            onComplete?.Invoke();
            return;
#else
            bool hasSaved = false;

            UILoading.ShowIndicator(() => hasSaved);

            Task.Run(async () =>
            {
                await SaveSystem.SaveConfig(config);
                await Awaitable.MainThreadAsync();

                hasSaved = true;
                onComplete?.Invoke();
            });
#endif

        }

        /// <summary>
        /// Config extension method to simplify loading configs.
        /// </summary>
        /// <param name="config"> The config to load. </param>
        /// <param name="onComplete"> Action to fire after loading has completed. (Happens on main thread) </param>
        public static void Load(this ISaveable config, Action onComplete = null)
        {
#if UNITY_WEBGL
            onComplete?.Invoke();
            return;
#else
            bool hasLoaded = false;

            UILoading.ShowIndicator(() => hasLoaded);

            Task.Run(async () =>
            {
                await SaveSystem.LoadConfig(config);
                await Awaitable.MainThreadAsync();

                hasLoaded = true;
                onComplete?.Invoke();
            });
#endif
        }

        public static void FadeCanvasAsync(CanvasGroup canvasGroup, bool fadeIn, float duration, Action onComplete = null)
        {
            Tweener tween = canvasGroup.DOFade(fadeIn ? 1 : 0, duration).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public static IEnumerator FadeCanvas(CanvasGroup canvasGroup, bool fadeIn, float duration)
        {
            Tweener tween = canvasGroup.DOFade(fadeIn ? 1 : 0, duration).SetAutoKill(true);

            yield return new WaitUntil(() => !tween.active);
        }
    }
}