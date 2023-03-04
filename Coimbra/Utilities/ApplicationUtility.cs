#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Coimbra
{
    /// <summary>
    /// General application utilities.
    /// </summary>
    public static class ApplicationUtility
    {
        /// <summary>
        /// Gets a value indicating whether the application is currently in Edit Mode. Always return false in a build.
        /// </summary>
        public static bool IsEditMode
        {
            get
            {
#if UNITY_EDITOR
                return !Application.isPlaying;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Gets a value indicating whether the application is currently in its first frame. Always false if <see cref="IsPlayMode"/> is false.
        /// </summary>
        public static bool IsFirstFrame => IsPlayMode && Time.frameCount == 0;

        /// <summary>
        /// Gets a value indicating whether the application is currently in Play Mode. Always return true in a build.
        /// </summary>
        public static bool IsPlayMode
        {
            get
            {
#if UNITY_EDITOR
                return Application.isPlaying;
#else
                return true;
#endif
            }
        }

        /// <summary>
        /// Gets a value indicating whether the application is quitting.
        /// </summary>
        public static bool IsQuitting { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the scripts currently reloading.
        /// </summary>
        public static bool IsReloadingScripts { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the code is running inside Unity Cloud Build.
        /// </summary>
        public static bool IsUnityCloudBuild
        {
            get
            {
#if UNITY_CLOUD_BUILD
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Gets a value indicating whether we are currently inside a <see cref="TryReadAsset"/> call.
        /// </summary>
        public static bool IsReadingAsset { get; private set; }

        /// <summary>
        /// Create a more human-readable string from the input value. Ex: CSEditorGUIUtility turns into CS Editor GUI Utility.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>The more human-readable string.</returns>
        public static string GetDisplayName(string? value)
        {
            if (IsNullOrUnderscores(ref value, out int i))
            {
                return string.Empty;
            }

            const char underscore = '_';

            using (StringBuilderPool.Pop(out StringBuilder stringBuilder))
            {
                stringBuilder.EnsureCapacity(value.Length * 2);

                char currentInput = value[i];
                char lastOutput = char.ToUpper(currentInput);
                int underscoreSequence = 0;
                int letterSequence = char.IsNumber(lastOutput) ? 0 : 1;
                stringBuilder.Append(lastOutput);

                for (i++; i < value.Length; i++)
                {
                    char lastInput = currentInput;
                    currentInput = value[i];

                    if (currentInput == underscore)
                    {
                        letterSequence = 0;
                        underscoreSequence++;

                        continue;
                    }

                    bool hasUnderscoreSequence = underscoreSequence > 1;
                    underscoreSequence = 0;

                    if (TryAppendNumber(stringBuilder, currentInput, ref lastOutput, ref letterSequence, lastInput, hasUnderscoreSequence, underscore))
                    {
                        continue;
                    }

                    if (TryAppendUpper(stringBuilder, currentInput, ref lastOutput, ref letterSequence, in value, i))
                    {
                        continue;
                    }

                    if (TryAppendLower(stringBuilder, currentInput, ref lastOutput, ref letterSequence, lastInput, underscore))
                    {
                        continue;
                    }

                    // ignore unsupported char
                    currentInput = lastInput;
                }

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Gets a prefs key to use with a given type in the format '{<see cref="CoimbraUtility.PackageName"/>}.{<see cref="Type.FullName"/>}'.
        /// </summary>
        public static string GetPrefsKey(Type type)
        {
            return $"{CoimbraUtility.PackageName}.{type.FullName}";
        }

        /// <summary>
        /// Reads an asset data and destroys it immediately. Does nothing outside the editor.
        /// </summary>
        /// <param name="assetPath">The path to the asset, can be relative to the project folder.</param>
        /// <param name="assetType">The type of the asset to be read. Will get the first instance of the given type at the given path.</param>
        /// <param name="assetData">The data in EditorJsonUtility API format.</param>
        /// <param name="prettyPrint">If true, format the output for readability. If false, format the output for minimum size. Default is false.</param>
        /// <returns>True if the data was filled with a valid result. Always returns false outside the editor.</returns>
        public static bool TryReadAsset(string assetPath, Type assetType, [NotNullWhen(true)] out string? assetData, bool prettyPrint = false)
        {
            assetData = null;
#if UNITY_EDITOR
            IsReadingAsset = true;

            Object[] objects = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(assetPath);

            foreach (Object o in objects)
            {
                if (string.IsNullOrWhiteSpace(assetData) && assetType.IsInstanceOfType(o))
                {
                    assetData = UnityEditor.EditorJsonUtility.ToJson(o, prettyPrint);
                }

                Object.DestroyImmediate(o);
            }

            IsReadingAsset = false;

            if (!string.IsNullOrWhiteSpace(assetData))
            {
                return true;
            }
#endif
            return false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void HandleSubsystemRegistration()
        {
            Application.quitting -= HandleApplicationQuitting;
            Application.quitting += HandleApplicationQuitting;
        }

        private static void HandleApplicationQuitting()
        {
            IsQuitting = true;
        }

        private static bool IsNullOrUnderscores([NotNullWhen(false)] ref string? value, out int firstIndexOfNonUnderscore)
        {
            const char underscore = '_';
            firstIndexOfNonUnderscore = 0;

            if (string.IsNullOrWhiteSpace(value))
            {
                return true;
            }

            const string startBackingField = "<";
            const string endBackingField = ">k__BackingField";

            if (value.StartsWith(startBackingField) && value.EndsWith(endBackingField))
            {
                value = value.Substring(1, value.Length - startBackingField.Length - endBackingField.Length);
            }

            if (value.Length > 1 && value[1] == underscore)
            {
                firstIndexOfNonUnderscore += 2;
            }

            if (value.Length <= firstIndexOfNonUnderscore)
            {
                return true;
            }

            while (value[firstIndexOfNonUnderscore] == underscore)
            {
                firstIndexOfNonUnderscore++;

                if (value.Length != firstIndexOfNonUnderscore)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private static bool TryAppendLower(StringBuilder stringBuilder, char currentInput, ref char lastOutput, ref int letterSequence, char lastInput, char underscore)
        {
            if (!char.IsLower(currentInput))
            {
                return false;
            }

            if (char.IsNumber(lastOutput) || lastInput == underscore)
            {
                lastOutput = char.ToUpper(currentInput);
                stringBuilder.Append(' ');
                stringBuilder.Append(lastOutput);
                letterSequence++;

                return true;
            }

            if (char.IsLower(lastOutput))
            {
                lastOutput = currentInput;
                stringBuilder.Append(lastOutput);
                letterSequence++;

                return true;
            }

            if (letterSequence == 0)
            {
                lastOutput = char.ToUpper(currentInput);
                stringBuilder.Append(lastOutput);
            }
            else
            {
                lastOutput = currentInput;
                stringBuilder.Append(lastOutput);
            }

            letterSequence++;

            return true;
        }

        private static bool TryAppendNumber(StringBuilder stringBuilder, char currentInput, ref char lastOutput, ref int letterSequence, char lastInput, bool hasUnderscoreSequence, char underscore)
        {
            if (!char.IsNumber(currentInput))
            {
                return false;
            }

            letterSequence = 0;

            if (char.IsNumber(lastOutput))
            {
                if (lastInput == underscore)
                {
                    stringBuilder.Append(hasUnderscoreSequence ? ' ' : '.');
                }
            }
            else
            {
                stringBuilder.Append(' ');
            }

            lastOutput = currentInput;
            stringBuilder.Append(lastOutput);

            return true;
        }

        private static bool TryAppendUpper(StringBuilder stringBuilder, char currentInput, ref char lastOutput, ref int letterSequence, in string rawInput, int currentIndex)
        {
            if (!char.IsUpper(currentInput))
            {
                return false;
            }

            if (char.IsNumber(lastOutput) || char.IsLower(lastOutput))
            {
                stringBuilder.Append(' ');
            }
            else if (char.IsUpper(lastOutput) && currentIndex + 1 < rawInput.Length && char.IsLower(rawInput[currentIndex + 1]))
            {
                stringBuilder.Append(' ');
            }

            lastOutput = currentInput;
            stringBuilder.Append(lastOutput);
            letterSequence++;

            return true;
        }
    }
}
