using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using assert;

namespace lib
{
    /// <summary>
    /// Represents a 64-bit UUID.
    /// </summary>
    public class Uuid64
    {
        private readonly ulong _value;

        /// <summary>
        /// Gets the default UUID with all zeros.
        /// </summary>
        public static Uuid64 Default => new Uuid64(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="Uuid64"/> class.
        /// </summary>
        /// <param name="value">The 64-bit value of the UUID.</param>
        /// <exception cref="ArgumentException">Thrown when the value is zero.</exception>
        public Uuid64(ulong value)
        {
            Utils.Assert(value != 0, "Use one of the static methods to get a Uuid64 instance");

            _value = value;
        }

        /// <summary>
        /// Returns the string representation of the UUID.
        /// </summary>
        /// <returns>The formatted UUID string.</returns>
        public override string ToString()
        {
            var hexString = _value.ToString("X16");
            var formattedString = $"{hexString.Substring(0, 4)}-{hexString.Substring(4, 4)}-{hexString.Substring(8, 4)}-{hexString.Substring(12, 4)}";
            ValidateUuidString(formattedString);

            return formattedString.ToUpper();
        }

        /// <summary>
        /// Loads a UUID from various input formats.
        /// </summary>
        /// <param name="inputValue">The input value.</param>
        /// <returns>The loaded UUID.</returns>
        /// <exception cref="ArgumentException">Thrown when the input value is not supported.</exception>
        public static Uuid64 LoadFromAny(object inputValue)
        {
            return inputValue switch
            {
                string strValue when strValue.StartsWith("def") => FromDefinedString(strValue),
                string strValue when strValue.StartsWith("ref") => FromReferencedString(strValue),
                string strValue => FromFormattedString(strValue),
                int intValue => FromUInt64((ulong)intValue),
                Uuid64 uuidValue => uuidValue,
                _ => throw new ArgumentException("Input value not supported"),
            };
        }

        /// <summary>
        /// Creates a new UUID with the specified date and hash.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <param name="hash">The hash value.</param>
        /// <returns>The created UUID.</returns>
        public static Uuid64 CreateNewUuid(DateTime? dateValue = null, Hash? hash = null)
        {
            dateValue ??= DateTime.UtcNow;

            return hash != null
                ? new Uuid64(CreateRandomValueWithDateAndHash(dateValue.Value, hash))
                : new Uuid64(CreateRandomValueWithDate(dateValue.Value));
        }

        /// <summary>
        /// Creates a new UUID from a date string.
        /// </summary>
        /// <param name="date">The date string.</param>
        /// <param name="dateFormats">The list of date formats.</param>
        /// <returns>The created UUID.</returns>
        /// <exception cref="ArgumentException">Thrown when the date formats list is empty or the date does not match any format.</exception>
        public static Uuid64 CreateNewFromDateString(string date, List<string> dateFormats)
        {
            Utils.Assert(dateFormats != null && dateFormats.Count != 0, "date_formats must include at least one format");

            foreach (var fmt in dateFormats)
            {
                if (DateTime.TryParseExact(date, fmt, null, System.Globalization.DateTimeStyles.None, out var dateValue))
                {
                    return new Uuid64(CreateRandomValueWithDate(dateValue));
                }
            }

            Utils.Assert(false, $"time data '{date}' does not match any of the provided formats");
            return InvalidUuid();
        }

        /// <summary>
        /// Creates a UUID from a 64-bit value.
        /// </summary>
        /// <param name="value">The 64-bit value.</param>
        /// <returns>The created UUID.</returns>
        public static Uuid64 FromUInt64(ulong value)
        {
            ValidateUuidIntValue(value);
            return new Uuid64(value);
        }
        
        public static Uuid64 InvalidUuid()
        {
            return new Uuid64(1);
        }

        /// <summary>
        /// Creates a UUID from a formatted string.
        /// </summary>
        /// <param name="value">The formatted string.</param>
        /// <returns>The created UUID.</returns>
        public static Uuid64 FromFormattedString(string value)
        {
            value = value.ToUpper();
            ValidateUuidString(value);
            return new Uuid64(Convert.ToUInt64(value.Replace("-", ""), 16));
        }

        /// <summary>
        /// Compares two UUIDs for equality.
        /// </summary>
        /// <param name="uuid1">The first UUID.</param>
        /// <param name="uuid2">The second UUID.</param>
        /// <returns>True if the UUIDs are equal, otherwise false.</returns>
        public static bool CompareUuids(Uuid64 uuid1, Uuid64 uuid2)
        {
            return uuid1._value == uuid2._value;
        }

        /// <summary>
        /// Converts the UUID to a 64-bit value.
        /// </summary>
        /// <returns>The 64-bit value.</returns>
        public ulong ToUInt64()
        {
            ValidateUuidIntValue(_value);
            return _value;
        }

        /// <summary>
        /// Converts the UUID to a formatted string.
        /// </summary>
        /// <returns>The formatted string.</returns>
        public string ToFormattedStr()
        {
            return ToString();
        }

        /// <summary>
        /// Converts the UUID to a defined string.
        /// </summary>
        /// <returns>The defined string.</returns>
        public string ToDefinedString()
        {
            return $"def:{ToFormattedStr()}";
        }

        /// <summary>
        /// Converts the UUID to a referenced string.
        /// </summary>
        /// <returns>The referenced string.</returns>
        public string ToReferencedString()
        {
            return $"ref:{ToFormattedStr()}";
        }

        /// <summary>
        /// Creates a UUID from a defined string.
        /// </summary>
        /// <param name="value">The defined string.</param>
        /// <returns>The created UUID.</returns>
        /// <exception cref="ArgumentException">Thrown when the string format is invalid.</exception>
        public static Uuid64 FromDefinedString(string value)
        {
            Utils.Assert(value.StartsWith("def:"), "Invalid defined string format");
            return FromFormattedString(value.Substring(4));
        }

        /// <summary>
        /// Creates a UUID from a referenced string.
        /// </summary>
        /// <param name="value">The referenced string.</param>
        /// <returns>The created UUID.</returns>
        /// <exception cref="ArgumentException">Thrown when the string format is invalid.</exception>
        public static Uuid64 FromReferencedString(string value)
        {
            Utils.Assert(value.StartsWith("ref:"), "Invalid referenced string format");
            return FromFormattedString(value.Substring(4));
        }

        /// <summary>
        /// Validates the UUID string format.
        /// </summary>
        /// <param name="uuidString">The UUID string.</param>
        /// <exception cref="ArgumentException">Thrown when the string format is invalid or the date is in the future.</exception>
        private static void ValidateUuidString(string uuidString)
        {
            var validationRegex = new Regex(@"^[0-9A-F]{4}\b-[0-9A-F]{4}\b-[0-9A-F]{4}\b-[0-9A-F]{4}$");

            Utils.Assert(validationRegex.IsMatch(uuidString), "Invalid UUID string");

            var uuidPosixDate = Convert.ToUInt32(uuidString.Replace("-", "").Substring(0, 8), 16);
            ValidateDateNotInTheFuture(uuidPosixDate);
        }

        /// <summary>
        /// Validates the UUID integer value.
        /// </summary>
        /// <param name="uuidInt">The UUID integer value.</param>
        /// <exception cref="ArgumentException">Thrown when the value is not valid or the date is in the future.</exception>
        private static void ValidateUuidIntValue(ulong uuidInt)
        {
            Utils.Assert((uuidInt >= 0 && uuidInt <= 18446744073709551615), "The value is not a valid UUID");

            var uuidPosixDate = (uint)(uuidInt >> 32);
            ValidateDateNotInTheFuture(uuidPosixDate);
        }

        /// <summary>
        /// Validates that the date is not in the future.
        /// </summary>
        /// <param name="posixDateValue">The POSIX date value.</param>
        /// <exception cref="ArgumentException">Thrown when the date is more than one hour in the future.</exception>
        private static void ValidateDateNotInTheFuture(uint posixDateValue)
        {
            var currentPosixDate = (uint)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() & 0xFFFFFFFF);

            Utils.Assert(!(posixDateValue > currentPosixDate + 3600), "The UUID date part must not be more than one hour in the future");
        }

        /// <summary>
        /// Creates a random value with the specified date.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <returns>The created random value.</returns>
        private static ulong CreateRandomValueWithDate(DateTime dateValue)
        {
            var unixTime32bit = (uint)(new DateTimeOffset(dateValue).ToUnixTimeSeconds() & 0xFFFFFFFF);
            ValidateDateNotInTheFuture(unixTime32bit);

            var random32bit = (uint)RandomNumberGenerator.GetInt32(int.MaxValue);
            return ((ulong)random32bit & 0xFFFFFFFF) | ((ulong)unixTime32bit << 32);
        }

        /// <summary>
        /// Creates a random value with the specified date and hash.
        /// </summary>
        /// <param name="dateValue">The date value.</param>
        /// <param name="hashContent">The hash content.</param>
        /// <returns>The created random value.</returns>
        private static ulong CreateRandomValueWithDateAndHash(DateTime dateValue, Hash hashContent)
        {
            var unixTime32bit = (uint)(new DateTimeOffset(dateValue).ToUnixTimeSeconds() & 0xFFFFFFFF);
            ValidateDateNotInTheFuture(unixTime32bit);

            var hashValue = hashContent.ToInt();
            var lower32BitsOfHash = (uint)(hashValue & 0xFFFFFFFF);

            return ((ulong)lower32BitsOfHash & 0xFFFFFFFF) | ((ulong)unixTime32bit << 32);
        }
    }
}