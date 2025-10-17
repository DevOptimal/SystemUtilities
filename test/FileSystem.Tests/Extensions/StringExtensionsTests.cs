using DevOptimal.SystemUtilities.FileSystem.Extensions;

namespace DevOptimal.SystemUtilities.FileSystem.Tests.Extensions
{
    [TestClass]
    public class StringExtensionsTests
    {
        [TestMethod]
        public void RemoveInvalidPathChars_RemovesInvalidCharacters()
        {
            var input = "C:\\foo\r\nbar";
            var expected = "C:\\foobar";
            var result = input.RemoveInvalidPathChars();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReplaceInvalidPathChars_ReplacesInvalidCharacters()
        {
            var input = "C:\\foo\r\nbar";
            var expected = "C:\\foo__bar";
            var result = input.ReplaceInvalidPathChars('_');
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReplaceInvalidPathChars_WithString_ReplacesInvalidCharacters()
        {
            var input = "C:\\foo\r\nbar";
            var expected = "C:\\foo----bar";
            var result = input.ReplaceInvalidPathChars("--");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SplitOnDirectorySeparator_SplitsPathCorrectly()
        {
            var input = "C:\\foo\\bar\\baz";
            string[] expected = ["C:", "foo", "bar", "baz"];
            var result = input.SplitOnDirectorySeparator();
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        public void SplitOnPathSeparator_SplitsPathCorrectly()
        {
            var input = "C:\\foo;D:\\bar;E:\\baz";
            string[] expected = ["C:\\foo", "D:\\bar", "E:\\baz"];
            var result = input.SplitOnPathSeparator();
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
