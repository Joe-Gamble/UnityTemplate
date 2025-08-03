using UnityEngine;
using Wattle.Utils;

namespace Wattle.Infrastructure
{
    public class AudioConfig : ISaveable
    {
        private struct SoundData
        {
            public SoundData(AudioConfig config)
            {
                master = config.masterVolume.Value;
                sfx = config.sfxVolume.Value;
                music = config.musicVolume.Value;
                dialogue = config.dialogueVolume.Value;
            }

            public float master;
            public float sfx;
            public float music;
            public float dialogue;
        }

        public string FileName => "SoundSettings";

        public Observable<float> masterVolume = new Observable<float>(0.7f);
        public Observable<float> sfxVolume = new Observable<float>(0.7f);
        public Observable<float> musicVolume = new Observable<float>(0.2f);
        public Observable<float> dialogueVolume = new Observable<float>(0.9f);

        public void Deserialize(string json)
        {
            SoundData data = JsonUtility.FromJson<SoundData>(json);

            masterVolume.Value = data.master;
            sfxVolume.Value = data.sfx;
            musicVolume.Value = data.music;
            dialogueVolume.Value = data.dialogue;
        }

        public string Serialize()
        {
            string data = JsonUtility.ToJson(new SoundData(this), true);
            return data;
        }
    }
}