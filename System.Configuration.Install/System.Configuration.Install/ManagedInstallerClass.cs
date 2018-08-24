using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace System.Configuration.Install
{
	/// <summary>Represents a managed install.</summary>
	public class ManagedInstallerClass
	{
		/// <summary>Handles the functionality of the Installutil.exe (Installer Tool).</summary>
		/// <param name="args">The arguments passed to the Installer Tool.</param>
		public static void InstallHelper(string[] args)
		{
			var doUninstall = false;
			var shouldLoadByName = false;
			var transactedInstaller = new TransactedInstaller();
			var showHelp = false;
			try
			{
				var arrayList = new ArrayList();
				foreach (var arg in args)
				{
					if (arg.StartsWith("-", StringComparison.Ordinal))
					{
						var strA = arg.Substring(1);
						if (string.Compare(strA, "u", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA, "uninstall", StringComparison.OrdinalIgnoreCase) == 0)
						{
							doUninstall = true;
						}
						else if (string.Compare(strA, "?", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA, "help", StringComparison.OrdinalIgnoreCase) == 0)
						{
							showHelp = true;
						}
						else if (string.Compare(strA, "AssemblyName", StringComparison.OrdinalIgnoreCase) == 0)
						{
							shouldLoadByName = true;
						}
						else
						{
							arrayList.Add(arg);
						}
					}
					else
					{
						Assembly assembly;
						try
						{
							assembly = shouldLoadByName ? Assembly.Load(arg) : Assembly.LoadFrom(arg);
						}
						catch (Exception innerException)
						{
							if (arg.IndexOf('=') != -1)
							{
								throw new ArgumentException(Res.GetString("InstallFileDoesntExistCommandLine", arg), innerException);
							}
							throw;
						}
						var value = new AssemblyInstaller(assembly, (string[])arrayList.ToArray(typeof(string)));
						transactedInstaller.Installers.Add(value);
					}
				}
				if (showHelp || transactedInstaller.Installers.Count == 0)
				{
					showHelp = true;
					transactedInstaller.Installers.Add(new AssemblyInstaller());
					throw new InvalidOperationException(GetHelp(transactedInstaller));
				}
				transactedInstaller.Context = new InstallContext("InstallUtil.InstallLog", (string[])arrayList.ToArray(typeof(string)));
			}
			catch (Exception ex)
			{
				if (showHelp)
				{
					throw ex;
				}
				throw new InvalidOperationException(Res.GetString("InstallInitializeException", ex.GetType().FullName, ex.Message));
			}
			var installType = transactedInstaller.Context.Parameters["installtype"];
			if (installType != null && string.Compare(installType, "notransaction", StringComparison.OrdinalIgnoreCase) == 0)
			{
				var action = transactedInstaller.Context.Parameters["action"];
				if (action != null && string.Compare(action, "rollback", StringComparison.OrdinalIgnoreCase) == 0)
				{
					transactedInstaller.Context.LogMessage(Res.GetString("InstallRollbackNtRun"));
					for (var j = 0; j < transactedInstaller.Installers.Count; j++)
					{
						transactedInstaller.Installers[j].Rollback(null);
					}
				}
				else if (action != null && string.Compare(action, "commit", StringComparison.OrdinalIgnoreCase) == 0)
				{
					transactedInstaller.Context.LogMessage(Res.GetString("InstallCommitNtRun"));
					for (var k = 0; k < transactedInstaller.Installers.Count; k++)
					{
						transactedInstaller.Installers[k].Commit(null);
					}
				}
				else if (action != null && string.Compare(action, "uninstall", StringComparison.OrdinalIgnoreCase) == 0)
				{
					transactedInstaller.Context.LogMessage(Res.GetString("InstallUninstallNtRun"));
					for (var l = 0; l < transactedInstaller.Installers.Count; l++)
					{
						transactedInstaller.Installers[l].Uninstall(null);
					}
				}
				else
				{
					transactedInstaller.Context.LogMessage(Res.GetString("InstallInstallNtRun"));
					for (var m = 0; m < transactedInstaller.Installers.Count; m++)
					{
						transactedInstaller.Installers[m].Install(null);
					}
				}
			}
			else if (!doUninstall)
			{
				IDictionary stateSaver = new Hashtable();
				transactedInstaller.Install(stateSaver);
			}
			else
			{
				transactedInstaller.Uninstall(null);
			}
		}

		


		private static string GetHelp(Installer installerWithHelp)
		{
			return Res.GetString("InstallHelpMessageStart") + Environment.NewLine + installerWithHelp.HelpText + Environment.NewLine + Res.GetString("InstallHelpMessageEnd") + Environment.NewLine;
		}

	}
}
