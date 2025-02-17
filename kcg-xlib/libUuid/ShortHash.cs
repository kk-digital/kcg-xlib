
namespace libUuid;


public class ShortHash
{
    // first 1000 Ids are reserved
    public const UInt64 MINIMUM_TILE_ID = 1000UL;
    
    private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
    private const int HASH_LENGTH = 11;
    
    public static UInt64 GenerateUUID()
    {
        UInt64 uuid = 0UL;
        
        // Make sure to generate a valid UUID
        while (uuid < MINIMUM_TILE_ID)
        {
            int unixTime32Bit = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() & 0xFFFFFFFF);
            int random32Bit   = Random.Shared.Next(); 
            uuid = ((UInt64)random32Bit & 0xFFFFFFFFUL) | ((UInt64)unixTime32Bit << 32);
        }

        return uuid;
    }
    
}