#if !UNITY_2020_1_OR_NEWER
using System.IO;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
    internal class ScriptableSingleton<T> : ScriptableObject
        where T : ScriptableObject
    {
        private static T _instance;

        protected ScriptableSingleton()
        {
            if (_instance != null)
            {
                Debug.LogError("ScriptableSingleton already exists. Did you query the singleton in a constructor?");
            }
            else
            {
                _instance = (object)this as T;
            }
        }

        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    CreateAndLoad();
                }

                return _instance;
            }
        }

        protected static string GetFilePath()
        {
            foreach (object customAttribute in typeof(T).GetCustomAttributes(true))
            {
                if (customAttribute is FilePathAttribute)
                {
                    return (customAttribute as FilePathAttribute).filepath;
                }
            }

            return string.Empty;
        }

        protected virtual void Save(bool saveAsText)
        {
            if (_instance == null)
            {
                Debug.LogError("Cannot save ScriptableSingleton: no instance!");
            }
            else
            {
                string filePath = GetFilePath();

                if (string.IsNullOrEmpty(filePath))
                {
                    return;
                }

                string directoryName = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                InternalEditorUtility.SaveToSerializedFileAndForget(new T[]
                {
                    _instance,
                }, filePath, saveAsText);
            }
        }

        private static void CreateAndLoad()
        {
            string filePath = GetFilePath();

            if (!string.IsNullOrEmpty(filePath))
            {
                InternalEditorUtility.LoadSerializedFileAndForget(filePath);
            }

            if (!(_instance == null))
            {
                return;
            }

            CreateInstance<T>().hideFlags = HideFlags.HideAndDontSave;
        }
    }
}
#endif
