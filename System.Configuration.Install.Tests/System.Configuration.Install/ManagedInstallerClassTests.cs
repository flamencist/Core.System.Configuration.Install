using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace System.Configuration.Install.Tests.System.Configuration.Install
{
    [Collection("Installer")]
    public class ManagedInstallerClassTests
    {

        [Fact]
        public void Should_Install_UnInstall_Component()
        {
            const string fileName = "test";
            var installStateFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.InstallState";
            var installLogFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.InstallLog";

            InstallComponent();
            Assert.True(File.Exists(fileName));
            Assert.True(File.Exists(installStateFileName));
            
            UnInstallComponent();
            Assert.False(File.Exists(fileName));
            Assert.False(File.Exists(installStateFileName));    
            Assert.True(File.Exists(installLogFileName));
        }

        [Fact]
        public void Should_Rollback_Component()
        {
            var log = new StringBuilder();
            InstallerLogHandler.Instance.OnLog += (source, message) => { log.AppendLine(message); };
            Assert.Throws<InvalidOperationException>(() => ManagedInstallerClass.InstallHelper(new[]
                {"-ThrowException=True", "-AssemblyName",  Assembly.GetExecutingAssembly().GetName().Name}));
            Assert.Contains("The ThrowException parameter is detected.", log.ToString());
        }

        [Theory]
        [InlineData("-help")]
        [InlineData("-h")]
        [InlineData("-?")]
        public void Should_Show_Help(string argument)
        {
            const string helpMessage = "Usage: InstallUtil [-u | -uninstall] [option [...]] assembly [[option [...]] assembly] [...]]";
            
            var ex = Assert.Throws<InvalidOperationException>(() => ManagedInstallerClass.InstallHelper(new[] {argument}));
            Assert.Contains(helpMessage, ex.Message);
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