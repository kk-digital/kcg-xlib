using System.Text;
using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Encoders;
using Decoder = SevenZip.Compression.LZMA.Decoder;
using Encoder = SevenZip.Compression.LZMA.Encoder;

namespace CompressionUtility;

public class Compression
{
	public static byte[] CompressLZ4(byte[] input)
		{
			int topupSize = 1024; 
			int blockSize = 1024;
			int extraBlocks = 0;
				
			using (var outputStream = new MemoryStream())
			using (var inputStream = new MemoryStream(input))
			{
				using (var inputReader = new BinaryReader(inputStream, Encoding.UTF8, false))
				using (var outputWriter = new BinaryWriter(outputStream, Encoding.UTF8, true))
				using (var encoder = new LZ4FastChainEncoder(blockSize, extraBlocks))
				{
					var inputBuffer = new byte[topupSize];
					var outputBuffer = new byte[LZ4Codec.MaximumOutputSize(encoder.BlockSize)];

					while (true)
					{
						var bytes = inputReader.Read(inputBuffer, 0, inputBuffer.Length);

						if (bytes == 0)
						{
							Flush(outputWriter, encoder, outputBuffer);
							outputWriter.Write(-1);
							break;
						}

						Write(outputWriter, encoder, inputBuffer, bytes, outputBuffer);
					}
				}

				return outputStream.ToArray();
			}
		}

		public static byte[] DecompressLZ4(byte[] input)
		{
			int blockSize = 1024;
			int extraBlocks = 0;
			
			using (var outputStream = new MemoryStream())
			using (var inputStream = new MemoryStream(input))
			{
				using (var inputReader = new BinaryReader(inputStream, Encoding.UTF8, false))
				using (var outputWriter = new BinaryWriter(outputStream, Encoding.UTF8, true))
				using (var decoder = new LZ4ChainDecoder(blockSize, extraBlocks))
				{
					var maximumInputBlock = LZ4Codec.MaximumOutputSize(blockSize);
					var inputBuffer = new byte[maximumInputBlock];
					var outputBuffer = new byte[blockSize];

					while (true)
					{
						var length = inputReader.ReadInt32();
						if (length < 0)
							break;
						
						inputReader.Read(inputBuffer, 0, length);

						decoder.DecodeAndDrain(
							inputBuffer,
							0,
							length,
							outputBuffer,
							0,
							outputBuffer.Length,
							out var decoded);

						outputWriter.Write(outputBuffer, 0, decoded);
					}
				}

				return outputStream.ToArray();
			}
		}


		private static void Write(
			BinaryWriter outputWriter,
			ILZ4Encoder encoder,
			byte[] inputBuffer, int bytes,
			byte[] outputBuffer)
		{
			var offset = 0;

			while (bytes > 0)
			{
				encoder.TopupAndEncode(
					inputBuffer, offset, bytes,
					outputBuffer, 0, outputBuffer.Length,
					false, false,
					out var loaded,
					out var encoded);

				if (encoded > 0)
				{
					outputWriter.Write(encoded);
					outputWriter.Write(outputBuffer, 0, encoded);
				}

				bytes -= loaded;
				offset += loaded;
			}
		}

		private static void Flush(
			BinaryWriter outputWriter, ILZ4Encoder encoder, byte[] outputBuffer)
		{
			if (encoder.BytesReady <= 0)
				return;

			var encoded = encoder.Encode(outputBuffer, 0, outputBuffer.Length, false);
			outputWriter.Write(encoded);
			outputWriter.Write(outputBuffer, 0, encoded);
		}

		public static byte[] CompressLZMA(byte[] inputData)
    {
        Encoder coder = new Encoder();
        MemoryStream input = new MemoryStream(inputData);
        MemoryStream output = new MemoryStream();

        // Write the encoder properties
        coder.WriteCoderProperties(output);

        // Write the decompressed file size.
        output.Write(BitConverter.GetBytes(input.Length), 0, 8);

        // Encode the file.
        coder.Code(input, output, input.Length, -1, null);
        return output.ToArray();
    }

    public static byte[] DecompressLZMA(byte[] inputData)
    {
        Decoder coder = new Decoder();
        MemoryStream input = new MemoryStream(inputData);
        MemoryStream output = new MemoryStream();

        // Read the decoder properties
        byte[] properties = new byte[5];
        input.Read(properties, 0, 5);

        // Read in the decompress file size.
        byte [] fileLengthBytes = new byte[8];
        input.Read(fileLengthBytes, 0, 8);
        long fileLength = BitConverter.ToInt64(fileLengthBytes, 0);

        coder.SetDecoderProperties(properties);
        coder.Code(input, output, input.Length, fileLength, null);
        
        
        return output.ToArray();
    }
}
