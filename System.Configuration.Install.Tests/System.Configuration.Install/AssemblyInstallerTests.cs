using System.IO;
using System.Reflection;
using Xunit;


namespace System.Configuration.Install.Tests.System.Configuration.Install
{
    [Collection("Installer")]
    public class AssemblyInstallerTests
    {
        [Fact]
        public void Install_Uninstall_Should_Read_State_From_File()
        {
            var executingAssembly = Assembly.GetExecutingAssembly().Location;
            var assemblyInstaller = new AssemblyInstaller
            {
                Context = new InstallContext("/var/log/log.log",new []{"-LogToConsole=true","-TestFile="+Guid.NewGuid().ToString().Substring(0,6)}),
                Assembly = Assembly.GetExecutingAssembly()
            };
            var stateFilePath = assemblyInstaller.GetInstallStatePath(executingAssembly);
            
            assemblyInstaller.Install(null);
            Assert.True(File.Exists(stateFilePath));
            Assert.False( string.IsNullOrWhiteSpace(File.ReadAllText(stateFilePath)));
            
            assemblyInstaller.Uninstall(null);
            Assert.False(File.Exists(stateFilePath));
        }
    }
}