using System.Text;
using System.Text.RegularExpressions;

namespace Coimbra
{
    /// <summary>
    /// Utility methods to use with system paths.
    /// </summary>
    public static class PathUtility
    {
        /// <summary>
        /// Gets a regex for the given <paramref name="pattern"/>.
        /// </summary>
        /// <param name="pattern">Any '*' will be treated as wildcards.</param>
        /// <param name="ignoreCase">True if the regex should be case insensitive. False if casing should be considered.</param>
        /// <returns>The regex instance for the given pattern.</returns>
        public static Regex GetRegexFromPattern(string pattern, bool ignoreCase)
        {
            using (StringBuilderPool.Pop(out StringBuilder stringBuilder))
            {
                stringBuilder.Clear();
                stringBuilder.Append("^");

                foreach (char c in pattern)
                {
                    switch (c)
                    {
                        case '*':
                        {
                            stringBuilder.Append(".*");

                            break;
                        }

                        default:
                        {
                            stringBuilder.Append("[");

                            if (ignoreCase && char.IsLetter(c))
                            {
                                stringBuilder.Append(char.ToUpperInvariant(c));
                                stringBuilder.Append(char.ToLowerInvariant(c));
                            }
                            else
                            {
                                stringBuilder.Append(c);
                            }

                            stringBuilder.Append("]");

                            break;
                        }
                    }
                }

                stringBuilder.Append("$");

                return new Regex(stringBuilder.ToString(), RegexOptions.CultureInvariant);
            }
        }
    }
}
