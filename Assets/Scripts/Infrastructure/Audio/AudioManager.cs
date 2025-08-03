using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Wattle.Utils;

namespace Wattle.Infrastructure
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private Transform instanceParent;
        [SerializeField] private AudioInstance audioInstancePrefab;

        private ObjectPool<AudioInstance> instancePool;

        private int defaultCapacity = 5;
        private int maxCapacity = 100;

        private AudioInstance musicInstance = null;

        public override IEnumerator Initalise()
        {
            instancePool = new ObjectPool<AudioInstance>(
                createFunc: () => { return CreateAudioInstance(); },
                OnAudioInstanceRetrieved,
                OnAudioInstanceReleased,
                OnAudioInstanceDestroyed,
                collectionCheck: false,
                defaultCapacity: defaultCapacity,
                maxSize: maxCapacity
                );

            musicInstance = Instantiate(audioInstancePrefab, instanceParent);

            initialised = true;

            yield return base.Initalise();
        }

        private void OnApplicationQuit()
        {
            StartCoroutine(musicInstance.CleanUp());

            instancePool.Dispose();
            instancePool = null;
        }

        public static void PlayMusic(AudioClip musicTrack, Action onComplete = null)
        {
            Instance.musicInstance.Load(musicTrack, AudioType.MUSIC, onComplete);
        }

        public static AudioInstance Play(AudioClip audio, Vector3 position, AudioType audioType, Action onComplete = null)
        {
            AudioInstance instance = Instance.instancePool.Get();
            instance.transform.position = position;

            instance.Load(audio, audioType, onComplete);

            return instance;
        }

        private AudioInstance CreateAudioInstance()
        {
            AudioInstance audioInstance = Instantiate(audioInstancePrefab, instanceParent);
            audioInstance.gameObject.SetActive(false);

            return audioInstance;
        }
        private void OnAudioInstanceRetrieved(AudioInstance instance)
        {
            instance.onInstanceFinished += OnAudioInstanceFinished;
            instance.gameObject.SetActive(true);
        }

        private void OnAudioInstanceReleased(AudioInstance instance)
        {
            instance.onInstanceFinished -= OnAudioInstanceFinished;
            instance.gameObject.SetActive(false);
            StartCoroutine(instance.CleanUp());
        }

        private void OnAudioInstanceDestroyed(AudioInstance instance)
        {
            instance.onInstanceFinished -= OnAudioInstanceFinished;
            StartCoroutine(instance.CleanUp(() =>
            {
                Destroy(instance.gameObject);

            }));
        }

        private void OnAudioInstanceFinished(AudioInstance audioInstance)
        {
            instancePool.Release(audioInstance);
        }
    }

    public enum AudioType
    {
        SFX,
        MUSIC,
        VOICE
    }
}