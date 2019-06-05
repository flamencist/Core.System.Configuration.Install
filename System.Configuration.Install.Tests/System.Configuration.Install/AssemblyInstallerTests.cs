using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Configuration.Install.Tests.System.Configuration.Install
{
    [TestClass]
    public class AssemblyInstallerTests
    {
        private readonly string _installStateDir =
            Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString().Substring(0, 6)); 
        
        [TestInitialize]
        public void TestInitialize()
        {
            Directory.CreateDirectory(_installStateDir);
        }
        
        [TestCleanup]
        public void TestCleanup()
        {
            Directory.Delete(_installStateDir, true);
        }
        
        [TestMethod]
        public void Install_Uninstall_Read_State_From_File()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var assemblyInstaller = new AssemblyInstaller(executingAssembly,new string[0])
            {
                Context = new InstallContext("/var/log/log.log",new []{"-LogToConsole=true"})
            };
            var stateFilePath = assemblyInstaller.GetInstallStatePath(executingAssembly.Location);
            
            assemblyInstaller.Install(null);
            Assert.IsTrue(File.Exists(stateFilePath));
            Assert.IsFalse( string.IsNullOrWhiteSpace(File.ReadAllText(stateFilePath)));
            
            assemblyInstaller.Uninstall(null);
            Assert.IsFalse(File.Exists(stateFilePath));
        }
    }
}