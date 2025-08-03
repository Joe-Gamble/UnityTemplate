using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace Wattle.Infrastructure
{
    public class AudioInstance : MonoBehaviour
    {
        public Action<AudioInstance> onInstanceFinished;

        public AudioClip Audio => audioClip;
        public bool IsPlaying => audioSource.isPlaying;

        [SerializeField] private AudioSource audioSource;

        private Coroutine audioCoroutine = null;
        private AudioClip audioClip = null;
        private AudioType instanceType;

        private Action onCompleteCallback = null;

        public void Load(AudioClip audioClip, AudioType instanceType, Action onCompleteCallback)
        {
            StartCoroutine(CleanUp(() =>
            {
                if (audioSource != null)
                {
                    this.onCompleteCallback = onCompleteCallback;
                    this.instanceType = instanceType;
                    this.audioClip = audioClip;

                    if (instanceType == AudioType.MUSIC)
                    {
                        audioSource.loop = true;
                        audioSource.spatialize = false;
                    }

                    SubscribeToSettingEvents();

                    Play();
                }
            }));
        }

        public IEnumerator CleanUp(Action onComplete = null)
        {
            if (audioClip == null)
            {
                onComplete?.Invoke();
            }
            else
            {
                audioSource.Stop();

                if (audioCoroutine != null)
                    StopCoroutine(audioCoroutine);

                audioCoroutine = null;
                audioClip = null;

                UnsubscribeToSettingEvents();

                onComplete?.Invoke();
            }

            yield return null;
        }

        private void Play()
        {
            audioSource.clip = audioClip;
            audioCoroutine = StartCoroutine(Play_Internal());
        }

        private IEnumerator Play_Internal()
        {
            audioSource.volume = 0;

            audioSource.volume = EvaluateVolume();

            audioSource.Play();
            yield return new WaitUntil(() => !audioSource.isPlaying);

            onCompleteCallback?.Invoke();
            onInstanceFinished?.Invoke(this);

            audioCoroutine = null;
        }

        private void SubscribeToSettingEvents()
        {
            if (!SaveSystem.TryGetConfig(out AudioConfig config))
                return;

            config.masterVolume.onValueChanged += OnAudioSettingChanged;

            switch (instanceType)
            {
                case AudioType.SFX:
                    config.sfxVolume.onValueChanged += OnAudioSettingChanged;
                    break;
                case AudioType.MUSIC:
                    config.musicVolume.onValueChanged += OnAudioSettingChanged;
                    break;
                case AudioType.VOICE:
                    config.musicVolume.onValueChanged += OnAudioSettingChanged;
                    break;
            }
        }

        private void UnsubscribeToSettingEvents()
        {
            if (!SaveSystem.TryGetConfig(out AudioConfig config))
                return;

            config.masterVolume.onValueChanged -= OnAudioSettingChanged;
            config.sfxVolume.onValueChanged -= OnAudioSettingChanged;
            config.musicVolume.onValueChanged -= OnAudioSettingChanged;
        }

        private void OnAudioSettingChanged(float value)
        {
            audioSource.volume = EvaluateVolume();
        }

        private float EvaluateVolume()
        {
            if (!SaveSystem.TryGetConfig(out AudioConfig config))
                return 0;

            float masterVolume = config.masterVolume.Value;
            float paramMod = 1;

            switch (instanceType)
            {
                case AudioType.SFX:
                    paramMod = config.sfxVolume.Value;
                    break;
                case AudioType.MUSIC:
                    paramMod = config.musicVolume.Value;
                    break;
                case AudioType.VOICE:
                    paramMod = config.dialogueVolume.Value;
                    break;
            }

            float volume = masterVolume * paramMod;
            return volume;
        }
    }
}