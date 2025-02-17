namespace Tests
{

    [TestFixture]
    public class UuidTests
    {
        [Test]
        public void Test_UuidGeneration()
        {
            UInt64 uuid = libUuid.ShortHash.GenerateUUID();

            Assert.NotNull(uuid);
            Assert.GreaterOrEqual(uuid, libUuid.ShortHash.MINIMUM_STARTING_UUID);
        }
    }
}