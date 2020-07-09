using System.Collections;
using System.ComponentModel;
using System.IO;

namespace System.Configuration.Install.Tests
{
    [RunInstaller(true)]
    public class TestInstaller:Installer
    {
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            File.Create("test").Dispose();
            
            if (Context.IsParameterTrue("ThrowException"))
            {
                throw new Exception("The ThrowException parameter is detected.");
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            File.Delete("test");
            base.Uninstall(savedState);
        }
        
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
            Context.LogMessage("Do Rollback from TestInstaller.");
        }
    }
}