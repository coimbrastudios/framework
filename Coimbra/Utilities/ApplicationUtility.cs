using System;
using System.Text;
using UnityEngine;

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
        /// Create a more human-readable string from the input value. Ex: CSEditorGUIUtility turns into CS Editor GUI Utility.
        /// </summary>
        /// <param name="value">The input value.</param>
        /// <returns>The more human-readable string.</returns>
        public static string GetDisplayName(string value)
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

        private static bool IsNullOrUnderscores(ref string value, out int firstIndexOfNonUnderscore)
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
