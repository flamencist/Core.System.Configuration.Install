# Core.System.Configuration.Install
[![Build Status](https://travis-ci.org/flamencist/Core.System.Configuration.Install.svg?branch=master)](https://travis-ci.org/flamencist/Core.System.Configuration.Install)
[![NuGet](https://img.shields.io/nuget/v/Core.System.Configuration.Install.svg)](https://www.nuget.org/packages/Core.System.Configuration.Install/)

Support System.Configuration.Install for dotnet core. Used version from full framework .NET 4.0.

Help support the project:

<a href="https://www.buymeacoffee.com/flamencist" target="_blank"><img src="https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png" alt="Buy Me A Coffee" style="height: auto !important;width: auto !important;" ></a>


Sample usage 
TestInstaller.cs
```cs
using System.Collections;
using System.ComponentModel;
using System.IO;

namespace TestInstaller
{
    [RunInstaller(true)]
    public class TestInstaller:Installer
    {
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            File.Create("test");
        }

        public override void Uninstall(IDictionary savedState)
        {
            File.Delete("test");
            base.Uninstall(savedState);
        }
    }
}
```

InstallUtil:
```cs
   class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine($"InstallUtil - {Assembly.GetExecutingAssembly().GetName().Version}");
            try
            {
                ManagedInstallerClass.InstallHelper(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
            return 0;
        }
    }
```
We can use installUtil as global tool (https://github.com/flamencist/InstallUtil/)


## Installation

``` dotnet add package Core.System.Configuration.Install```

## Api

### InstallerLog.Instance.OnLog (retrieve log)
```c#
var log = new StringBuilder();
InstallerLogHandler.Instance.OnLog += (source, message) => { log.AppendLine(message); };
ManagedInstallerClass.InstallHelper(new[] {"TestInstaller.dll"});

Console.Write(log);
```

### ManagedInstallerClass parameters usage
```c#
ManagedInstallerClass.InstallHelper(new[] {"-LogToConsole=True","TestInstaller.dll"});
```

Syntax looks like for InstallUtil: [-u | -uninstall] [option [...]] assembly [[option [...]] assembly] [...]]

InstallUtil help:
``` 
InstallUtil executes the installers in each given assembly.
If the -u or -uninstall switch is specified, it uninstalls
the assemblies, otherwise it installs them. Unlike other
options, -u applies to all assemblies, regardless of where it
appears on the command line.

Installation is done in a transactioned way: If one of the
assemblies fails to install, the installations of all other
assemblies are rolled back. Uninstall is not transactioned.

Options take the form -switch=[value]. Any option that occurs
before the name of an assembly will apply to that assembly's
installation. Options are cumulative but overridable - options
specified for one assembly will apply to the next as well unless
the option is specified with a new value. The default for all
options is empty or false unless otherwise specified.

Options recognized:

Options for installing any assembly:
-AssemblyName
 The assembly parameter will be interpreted as an assembly name (Name,
 Locale, PublicKeyToken, Version). The default is to interpret the
 assembly parameter as the filename of the assembly on disk.

-LogFile=[filename]
 File to write progress to. If empty, do not write log. Default
 is <assemblyname>.InstallLog

-LogToConsole={true|false}
 If false, suppresses output to the console.

-ShowCallStack
 If an exception occurs at any point during installation, the call
 stack will be printed to the log.

-InstallStateDir=[directoryname]
 Directory in which the .InstallState file will be stored. Default
 is the directory of the assembly.


Individual installers used within an assembly may recognize other
options. To learn about these options, run InstallUtil with the paths
of the assemblies on the command line along with the -? or -help option.
```


### License

This software is distributed under the terms of the MIT License (MIT).

### Authors

Alexander Chermyanin / [LinkedIn](https://www.linkedin.com/in/alexander-chermyanin)



Contributions and bugs reports are welcome.
