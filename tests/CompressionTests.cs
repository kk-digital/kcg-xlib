using System;
using System.Text;
using CompressionUtility;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class CompressionTests
    {
        [Test]
        public void CompressLZ4_Tests()
        {
            byte[] input = Encoding.UTF8.GetBytes("TestStringCompressLZ4");
            byte[] compressed = Compression.CompressLZ4(input);
            byte[] decompressed = Compression.DecompressLZ4(compressed);
            Assert.AreEqual(Encoding.UTF8.GetString(input), Encoding.UTF8.GetString(decompressed));
        }

        [Test]
        public void DecompressLZ4_Tests()
        {
            byte[] input = Encoding.UTF8.GetBytes("TestStringDecompressLZ4");
            byte[] compressed = Compression.CompressLZ4(input);
            byte[] decompressed = Compression.DecompressLZ4(compressed);
            Assert.AreEqual(Encoding.UTF8.GetString(input), Encoding.UTF8.GetString(decompressed));
        }

        [Test]
        public void CompressLZMA_Tests()
        {
            byte[] input = Encoding.UTF8.GetBytes("TestStringCompressLZMA");
            byte[] compressed = Compression.CompressLZMA(input);
            byte[] decompressed = Compression.DecompressLZMA(compressed);
            Assert.AreEqual(Encoding.UTF8.GetString(input), Encoding.UTF8.GetString(decompressed));
        }

        [Test]
        public void DecompressLZMA_Tests()
        {
            byte[] input = Encoding.UTF8.GetBytes("TestStringDecompressLZMA");
            byte[] compressed = Compression.CompressLZMA(input);
            byte[] decompressed = Compression.DecompressLZMA(compressed);
            Assert.AreEqual(Encoding.UTF8.GetString(input), Encoding.UTF8.GetString(decompressed));
        }
    }
}