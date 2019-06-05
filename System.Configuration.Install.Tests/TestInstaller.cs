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
            var path = Context.Parameters.ContainsKey("TestFile")? Context.Parameters["TestFile"]: "test";
            
            File.Create(path);
            
            if (Context.IsParameterTrue("ThrowException"))
            {
                throw new Exception("The ThrowException parameter is detected.");
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            var path = Context.Parameters.ContainsKey("TestFile")? Context.Parameters["TestFile"]: "test";
            File.Delete(path);
            base.Uninstall(savedState);
        }
        
        public override void Rollback(IDictionary savedState)
        {
            base.Rollback(savedState);
            Context.LogMessage("Do Rollback from TestInstaller.");
        }
    }
}