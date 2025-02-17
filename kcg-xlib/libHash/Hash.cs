
using Blake2Fast;

namespace lib
{
    /// <summary>
    /// This class defines the standard hash we use (BLAKE2s).
    /// </summary>
    public class Hash
    {
        private readonly byte[] _hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="Hash"/> class from a file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        public Hash(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            using (var fileStream = File.OpenRead(filePath))
            {
                using (var memoryStream = new MemoryStream())
                {
                    fileStream.CopyTo(memoryStream);
                    _hash = Blake2s.ComputeHash(memoryStream.ToArray());
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hash"/> class from a byte array.
        /// </summary>
        /// <param name="byteArray">The byte array.</param>
        private Hash(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
            {
                throw new ArgumentException("Byte array cannot be null or empty", nameof(byteArray));
            }

            _hash = Blake2s.ComputeHash(byteArray);
        }

        /// <summary>
        /// Asynchronously initializes a new instance of the <see cref="Hash"/> class from a file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task<Hash> CreateAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
            }

            byte[] hashBytes;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                using (var memoryStream = new MemoryStream())
                {
                    await fileStream.CopyToAsync(memoryStream);
                    hashBytes = Blake2s.ComputeHash(memoryStream.ToArray());
                }
            }

            return new Hash(hashBytes);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Hash"/> class from a byte array.
        /// </summary>
        /// <param name="byteArray">The byte array.</param>
        /// <returns>A new instance of the <see cref="Hash"/> class.</returns>
        public static Hash FromByteArray(byte[] byteArray)
        {
            return new Hash(byteArray);
        }

        /// <summary>
        /// Returns the hexadecimal representation of the hash.
        /// </summary>
        /// <returns>The hexadecimal representation of the hash.</returns>
        public override string ToString()
        {
            return BitConverter.ToString(_hash).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Returns the integer representation of the hash.
        /// </summary>
        /// <returns>The integer representation of the hash.</returns>
        public int ToInt()
        {
            return BitConverter.ToInt32(_hash, 0);
        }

        /// <summary>
        /// Recreates a hash object from an integer.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <returns>A new instance of the <see cref="Hash"/> class.</returns>
        public static Hash FromInt(int value)
        {
            var byteArray = BitConverter.GetBytes(value);
            return FromByteArray(byteArray);
        }

        /// <summary>
        /// Recreates a hash object from a hexadecimal string.
        /// </summary>
        /// <param name="hexStr">The hexadecimal string.</param>
        /// <returns>A new instance of the <see cref="Hash"/> class.</returns>
        public static Hash FromString(string hexStr)
        {
            if (string.IsNullOrEmpty(hexStr))
            {
                throw new ArgumentException("Hex string cannot be null or empty", nameof(hexStr));
            }

            var byteArray = Convert.FromHexString(hexStr);
            return FromByteArray(byteArray);
        }

        /// <summary>
        /// Returns the binary representation of the hash.
        /// </summary>
        /// <returns>The binary representation of the hash.</returns>
        public byte[] ToBinary()
        {
            return _hash;
        }
    }
}