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
            string path = PathUtils.GetWorkingDirectory() + "/valid_path/to/file.txt";

            // Act
            bool result = PathUtils.IsValidAbsolutePath(path);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IsValidAbsolutePath_InvalidPath_ReturnsFalse()
        {
            // Arrange
            string path = PathUtils.GetWorkingDirectory() + "C:/file.txt";

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
            string path = Path.Combine(PathUtils.GetWorkingDirectory(), "example\\file\\path");
            string expected = PathUtils.ReplaceBackSlashesWithForwardSlashes(path);

            // Act
            string result = PathUtils.ReplaceBackSlashesWithForwardSlashes(path);

            // Assert 
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void GetRelative_Valid_ReturnsRelativePath()
        {
            // Arrange
            string path = PathUtils.GetWorkingDirectory() + "/base/folder/sub/file.txt";
            string rootDirectory = PathUtils.GetWorkingDirectory() + "/base/folder";
            string expected = "sub/file.txt";

            // Act
            string result = PathUtils.GetRelative(path, rootDirectory);

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}