using Common.Utils;

namespace CommonTests
{
    public class EnvvironmentVariableTests
    {
        [Fact]
        public void ShouldCreateAndReadSystemEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable("DEVICE_ISOLATE", "YES", EnvironmentVariableTarget.User);
            Assert.Equal("YES", Environment.GetEnvironmentVariable("DEVICE_ISOLATE", EnvironmentVariableTarget.User));
        }
    }
}