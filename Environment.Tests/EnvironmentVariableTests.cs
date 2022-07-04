namespace DevOptimal.System.Resources.Environment.Tests
{
    [TestClass]
    public class EnvironmentVariableTests
    {
        private MockEnvironmentProxy proxy;

        private const string name = "foo";

        private const EnvironmentVariableTarget target = EnvironmentVariableTarget.Machine;

        private const string expectedValue = "bar";

        [TestInitialize]
        public void TestInitialize()
        {
            proxy = new MockEnvironmentProxy();
        }

        [TestMethod]
        public void GetsTheCorrectValue()
        {
            proxy.environmentVariables[target][name] = expectedValue;

            var actualValue = proxy.GetEnvironmentVariable(name, target);

            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void SetsTheCorrectValue()
        {
            proxy.SetEnvironmentVariable(name, expectedValue, target);

            var actualValue = proxy.environmentVariables[target][name];

            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void SetsAVariableTwice()
        {
            proxy.SetEnvironmentVariable(name, expectedValue, target);

            var actualValue = proxy.environmentVariables[target][name];

            Assert.AreEqual(expectedValue, actualValue);

            var newValue = "baz";
            proxy.SetEnvironmentVariable(name, newValue, target);

            actualValue = proxy.environmentVariables[target][name];

            Assert.AreEqual(newValue, actualValue);
        }

        [TestMethod]
        public void SetToNullDeletesEnvironmentVariable()
        {
            proxy.environmentVariables[target][name] = expectedValue;

            proxy.SetEnvironmentVariable(name, null, target);

            Assert.IsFalse(proxy.environmentVariables[target].ContainsKey(name));
        }

        [TestMethod]
        public void GetNonexistentValueReturnsNull()
        {
            var actualValue = proxy.GetEnvironmentVariable(name, target);

            Assert.AreEqual(null, actualValue);
        }
    }
}