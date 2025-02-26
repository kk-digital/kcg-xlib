namespace Tests
{

    [TestFixture]
    public class UuidTests
    {
        [Test]
        public void Test_UuidGeneration()
        {
            UInt64 Uid = libUid.ShortHash.GenerateUID();

            Assert.NotNull(Uid);
            Assert.GreaterOrEqual(Uid, libUid.ShortHash.MINIMUM_STARTING_UID);
        }
    }
}