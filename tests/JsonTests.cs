
using NUnit.Framework;
using libJson;

namespace Tests
{

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    
    [TestFixture]
    public class JsonTests
    {
        // Test for Serialize method
        [Test]
        public void Serialize_SimpleObject_ReturnsExpectedJson()
        {
            var obj = new { Name = "John", Age = 30 };
            string result = Json.Serialize(obj);

            Assert.AreEqual("{\"Name\":\"John\",\"Age\":30}", result);
        }

        // Test for Deserialize method
        [Test]
        public void Deserialize_ValidJson_ReturnsExpectedObject()
        {
            string json = "{\"Name\":\"John\",\"Age\":30}";
            Person result = Json.Deserialize<Person>(json);

            Assert.AreEqual("John", result.Name);
            Assert.AreEqual(30, result.Age);
        }

        // Test for Deserialize method with invalid JSON
        [Test]
        public void Deserialize_InvalidJson_ThrowsException()
        {
            string json = "InvalidJSON";

            Assert.Throws<InvalidOperationException>(() => Json.Deserialize<dynamic>(json));
        }

        // Test for IsValidJson with valid JSON
        [Test]
        public void IsValidJson_ValidJson_ReturnsTrue()
        {
            string json = "{\"Name\":\"John\",\"Age\":30}";

            bool result = Json.IsValidJson(json);

            Assert.IsTrue(result);
        }

        // Test for IsValidJson with invalid JSON
        [Test]
        public void IsValidJson_InvalidJson_ReturnsFalse()
        {
            string json = "InvalidJSON";

            bool result = Json.IsValidJson(json);

            Assert.IsFalse(result);
        }
    }
}