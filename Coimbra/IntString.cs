using System.Collections.Generic;

namespace Coimbra
{
    /// <summary>
    /// Helper class to avoid allocating each time you are converting an <see cref="int"/> to a <see cref="string"/>.
    /// </summary>
    public static class IntString
    {
        private static readonly Dictionary<int, string> Cache = new();

        /// <summary>
        /// Gets the <see cref="string"/> for the specified <see cref="int"/>..
        /// </summary>
        public static string Get(int i)
        {
            if (Cache.TryGetValue(i, out string value))
            {
                return value;
            }

            value = i.ToString();
            Cache.Add(i, value);

            return value;
        }
    }
}
