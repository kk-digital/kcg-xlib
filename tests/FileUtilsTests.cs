using FileLib;
using UtilityIO;
namespace Tests
{

    [TestFixture]
    public class FileUtilsTests
    {
        private const string TestFilePath = "TestFile.txt";

        [SetUp]
        public void SetUp()
        {
            if (System.IO.File.Exists(PathUtils.GetWorkingDirectory() + "/" + TestFilePath.Replace('\\', '/')))
            {
                System.IO.File.Delete(PathUtils.GetWorkingDirectory() + "/" + TestFilePath.Replace('\\', '/'));
            }
        }

        [Test]
        public void FileExists_ValidFilePath_ReturnsTrue()
        {
            // Arrange
            string filePath = PathUtils.GetWorkingDirectory() + "/" + TestFilePath.Replace('\\', '/');
            System.IO.File.Create(filePath).Close();

            // Act
            bool result = FileUtils.FileExists(filePath);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void FileExists_InvalidFilePath_ReturnsFalse()
        {
            // Arrange
            string filePath = PathUtils.GetWorkingDirectory() + "/" + TestFilePath.Replace('\\', '/');

            // Act
            bool result = FileUtils.FileExists(filePath);

            // Assert
            Assert.IsFalse(result);
        }

        // Additional tests for other methods in FileUtils can be added here.
    }
}