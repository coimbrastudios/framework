#if !UNITY_2020_1_OR_NEWER
using System;
using UnityEditorInternal;
using UnityEngine;

namespace UnityEditor
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class FilePathAttribute : Attribute
    {
        public enum Location
        {
            PreferencesFolder,
            ProjectFolder,
        }

        private readonly Location _location;
        private string _filePath;
        private string _relativePath;

        public FilePathAttribute(string relativePath, Location location)
        {
            _relativePath = !string.IsNullOrEmpty(relativePath) ? relativePath : throw new ArgumentException("Invalid relative path (it is empty)");
            _location = location;
        }

        internal string filepath
        {
            get
            {
                if (_filePath == null && _relativePath != null)
                {
                    _filePath = CombineFilePath(_relativePath, _location);
                    _relativePath = null;
                }

                return _filePath;
            }
        }

        private static string CombineFilePath(string relativePath, Location location)
        {
            if (relativePath[0] == '/')
            {
                relativePath = relativePath.Substring(1);
            }

            switch (location)
            {
                case Location.PreferencesFolder:
                    return InternalEditorUtility.unityPreferencesFolder + "/" + relativePath;

                case Location.ProjectFolder:
                    return relativePath;

                default:
                    Debug.LogError("Unhandled enum: " + location);

                    return relativePath;
            }
        }
    }
}
#endif
