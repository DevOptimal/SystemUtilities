using DevOptimal.SystemUtilities.Environment.Abstractions;
using System;

namespace DevOptimal.SystemUtilities.Environment.Tests
{
    [TestClass]
    public class EnvironmentVariableTests
    {
        private MockEnvironment environment;

        private const string name = "foo";

        private const EnvironmentVariableTarget target = EnvironmentVariableTarget.Machine;

        private const string expectedValue = "bar";

        [TestInitialize]
        public void TestInitialize()
        {
            environment = new MockEnvironment();
        }

        [TestMethod]
        public void GetsTheCorrectValue()
        {
            environment.data[target][name] = expectedValue;

            var actualValue = environment.GetEnvironmentVariable(name, target);

            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void SetsTheCorrectValue()
        {
            environment.SetEnvironmentVariable(name, expectedValue, target);

            var actualValue = environment.data[target][name];

            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void SetsAVariableTwice()
        {
            environment.SetEnvironmentVariable(name, expectedValue, target);

            var actualValue = environment.data[target][name];

            Assert.AreEqual(expectedValue, actualValue);

            var newValue = "baz";
            environment.SetEnvironmentVariable(name, newValue, target);

            actualValue = environment.data[target][name];

            Assert.AreEqual(newValue, actualValue);
        }

        [TestMethod]
        public void SetToNullDeletesEnvironmentVariable()
        {
            environment.data[target][name] = expectedValue;

            environment.SetEnvironmentVariable(name, null, target);

            Assert.IsFalse(environment.data[target].ContainsKey(name));
        }

        [TestMethod]
        public void GetNonexistentValueReturnsNull()
        {
            var actualValue = environment.GetEnvironmentVariable(name, target);

            Assert.IsNull(actualValue);
        }
    }
}