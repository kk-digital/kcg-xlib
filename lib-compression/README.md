# lib-compression

The `lib-compression` library is designed to provide efficient compression and decompression functionality to your C# applications.
It supports two widely-used algorithms: **LZ4** and **LZMA**. 
The library allows for high-speed data compression and decompression with a balance between performance and compression ratio.

## Features

- **Compression and Decompression:** Supports both LZ4 and LZMA algorithms.
- **Performance:** Optimized for speed.
- **Flexibility:** Provides detailed control over compression behavior.

## Supported Algorithms

- **LZ4:** A fast compression algorithm suitable for real-time applications.
- **LZMA:** Known for high compression ratios but may require more processing time.

## Dependencies

This library has the following dependencies:

- `K4os.Compression.LZ4` for LZ4 compression.
- `SevenZip.Compression.LZMA` for LZMA compression.

Ensure to install these dependencies in the environment where this library is used.

## How To Use

### Example Of Using LZ4

```csharp
using CompressionUtility;
using System.Text;

// Sample byte array
data byte[] = Encoding.UTF8.GetBytes("Sample text to compress");

// Compression
byte[] compressedData = Compression.CompressLZ4(data);

// Decompression
byte[] decompressedData = Compression.DecompressLZ4(compressedData);

```

### Example Of Using LZMA

```csharp
using CompressionUtility;
using System.Text;

// Sample byte array
data byte[] = Encoding.UTF8.GetBytes("Sample text to compress");

// Compression
byte[] compressedData = Compression.CompressLZMA(data);

// Decompression
byte[] decompressedData = Compression.DecompressLZMA(compressedData);

```

## Limitations And Known Issues

- **Performance:** LZMA may have slower compression times compared to LZ4, but offers better compression ratios.
- **Thread Safety:** Ensure proper usage of library within multi-threaded applications to avoid any potential conflicts.

## Future Improvements

- Integration with more compression algorithms.
- Further optimizations for performance.

---

**Note:** Always review and test the latest versions of the library and its dependencies to ensure compatibility and to take advantage of improvements and bug fixes.