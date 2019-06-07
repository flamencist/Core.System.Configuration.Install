using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace System.Configuration.Install
{
	/// <summary>Loads an assembly, and runs all the installers in it.</summary>
	public class AssemblyInstaller : Installer
	{
		private static bool _helpPrinted;

		private bool _initialized;
		private readonly IStateSerializer _stateSerializer = new XmlStateSerializer();

		/// <summary>Gets or sets the assembly to install.</summary>
		/// <returns>An <see cref="T:System.Reflection.Assembly" /> that defines the assembly to install.</returns>
		/// <exception cref="T:System.ArgumentNullException">The property value is null. </exception>
		[ResDescription("Desc_AssemblyInstaller_Assembly")]
		public Assembly Assembly { get; set; }

		/// <summary>Gets or sets the command line to use when creating a new <see cref="T:System.Configuration.Install.InstallContext" /> object for the assembly's installation.</summary>
		/// <returns>An array of type <see cref="T:System.String" /> that represents the command line to use when creating a new <see cref="T:System.Configuration.Install.InstallContext" /> object for the assembly's installation.</returns>
		[ResDescription("Desc_AssemblyInstaller_CommandLine")]
		public string[] CommandLine { get; set; }

		/// <summary>Gets the help text for all the installers in the installer collection.</summary>
		/// <returns>The help text for all the installers in the installer collection, including the description of what each installer does and the command-line options (for the installation program) that can be passed to and understood by each installer.</returns>
		public override string HelpText
		{
			get
			{
				if (!string.IsNullOrEmpty(Path))
				{
					Context = new InstallContext(null, new string[0]);
					if (!_initialized)
					{
						InitializeFromAssembly();
					}
				}
				if (_helpPrinted)
				{
					return base.HelpText;
				}
				_helpPrinted = true;
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
				if (Assembly == null)
				{
					return null;
				}
				return Assembly.Location;
			}
			set
			{
				if (value == null)
				{
					Assembly = null;
				}
				Assembly = Assembly.LoadFrom(value);
			}
		}

		/// <summary>Gets or sets a value indicating whether to create a new <see cref="T:System.Configuration.Install.InstallContext" /> object for the assembly's installation.</summary>
		/// <returns>true if a new <see cref="T:System.Configuration.Install.InstallContext" /> object should be created for the assembly's installation; otherwise, false. The default is true.</returns>
		[ResDescription("Desc_AssemblyInstaller_UseNewContext")]
		public bool UseNewContext { get; set; }

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
			CommandLine = commandLine;
			UseNewContext = true;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.AssemblyInstaller" /> class, and specifies both the assembly to install and the command line to use when creating a new <see cref="T:System.Configuration.Install.InstallContext" /> object.</summary>
		/// <param name="assembly">The <see cref="T:System.Reflection.Assembly" /> to install. </param>
		/// <param name="commandLine">The command line to use when creating a new <see cref="T:System.Configuration.Install.InstallContext" /> object for the assembly's installation. Can be a null value.</param>
		public AssemblyInstaller(Assembly assembly, string[] commandLine)
		{
			Assembly = assembly;
			CommandLine = commandLine;
			UseNewContext = true;
		}

		/// <summary>Checks to see if the specified assembly can be installed.</summary>
		/// <param name="assemblyName">The assembly in which to search for installers. </param>
		/// <exception cref="T:System.Exception">The specified assembly cannot be installed. </exception>
		public static void CheckIfInstallable(string assemblyName)
		{
			var assemblyInstaller = new AssemblyInstaller();
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
			var installContext = new InstallContext(System.IO.Path.ChangeExtension(Path, ".InstallLog"), CommandLine);
			if (Context != null)
			{
				installContext.Parameters["logtoconsole"] = Context.Parameters["logtoconsole"];
			}
			installContext.Parameters["assemblypath"] = Path;
			return installContext;
		}

		private void InitializeFromAssembly()
		{
			Type[] array = null;
			try
			{
				array = GetInstallerTypes(Assembly);
			}
			catch (Exception ex)
			{
				Context.LogMessage(Res.GetString("InstallException", Path));
				LogException(ex, Context);
				Context.LogMessage(Res.GetString("InstallAbort", Path));
				throw new InvalidOperationException(Res.GetString("InstallNoInstallerTypes", Path), ex);
			}
			if (array == null || array.Length == 0)
			{
				Context.LogMessage(Res.GetString("InstallNoPublicInstallers", Path));
			}
			else
			{
				for (var i = 0; i < array.Length; i++)
				{
					try
					{
						var value = (Installer)Activator.CreateInstance(array[i], BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, null, new object[0], null);
						Installers.Add(value);
					}
					catch (Exception ex2)
					{
						Context.LogMessage(Res.GetString("InstallCannotCreateInstance", array[i].FullName));
						LogException(ex2, Context);
						throw new InvalidOperationException(Res.GetString("InstallCannotCreateInstance", array[i].FullName), ex2);
					}
				}
				_initialized = true;
			}
		}

		internal string GetInstallStatePath(string assemblyPath)
		{
			var text = Context.Parameters["InstallStateDir"];
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
			if (!_initialized)
			{
				InitializeFromAssembly();
			}
			var installStatePath = GetInstallStatePath(Path);

			try
			{
				if (File.Exists(installStatePath))
				{
					var serialized = File.ReadAllText(installStatePath);
					savedState = _stateSerializer.Deserialize<Hashtable>(serialized);
				}
			}
			finally
			{
				if (Installers.Count == 0)
				{
					Context.LogMessage(Res.GetString("RemovingInstallState"));
					File.Delete(installStatePath);
				}
			}
			base.Commit(savedState);
		}

		private static Type[] GetInstallerTypes(Assembly assem)
		{
			var arrayList = new ArrayList();
			var modules = assem.GetModules();
			for (var i = 0; i < modules.Length; i++)
			{
				var types = modules[i].GetTypes();
				for (var j = 0; j < types.Length; j++)
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
			if (!_initialized)
			{
				InitializeFromAssembly();
			}
			var hashtable = new Hashtable();
			savedState = hashtable;
			try
			{
				base.Install(savedState);
			}
			finally
			{
				var serialized = _stateSerializer.Serialize(savedState);
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
				var installContext = CreateAssemblyContext();
				if (Context != null)
				{
					Context.LogMessage(Res.GetString("InstallLogContent", Path));
					Context.LogMessage(Res.GetString("InstallFileLocation", installContext.Parameters["logfile"]));
				}
				Context = installContext;
			}
			Context.LogMessage(string.Format(CultureInfo.InvariantCulture, activity, new object[1]
			{
				Path
			}));
			Context.LogMessage(Res.GetString("InstallLogParameters"));
			if (Context.Parameters.Count == 0)
			{
				Context.LogMessage("   " + Res.GetString("InstallLogNone"));
			}
			var dictionaryEnumerator = (IDictionaryEnumerator)Context.Parameters.GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				var text = (string)dictionaryEnumerator.Key;
				var str = (string)dictionaryEnumerator.Value;
				if (text.Equals("password", StringComparison.InvariantCultureIgnoreCase))
				{
					str = "********";
				}
				Context.LogMessage("   " + text + " = " + str);
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
			if (!_initialized)
			{
				InitializeFromAssembly();
			}
			var installStatePath = GetInstallStatePath(Path);

			if (File.Exists(installStatePath))
			{
				
				var serialized = File.ReadAllText(installStatePath);
				savedState = _stateSerializer.Deserialize<Hashtable>(serialized);
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
			if (!_initialized)
			{
				InitializeFromAssembly();
			}
			var installStatePath = GetInstallStatePath(Path);
			if (installStatePath != null && File.Exists(installStatePath))
			{
				try
				{
					var serialized = File.ReadAllText(installStatePath);
					savedState = _stateSerializer.Deserialize<Hashtable>(serialized);
				}
				catch
				{
					Context.LogMessage(Res.GetString("InstallSavedStateFileCorruptedWarning", Path, installStatePath));
					savedState = null;
				}
			}
			else
			{
				savedState = null;
			}
			base.Uninstall(savedState);
			if (!string.IsNullOrEmpty(installStatePath))
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
