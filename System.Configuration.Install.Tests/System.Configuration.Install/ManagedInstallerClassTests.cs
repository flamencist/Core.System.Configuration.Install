using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSTestExtensions;

namespace System.Configuration.Install.Tests.System.Configuration.Install
{
    [TestClass]
    public class ManagedInstallerClassTests
    {

        [TestMethod]
        public void Should_Install_UnInstall_Component()
        {
            const string fileName = "test";
            var installStateFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.InstallState";
            var installLogFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.InstallLog";

            InstallComponent();
            Assert.IsTrue(File.Exists(fileName));
            Assert.IsTrue(File.Exists(installStateFileName));
            
            UnInstallComponent();
            Assert.IsFalse(File.Exists(fileName));
            Assert.IsFalse(File.Exists(installStateFileName));    
            Assert.IsTrue(File.Exists(installLogFileName));
        }

        [TestMethod]
        public void Should_Rollback_Component()
        {
            var log = new StringBuilder();
            InstallerLogHandler.Instance.OnLog += (source, message) => { log.AppendLine(message); };
            Assert.ThrowsException<InvalidOperationException>(() => ManagedInstallerClass.InstallHelper(new[]
                {"-ThrowException=True", "-AssemblyName",  Assembly.GetExecutingAssembly().GetName().Name}));
            StringAssert.Contains(log.ToString(), "The ThrowException parameter is detected.");
        }

        [TestMethod]
        public void Should_Show_Help()
        {
            const string helpMessage = "Usage: InstallUtil [-u | -uninstall] [option [...]] assembly [[option [...]] assembly] [...]]";
            Assert.ThrowsException<InvalidOperationException>(() => ManagedInstallerClass.InstallHelper(new[] {"-help"}),helpMessage, ExceptionMessageCompareOptions.Contains);
            Assert.ThrowsException<InvalidOperationException>(() => ManagedInstallerClass.InstallHelper(new[] {"-h"}),helpMessage, ExceptionMessageCompareOptions.Contains);
            Assert.ThrowsException<InvalidOperationException>(() => ManagedInstallerClass.InstallHelper(new[] {"-?"}),helpMessage, ExceptionMessageCompareOptions.Contains);
        }

        private static void InstallComponent()
        {
            ManagedInstallerClass.InstallHelper(new[] {"-AssemblyName", Assembly.GetExecutingAssembly().GetName().Name});
        }


        private static void UnInstallComponent()
        {
            ManagedInstallerClass.InstallHelper(new[] {"-u", "-AssemblyName", Assembly.GetExecutingAssembly().GetName().Name});
        }
    }
}