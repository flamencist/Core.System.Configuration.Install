using Xunit;

namespace System.Configuration.Install.Tests.System.Configuration.Install
{
    public class InstallContextTests
    {
        [Fact]
        public void Should_Parse_Command_Parameters()
        {
            var installContext = new InstallContext("/var/log/log.log",new []{"-LogToConsole=true"});
            Assert.Equal("/var/log/log.log", installContext.Parameters["logFile"]);
            Assert.Equal("true", installContext.Parameters["LogToConsole"]);
        }
    }
}