using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Configuration.Install.Tests.System.Configuration.Install
{
    [TestClass]
    public class InstallContextTests
    {
        [TestMethod]
        public void Should_Parse_Command_Parameters()
        {
            var installContext = new InstallContext("/var/log/log.log",new []{"-LogToConsole=true"});
            Assert.AreEqual("/var/log/log.log", installContext.Parameters["logFile"]);
            Assert.AreEqual("true", installContext.Parameters["LogToConsole"]);
        }

        [TestMethod]
        public void Should_Return_Parameter_True_For_Parameters()
        {
            var installContext = new InstallContext("/var/log/log.log", new[] { "/whatever", "/i", "-debug" });

            Assert.IsTrue(installContext.IsParameterTrue("debug"));
            Assert.IsTrue(installContext.IsParameterTrue("Debug"));
            Assert.IsTrue(installContext.IsParameterTrue("i"));
            Assert.IsTrue(installContext.IsParameterTrue("I"));
            Assert.IsTrue(installContext.IsParameterTrue("whatever"));
            Assert.IsTrue(installContext.IsParameterTrue("Whatever"));
        }
    }
}