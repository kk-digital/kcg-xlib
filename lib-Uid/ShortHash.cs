
namespace libUid;


public class ShortHash
{
    // first 1000 Ids are reserved
    public const UInt64 MINIMUM_STARTING_UUID = 1000UL;
    
    private const string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";
    private const int HASH_LENGTH = 11;
    
    public static UInt64 GenerateUUID()
    {
        UInt64 Uid = 0UL;
        
        // Make sure to generate a valid Uid
        while (Uid < MINIMUM_STARTING_UUID)
        {
            int unixTime32Bit = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() & 0xFFFFFFFF);
            int random32Bit   = Random.Shared.Next(); 
            Uid = ((UInt64)random32Bit & 0xFFFFFFFFUL) | ((UInt64)unixTime32Bit << 32);
        }

        return Uid;
    }
    
}