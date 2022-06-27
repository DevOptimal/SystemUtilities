namespace bradselw.System.Resources.Environment.Tests
{
    [TestClass]
    public class EnvironmentVariableTests
    {
        private MockEnvironmentProxy proxyUnderTest;

        [TestInitialize]
        public void TestInitialize()
        {
            proxyUnderTest = new MockEnvironmentProxy();
        }

        [TestMethod]
        public void GetsTheCorrectValue()
        {
            var name = "foo";
            var target = EnvironmentVariableTarget.Machine;
            var expectedValue = "bar";

            proxyUnderTest.environmentVariables[target][name] = expectedValue;

            var actualValue = proxyUnderTest.GetEnvironmentVariable(name, target);

            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void GetNonexistentValueReturnsNull()
        {
            var name = "foo";
            var target = EnvironmentVariableTarget.Machine;

            var actualValue = proxyUnderTest.GetEnvironmentVariable(name, target);

            Assert.AreEqual(null, actualValue);
        }
    }
}