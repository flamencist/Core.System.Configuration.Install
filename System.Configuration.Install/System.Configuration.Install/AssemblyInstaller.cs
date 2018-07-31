using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace System.Configuration.Install
{
	/// <summary>Loads an assembly, and runs all the installers in it.</summary>
	public class AssemblyInstaller : Installer
	{
		private Assembly assembly;

		private string[] commandLine;

		private bool useNewContext;

		private static bool helpPrinted;

		private bool initialized;

		/// <summary>Gets or sets the assembly to install.</summary>
		/// <returns>An <see cref="T:System.Reflection.Assembly" /> that defines the assembly to install.</returns>
		/// <exception cref="T:System.ArgumentNullException">The property value is null. </exception>
		[ResDescription("Desc_AssemblyInstaller_Assembly")]
		public Assembly Assembly
		{
			get
			{
				return assembly;
			}
			set
			{
				assembly = value;
			}
		}

		/// <summary>Gets or sets the command line to use when creating a new <see cref="T:System.Configuration.Install.InstallContext" /> object for the assembly's installation.</summary>
		/// <returns>An array of type <see cref="T:System.String" /> that represents the command line to use when creating a new <see cref="T:System.Configuration.Install.InstallContext" /> object for the assembly's installation.</returns>
		[ResDescription("Desc_AssemblyInstaller_CommandLine")]
		public string[] CommandLine
		{
			get
			{
				return commandLine;
			}
			set
			{
				commandLine = value;
			}
		}

		/// <summary>Gets the help text for all the installers in the installer collection.</summary>
		/// <returns>The help text for all the installers in the installer collection, including the description of what each installer does and the command-line options (for the installation program) that can be passed to and understood by each installer.</returns>
		public override string HelpText
		{
			get
			{
				if (Path != null && Path.Length > 0)
				{
					base.Context = new InstallContext(null, new string[0]);
					if (!initialized)
					{
						InitializeFromAssembly();
					}
				}
				if (helpPrinted)
				{
					return base.HelpText;
				}
				helpPrinted = true;
				return Res.GetString("InstallAssemblyHelp") + "\r\n" + base.HelpText;
			}
		}

		/// <summary>Gets or sets the path of the assembly to install.</summary>
		/// <returns>The path of the assembly to install.</returns>
		[ResDescription("Desc_AssemblyInstaller_Path")]
		public string Path
		{
			get
			{
				if (assembly == (Assembly)null)
				{
					return null;
				}
				return assembly.Location;
			}
			set
			{
				if (value == null)
				{
					assembly = null;
				}
				assembly = Assembly.LoadFrom(value);
			}
		}

		/// <summary>Gets or sets a value indicating whether to create a new <see cref="T:System.Configuration.Install.InstallContext" /> object for the assembly's installation.</summary>
		/// <returns>true if a new <see cref="T:System.Configuration.Install.InstallContext" /> object should be created for the assembly's installation; otherwise, false. The default is true.</returns>
		[ResDescription("Desc_AssemblyInstaller_UseNewContext")]
		public bool UseNewContext
		{
			get
			{
				return useNewContext;
			}
			set
			{
				useNewContext = value;
			}
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.AssemblyInstaller" /> class.</summary>
		public AssemblyInstaller()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.AssemblyInstaller" /> class, and specifies both the file name of the assembly to install and the command line to use when creating a new <see cref="T:System.Configuration.Install.InstallContext" /> object for the assembly's installation.</summary>
		/// <param name="fileName">The file name of the assembly to install. </param>
		/// <param name="commandLine">The command line to use when creating a new <see cref="T:System.Configuration.Install.InstallContext" /> object for the assembly's installation. Can be a null value.</param>
		public AssemblyInstaller(string fileName, string[] commandLine)
		{
			Path = System.IO.Path.GetFullPath(fileName);
			this.commandLine = commandLine;
			useNewContext = true;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.AssemblyInstaller" /> class, and specifies both the assembly to install and the command line to use when creating a new <see cref="T:System.Configuration.Install.InstallContext" /> object.</summary>
		/// <param name="assembly">The <see cref="T:System.Reflection.Assembly" /> to install. </param>
		/// <param name="commandLine">The command line to use when creating a new <see cref="T:System.Configuration.Install.InstallContext" /> object for the assembly's installation. Can be a null value.</param>
		public AssemblyInstaller(Assembly assembly, string[] commandLine)
		{
			Assembly = assembly;
			this.commandLine = commandLine;
			useNewContext = true;
		}

		/// <summary>Checks to see if the specified assembly can be installed.</summary>
		/// <param name="assemblyName">The assembly in which to search for installers. </param>
		/// <exception cref="T:System.Exception">The specified assembly cannot be installed. </exception>
		public static void CheckIfInstallable(string assemblyName)
		{
			AssemblyInstaller assemblyInstaller = new AssemblyInstaller();
			assemblyInstaller.UseNewContext = false;
			assemblyInstaller.Path = assemblyName;
			assemblyInstaller.CommandLine = new string[0];
			assemblyInstaller.Context = new InstallContext(null, new string[0]);
			assemblyInstaller.InitializeFromAssembly();
			if (assemblyInstaller.Installers.Count == 0)
			{
				throw new InvalidOperationException(Res.GetString("InstallNoPublicInstallers", assemblyName));
			}
		}

		private InstallContext CreateAssemblyContext()
		{
			InstallContext installContext = new InstallContext(System.IO.Path.ChangeExtension(Path, ".InstallLog"), CommandLine);
			if (base.Context != null)
			{
				installContext.Parameters["logtoconsole"] = base.Context.Parameters["logtoconsole"];
			}
			installContext.Parameters["assemblypath"] = Path;
			return installContext;
		}

		private void InitializeFromAssembly()
		{
			Type[] array = null;
			try
			{
				array = GetInstallerTypes(assembly);
			}
			catch (Exception ex)
			{
				base.Context.LogMessage(Res.GetString("InstallException", Path));
				Installer.LogException(ex, base.Context);
				base.Context.LogMessage(Res.GetString("InstallAbort", Path));
				throw new InvalidOperationException(Res.GetString("InstallNoInstallerTypes", Path), ex);
			}
			if (array == null || array.Length == 0)
			{
				base.Context.LogMessage(Res.GetString("InstallNoPublicInstallers", Path));
			}
			else
			{
				for (int i = 0; i < array.Length; i++)
				{
					try
					{
						Installer value = (Installer)Activator.CreateInstance(array[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, new object[0], null);
						base.Installers.Add(value);
					}
					catch (Exception ex2)
					{
						base.Context.LogMessage(Res.GetString("InstallCannotCreateInstance", array[i].FullName));
						Installer.LogException(ex2, base.Context);
						throw new InvalidOperationException(Res.GetString("InstallCannotCreateInstance", array[i].FullName), ex2);
					}
				}
				initialized = true;
			}
		}

		private string GetInstallStatePath(string assemblyPath)
		{
			string text = base.Context.Parameters["InstallStateDir"];
			assemblyPath = System.IO.Path.ChangeExtension(assemblyPath, ".InstallState");
			if (string.IsNullOrEmpty(text))
			{
				return assemblyPath;
			}
			return System.IO.Path.Combine(text, System.IO.Path.GetFileName(assemblyPath));
		}

		/// <summary>Completes the installation transaction.</summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary" /> that contains the state of the computer after all the installers in the installer collection have run. </param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="savedState" /> parameter is null.-or- The saved-state <see cref="T:System.Collections.IDictionary" /> might have been corrupted.-or- A file could not be found. </exception>
		/// <exception cref="T:System.Exception">An error occurred in the <see cref="E:System.Configuration.Install.Installer.Committing" /> event handler of one of the installers in the collection.-or- An error occurred in the <see cref="E:System.Configuration.Install.Installer.Committed" /> event handler of one of the installers in the collection.-or- An exception occurred during the <see cref="M:System.Configuration.Install.AssemblyInstaller.Commit(System.Collections.IDictionary)" /> phase of the installation. The exception is ignored and the installation continues. However, the application might not function correctly after installation completes.-or- Installer types were not found in one of the assemblies.-or- An instance of one of the installer types could not be created. </exception>
		/// <exception cref="T:System.Configuration.Install.InstallException">An exception occurred during the <see cref="M:System.Configuration.Install.AssemblyInstaller.Commit(System.Collections.IDictionary)" /> phase of the installation. The exception is ignored and the installation continues. However, the application might not function correctly after installation completes. </exception>
		public override void Commit(IDictionary savedState)
		{
			PrintStartText(Res.GetString("InstallActivityCommitting"));
			if (!initialized)
			{
				InitializeFromAssembly();
			}
			var installStatePath = GetInstallStatePath(Path);

			try
			{
				if (File.Exists(installStatePath))
				{
					var serialized = File.ReadAllText(installStatePath);
					savedState = JsonConvert.DeserializeObject<IDictionary>(serialized);
				}
			}
			finally
			{
				if (base.Installers.Count == 0)
				{
					base.Context.LogMessage(Res.GetString("RemovingInstallState"));
					File.Delete(installStatePath);
				}
			}
			base.Commit(savedState);
		}

		private static Type[] GetInstallerTypes(Assembly assem)
		{
			ArrayList arrayList = new ArrayList();
			Module[] modules = assem.GetModules();
			for (int i = 0; i < modules.Length; i++)
			{
				Type[] types = modules[i].GetTypes();
				for (int j = 0; j < types.Length; j++)
				{
					if (typeof(Installer).IsAssignableFrom(types[j]) && !types[j].IsAbstract && types[j].IsPublic && ((RunInstallerAttribute)TypeDescriptor.GetAttributes(types[j])[typeof(RunInstallerAttribute)]).RunInstaller)
					{
						arrayList.Add(types[j]);
					}
				}
			}
			return (Type[])arrayList.ToArray(typeof(Type));
		}

		/// <summary>Performs the installation.</summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary" /> used to save information needed to perform a commit, rollback, or uninstall operation. </param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="savedState" /> parameter is null.-or- A file could not be found. </exception>
		/// <exception cref="T:System.Exception">An exception occurred in the <see cref="E:System.Configuration.Install.Installer.BeforeInstall" /> event handler of one of the installers in the collection.-or- An exception occurred in the <see cref="E:System.Configuration.Install.Installer.AfterInstall" /> event handler of one of the installers in the collection.-or- Installer types were not found in one of the assemblies.-or- An instance of one of the installer types could not be created. </exception>
		public override void Install(IDictionary savedState)
		{
			PrintStartText(Res.GetString("InstallActivityInstalling"));
			if (!initialized)
			{
				InitializeFromAssembly();
			}
			Hashtable hashtable = new Hashtable();
			savedState = hashtable;
			try
			{
				base.Install(savedState);
			}
			finally
			{
				
				var serialized = JsonConvert.SerializeObject(savedState);
				using (var writer = File.CreateText(GetInstallStatePath(Path)))
				{
					writer.Write(serialized);
				}
			}
		}

		private void PrintStartText(string activity)
		{
			if (UseNewContext)
			{
				InstallContext installContext = CreateAssemblyContext();
				if (base.Context != null)
				{
					base.Context.LogMessage(Res.GetString("InstallLogContent", Path));
					base.Context.LogMessage(Res.GetString("InstallFileLocation", installContext.Parameters["logfile"]));
				}
				base.Context = installContext;
			}
			base.Context.LogMessage(string.Format(CultureInfo.InvariantCulture, activity, new object[1]
			{
				Path
			}));
			base.Context.LogMessage(Res.GetString("InstallLogParameters"));
			if (base.Context.Parameters.Count == 0)
			{
				base.Context.LogMessage("   " + Res.GetString("InstallLogNone"));
			}
			IDictionaryEnumerator dictionaryEnumerator = (IDictionaryEnumerator)base.Context.Parameters.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				string text = (string)dictionaryEnumerator.Key;
				string str = (string)dictionaryEnumerator.Value;
				if (text.Equals("password", StringComparison.InvariantCultureIgnoreCase))
				{
					str = "********";
				}
				base.Context.LogMessage("   " + text + " = " + str);
			}
		}

		/// <summary>Restores the computer to the state it was in before the installation.</summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary" /> that contains the pre-installation state of the computer. </param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="savedState" /> parameter is null.-or- The saved-state <see cref="T:System.Collections.IDictionary" /> might have been corrupted.-or- A file could not be found. </exception>
		/// <exception cref="T:System.Exception">An exception occurred in the <see cref="E:System.Configuration.Install.Installer.BeforeRollback" /> event handler of one of the installers in the collection.-or- An exception occurred in the <see cref="E:System.Configuration.Install.Installer.AfterRollback" /> event handler of one of the installers in the collection.-or- An exception occurred during the <see cref="M:System.Configuration.Install.AssemblyInstaller.Rollback(System.Collections.IDictionary)" /> phase of the installation. The exception is ignored and the rollback continues. However, the computer might not be fully reverted to its initial state after the rollback completes.-or- Installer types were not found in one of the assemblies.-or- An instance of one of the installer types could not be created. </exception>
		/// <exception cref="T:System.Configuration.Install.InstallException">An exception occurred during the <see cref="M:System.Configuration.Install.AssemblyInstaller.Rollback(System.Collections.IDictionary)" /> phase of the installation. The exception is ignored and the rollback continues. However, the computer might not be fully reverted to its initial state after the rollback completes. </exception>
		public override void Rollback(IDictionary savedState)
		{
			PrintStartText(Res.GetString("InstallActivityRollingBack"));
			if (!initialized)
			{
				InitializeFromAssembly();
			}
			var installStatePath = GetInstallStatePath(Path);

			if (File.Exists(installStatePath))
			{
				var serialized = File.ReadAllText(installStatePath);
				savedState = JsonConvert.DeserializeObject<IDictionary>(serialized);
			}

			try
			{
				base.Rollback(savedState);
			}
			finally
			{
				File.Delete(installStatePath);
			}
		}

		/// <summary>Removes an installation.</summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary" /> that contains the post-installation state of the computer. </param>
		/// <exception cref="T:System.ArgumentException">The saved-state <see cref="T:System.Collections.IDictionary" /> might have been corrupted.-or- A file could not be found. </exception>
		/// <exception cref="T:System.Exception">An error occurred in the <see cref="E:System.Configuration.Install.Installer.BeforeUninstall" /> event handler of one of the installers in the collection.-or- An error occurred in the <see cref="E:System.Configuration.Install.Installer.AfterUninstall" /> event handler of one of the installers in the collection.-or- An exception occurred while uninstalling. The exception is ignored and the uninstall continues. However, the application might not be fully uninstalled after the uninstall completes.-or- Installer types were not found in one of the assemblies.-or- An instance of one of the installer types could not be created.-or- A file could not be deleted. </exception>
		/// <exception cref="T:System.Configuration.Install.InstallException">An exception occurred while uninstalling. The exception is ignored and the uninstall continues. However, the application might not be fully uninstalled after the uninstall completes. </exception>
		public override void Uninstall(IDictionary savedState)
		{
			PrintStartText(Res.GetString("InstallActivityUninstalling"));
			if (!initialized)
			{
				InitializeFromAssembly();
			}
			string installStatePath = GetInstallStatePath(Path);
			if (installStatePath != null && File.Exists(installStatePath))
			{
				try
				{
					var serialized = File.ReadAllText(installStatePath);
					savedState = JsonConvert.DeserializeObject<IDictionary>(serialized);
				}
				catch
				{
					base.Context.LogMessage(Res.GetString("InstallSavedStateFileCorruptedWarning", Path, installStatePath));
					savedState = null;
				}
			}
			else
			{
				savedState = null;
			}
			base.Uninstall(savedState);
			if (installStatePath != null && installStatePath.Length != 0)
			{
				try
				{
					File.Delete(installStatePath);
				}
				catch
				{
					throw new InvalidOperationException(Res.GetString("InstallUnableDeleteFile", installStatePath));
				}
			}
		}
	}
	
}
