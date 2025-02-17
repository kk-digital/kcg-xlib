using NUnit.Framework;
using System.IO;
using lib;

namespace Tests
{
    [TestFixture]
    public class HashTests
    {
        [Test]
        public void CreateHashFromFile()
        {
            var filePath = "testfile.txt";
            File.WriteAllText(filePath, "Test content");
            var hash = Hash.Create(filePath);
            Assert.IsNotNull(hash);
            File.Delete(filePath);
        }

        [Test]
        public void CreateHashFromByteArray()
        {
            var byteArray = System.Text.Encoding.UTF8.GetBytes("Test content");
            var hash = Hash.FromByteArray(byteArray);
            Assert.IsNotNull(hash);
        }

        [Test]
        public void ToStringTest()
        {
            var byteArray = System.Text.Encoding.UTF8.GetBytes("Test content");
            var hash = Hash.FromByteArray(byteArray);
            var hexString = hash.ToString();
            Assert.IsNotEmpty(hexString);
        }

        [Test]
        public void ToIntTest()
        {
            // Arrange: Convert string to a byte array.
            var byteArray = System.Text.Encoding.UTF8.GetBytes("Test content");
    
            // Act: Generate the hash and convert it to an integer.
            var hash = Hash.FromByteArray(byteArray);
            var intValue = hash.ToInt();
    
            // Assert: Ensure that the integer value is not zero.
            Assert.AreNotEqual(0, intValue);
        }


        [Test]
        public void ToBinaryTest()
        {
            var byteArray = System.Text.Encoding.UTF8.GetBytes("Test content");
            var hash = Hash.FromByteArray(byteArray);
            var binary = hash.ToBinary();
            Assert.IsNotEmpty(binary);
        }
    }
}