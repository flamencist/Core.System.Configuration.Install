# System.Configuration.Install
[![Build Status](https://travis-ci.org/flamencist/System.Configuration.Install.svg?branch=master)](https://travis-ci.org/flamencist/System.Configuration.Install)
[![NuGet](https://img.shields.io/nuget/v/System.Configuration.Install.svg)](https://www.nuget.org/packages/System.Configuration.Install/)

Support System.Configuration.Install for dotnet core. Used version from full framework .NET 4.0.


Sample usage 
TestInstaller.cs
```
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
```
ManagedInstallerClass.InstallHelper(new[] {"TestInstaller.dll"});
```

## Installation

``` dotnet add package System.Configuration.Install --version 0.1.0-prerelease ``

### License

This software is distributed under the terms of the MIT License (MIT).

### Authors

Alexander Chermyanin / [LinkedIn](https://www.linkedin.com/in/alexander-chermyanin)



Contributions and bugs reports are welcome.
