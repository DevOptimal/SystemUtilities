using DevOptimal.SystemUtilities.FileSystem.Extensions;

namespace DevOptimal.SystemUtilities.FileSystem.Tests.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void RemoveInvalidPathChars_RemovesInvalidCharacters()
        {
            string input = "C:\\foo\r\nbar";
            string expected = "C:\\foobar";
            string result = input.RemoveInvalidPathChars();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReplaceInvalidPathChars_ReplacesInvalidCharacters()
        {
            string input = "C:\\foo\r\nbar";
            string expected = "C:\\foo__bar";
            string result = input.ReplaceInvalidPathChars('_');
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReplaceInvalidPathChars_WithString_ReplacesInvalidCharacters()
        {
            string input = "C:\\foo\r\nbar";
            string expected = "C:\\foo----bar";
            string result = input.ReplaceInvalidPathChars("--");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SplitOnDirectorySeparator_SplitsPathCorrectly()
        {
            string input = "C:\\foo\\bar\\baz";
            string[] expected = ["C:", "foo", "bar", "baz"];
            string[] result = input.SplitOnDirectorySeparator();
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SplitOnPathSeparator_SplitsPathCorrectly()
        {
            string input = "C:\\foo;D:\\bar;E:\\baz";
            string[] expected = ["C:\\foo", "D:\\bar", "E:\\baz"];
            string[] result = input.SplitOnPathSeparator();
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
