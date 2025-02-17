using FileLib;
namespace Tests
{

    [TestFixture]
    public class FileUtilsTests
    {
        private const string TestFilePath = "C:/temp/TestFile.txt";

        [SetUp]
        public void SetUp()
        {
            if (System.IO.File.Exists(TestFilePath))
            {
                System.IO.File.Delete(TestFilePath);
            }
        }

        [Test]
        public void FileExists_ValidFilePath_ReturnsTrue()
        {
            // Arrange
            string filePath = TestFilePath;
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
            string filePath = TestFilePath;

            // Act
            bool result = FileUtils.FileExists(filePath);

            // Assert
            Assert.IsFalse(result);
        }

        // Additional tests for other methods in FileUtils can be added here.
    }
}