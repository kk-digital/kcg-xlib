namespace Tests
{

    [TestFixture]
    public class AssertTests
    {
        [Test]
        public void IsTrue_ShouldNotThrow()
        {
            Assert.IsTrue(true, "Test Message");
        }

        [Test]
        public void IsTrue_ShouldThrow()
        {
            var exception = Assert.Catch(() => Assert.IsTrue(false, "Test Message"));
            Assert.IsInstanceOf(typeof(AssertionException), exception);
        }

        [Test]
        public void IsFalse_ShouldNotThrow()
        {
            Assert.IsFalse(false, "Test Message");
        }

        [Test]
        public void IsFalse_ShouldThrow()
        {
            var exception = Assert.Catch(() => Assert.IsFalse(true, "Test Message"));
            Assert.IsInstanceOf(typeof(AssertionException), exception);
        }

        [Test]
        public void AreEqual_ShouldNotThrow()
        {
            Assert.AreEqual(1, 1, "Test Message");
        }

        [Test]
        public void AreEqual_ShouldThrow()
        {
            var exception = Assert.Catch(() => Assert.AreEqual(1, 2, "Test Message"));
            Assert.IsInstanceOf(typeof(AssertionException), exception);
        }

        [Test]
        public void AreNotEqual_ShouldNotThrow()
        {
            Assert.AreNotEqual(1, 2, "Test Message");
        }

        [Test]
        public void AreNotEqual_ShouldThrow()
        {
            var exception = Assert.Catch(() => Assert.AreNotEqual(1, 1, "Test Message"));
            Assert.IsInstanceOf(typeof(AssertionException), exception);
        }

        [Test]
        public void IsNull_ShouldNotThrow()
        {
            Assert.IsNull(null, "Test Message");
        }

        [Test]
        public void IsNull_ShouldThrow()
        {
            var exception = Assert.Catch(() => Assert.IsNull("Not Null", "Test Message"));
            Assert.IsInstanceOf(typeof(AssertionException), exception);
        }

        [Test]
        public void IsNotNull_ShouldNotThrow()
        {
            Assert.IsNotNull("Not Null", "Test Message");
        }

        [Test]
        public void IsNotNull_ShouldThrow()
        {
            var exception = Assert.Catch(() => Assert.IsNotNull(null, "Test Message"));
            Assert.IsInstanceOf(typeof(AssertionException), exception);
        }
    }
}