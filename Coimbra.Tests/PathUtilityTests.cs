using NUnit.Framework;

namespace Coimbra.Tests
{
    [TestOf(typeof(PathUtility))]
    public class PathUtilityTests
    {
        [TestCase("Path/To/File", "Path/*/File", false, true)]
        [TestCase("value", "*lu*", false, true)]
        [TestCase("path/to/file", "Path/To/*", true, true)]
        [TestCase("VALUE", "*lue", true, true)]
        [TestCase("path/to/file", "Path/*/File", false, false)]
        [TestCase("path/to-file", "Path/To/*", true, false)]
        public void GetRegexFromPattern_IsMatch_TestCases(string input, string pattern, bool ignoreCase, bool expected)
        {
            Assert.That(PathUtility.GetRegexFromPattern(pattern, ignoreCase).IsMatch(input), Is.EqualTo(expected));
        }
    }
}
