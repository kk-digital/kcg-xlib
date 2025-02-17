using UtilityIO;
namespace Tests
{

    [TestFixture]
    public class PathUtilsTests
    {
        [Test]
        public void IsValidAbsolutePath_ValidPath_ReturnsTrue()
        {
            // Arrange
            string path = "/valid_path/to/file.txt";

            // Act
            bool result = PathUtils.IsValidAbsolutePath(path);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValidAbsolutePath_InvalidPath_ReturnsFalse()
        {
            // Arrange
            string path = "invalid_path/to/file.txt";

            // Act
            bool result = PathUtils.IsValidAbsolutePath(path);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void GetWorkingDirectory_Valid_ReturnsDirectory()
        {
            // Arrange
            string currentDirectory = Environment.CurrentDirectory;

            // Act
            string result = PathUtils.GetWorkingDirectory();

            // Assert
            Assert.AreEqual(currentDirectory.Replace('\\', '/'), result);
        }

        [Test]
        public void ReplaceBackSlashesWithForwardSlashes_Valid_ReturnsFixedPath()
        {
            // Arrange
            string path = "C:\\example\\file/path";
            string expected = "C:/example/file/path";

            // Act
            string result = PathUtils.ReplaceBackSlashesWithForwardSlashes(path);

            // Assert 
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void GetRelative_Valid_ReturnsRelativePath()
        {
            // Arrange
            string path = "/base/folder/sub/file.txt";
            string rootDirectory = "/base/folder";
            string expected = "sub/file.txt";

            // Act
            string result = PathUtils.GetRelative(path, rootDirectory);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}