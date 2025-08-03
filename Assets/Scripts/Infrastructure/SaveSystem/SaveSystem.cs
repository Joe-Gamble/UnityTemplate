using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Wattle.Utils;

namespace Wattle.Infrastructure
{
    public class SaveSystem : Singleton<SaveSystem>
    {
        public static bool TryGetConfig<T>(out T config) where T : ISaveable
        {
            config = default;

            foreach (ISaveable _config in Instance.configs)
            {
                if (typeof(T) == _config.GetType())
                {
                    config = (T)_config;
                    return true;
                } 
            }

            LOG.LogError($"Could not find config of type: {typeof(T).Name}", LOG.Type.SAVESYSTEM);
            return false;
        }

        private const string CONFIG_DIRECTORY = "Config";
        private static string configPath;

        private readonly List<ISaveable> configs = new List<ISaveable>();

        public override IEnumerator Initalise()
        {
            configPath = $"{Application.persistentDataPath}/{CONFIG_DIRECTORY}";
            initialised = true;

            Task.Run(() => LoadConfigs(configPath, () =>
            {
                initialised = true;
            }));

            yield return base.Initalise();
        }


        private async Task LoadConfigs(string configPath, Action onComplete = null)
        {
            List<Task> tasks = new List<Task>();
            configs.AddRange(InitialiseConfigs());

#if !UNITY_WEBGL
            if (!Directory.Exists(configPath))
            {
                LOG.Log($"Directory does not exist, creating: {configPath}", LOG.Type.SAVESYSTEM);
                Directory.CreateDirectory(configPath);
            }

            // ADD CONDIGS HERE
            foreach (ISaveable config in configs)
            {
                tasks.Add(LoadConfig(config));
            }
#endif
            await Task.WhenAll(tasks);

            onComplete?.Invoke();
        }

        public static async Task LoadConfig(ISaveable config)
        {
            await Task.Yield();
#if !UNITY_WEBGL
            string filePath = $"{configPath}/{config.FileName}.json";

            if (File.Exists(filePath))
            {
                string json = await File.ReadAllTextAsync(filePath);
                config.Deserialize(json);

                LOG.Log($"CONFIG: [{config.FileName}] Loaded from {filePath}", LOG.Type.SAVESYSTEM);
            }
            else
            {
                await SaveConfig(config);

                string json = await File.ReadAllTextAsync(filePath);
                config.Deserialize(json);

                LOG.Log($"CONFIG: {config.FileName} Created and Loaded from {filePath}", LOG.Type.SAVESYSTEM);
            }
#endif
        }

        public static async Task SaveConfig(ISaveable config)
        {
            string filePath = $"{configPath}/{config.FileName}.json";

            string json = config.Serialize();
            await File.WriteAllTextAsync(filePath, json);

            LOG.Log($"CONFIG: {config.FileName} Saved - At: {filePath}", LOG.Type.SAVESYSTEM);
        }

        public void ResetConfigs()
        {
            configs.Clear();
            configs.AddRange(InitialiseConfigs());

            LOG.Log($"Configs reset to defaults", LOG.Type.SAVESYSTEM);
        }

        private List<ISaveable> InitialiseConfigs()
        {
            List<ISaveable> configs = new List<ISaveable>();
            configs.Add(new AudioConfig());

            return configs;
        }

        public void SaveConfigs()
        {
            foreach (ISaveable config in configs)
            {
                config.Save();
            }

            LOG.Log($"Configs saved.", LOG.Type.SAVESYSTEM);
        }
    }
}