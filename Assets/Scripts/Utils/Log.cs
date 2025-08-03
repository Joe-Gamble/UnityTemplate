using UnityEngine;

namespace Wattle.Utils
{
    public static class LOG
    {
        public enum Type
        {
            NONE,
            GENERAL,
            STARTUP,
            UI,
            NETWORK, LOBBY, MIGRATION, SESSION,
            SAVESYSTEM,
            SCENE,
            SYSTEM,
            PLATFORM
        }

        private static bool isLoggingEnabled = true; // change this to an arg

        public static void Log(string message, Type logType = Type.GENERAL, Type secondType = Type.NONE)
        {
            if (isLoggingEnabled)
                Debug.Log(GetLogMessage(message, logType, secondType));
        }

        public static void LogWarning(string message, Type logType = Type.GENERAL, Type secondType = Type.NONE)
        {
            if (isLoggingEnabled)
                Debug.LogWarning(GetLogMessage(message, logType, secondType));
        }

        public static void LogError(string message, Type logType = Type.GENERAL, Type secondType = Type.NONE)
        {
            if (isLoggingEnabled)
                Debug.LogError(GetLogMessage(message, logType, secondType));
        }

        private static string GetLogMessage(string message, Type type, Type secondType)
        {
            string logMessage = $"{GetTypeTagText(type)} {GetTypeTagText(secondType)} {message}";
            return logMessage;

            string GetTypeTagText(Type type)
            {
                string colourTag = type switch
                {
                    Type.NONE => string.Empty,
                    Type.GENERAL => "grey",
                    Type.UI => "cyan",
                    Type.NETWORK => "red",
                    Type.SAVESYSTEM => "green",
                    Type.STARTUP => "yellow",
                    Type.SCENE => "magenta",
                    Type.SYSTEM => "white",
                    Type.PLATFORM => "orange",
                    Type.LOBBY => "#ff8351",
                    Type.MIGRATION => "#ff51ef",
                    Type.SESSION => "#ffce77",
                    _ => string.Empty
                };

                string typeText = type.ToString().Replace("_", " ");
                return string.IsNullOrEmpty(colourTag) ? string.Empty : $"<b><color={colourTag}>[{typeText}]</color></b>";
            }
        }
        public static void EnableLogging(bool enable)
        {
            isLoggingEnabled = enable;
        }
    }
}