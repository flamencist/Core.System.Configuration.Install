using System.Collections;
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
			try
			{
				var text = transactedInstaller.Context.Parameters["installtype"];
				if (text != null && string.Compare(text, "notransaction", StringComparison.OrdinalIgnoreCase) == 0)
				{
					var text2 = transactedInstaller.Context.Parameters["action"];
					if (text2 != null && string.Compare(text2, "rollback", StringComparison.OrdinalIgnoreCase) == 0)
					{
						transactedInstaller.Context.LogMessage(Res.GetString("InstallRollbackNtRun"));
						for (var j = 0; j < transactedInstaller.Installers.Count; j++)
						{
							transactedInstaller.Installers[j].Rollback(null);
						}
					}
					else if (text2 != null && string.Compare(text2, "commit", StringComparison.OrdinalIgnoreCase) == 0)
					{
						transactedInstaller.Context.LogMessage(Res.GetString("InstallCommitNtRun"));
						for (var k = 0; k < transactedInstaller.Installers.Count; k++)
						{
							transactedInstaller.Installers[k].Commit(null);
						}
					}
					else if (text2 != null && string.Compare(text2, "uninstall", StringComparison.OrdinalIgnoreCase) == 0)
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
			catch (Exception ex2)
			{
				throw ex2;
			}
		}

		private static string GetHelp(Installer installerWithHelp)
		{
			return Res.GetString("InstallHelpMessageStart") + Environment.NewLine + installerWithHelp.HelpText + Environment.NewLine + Res.GetString("InstallHelpMessageEnd") + Environment.NewLine;
		}

		private static string[] StringToArgs(string cmdLine)
		{
			var arrayList = new ArrayList();
			StringBuilder stringBuilder = null;
			var flag = false;
			var flag2 = false;
			foreach (var c in cmdLine)
			{
				if (stringBuilder == null)
				{
					if (char.IsWhiteSpace(c))
					{
						continue;
					}
					stringBuilder = new StringBuilder();
				}
				if (flag)
				{
					if (flag2)
					{
						if (c != '\\' && c != '"')
						{
							stringBuilder.Append('\\');
						}
						flag2 = false;
						stringBuilder.Append(c);
					}
					else
					{
						switch (c)
						{
						case '"':
							flag = false;
							break;
						case '\\':
							flag2 = true;
							break;
						default:
							stringBuilder.Append(c);
							break;
						}
					}
				}
				else if (char.IsWhiteSpace(c))
				{
					arrayList.Add(stringBuilder.ToString());
					stringBuilder = null;
					flag2 = false;
				}
				else if (flag2)
				{
					stringBuilder.Append(c);
					flag2 = false;
				}
				else
				{
					switch (c)
					{
					case '^':
						flag2 = true;
						break;
					case '"':
						flag = true;
						break;
					default:
						stringBuilder.Append(c);
						break;
					}
				}
			}
			if (stringBuilder != null)
			{
				arrayList.Add(stringBuilder.ToString());
			}
			var array = new string[arrayList.Count];
			arrayList.CopyTo(array);
			return array;
		}
	}
}
