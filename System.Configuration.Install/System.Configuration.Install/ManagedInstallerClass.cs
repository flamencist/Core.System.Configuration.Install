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
			var flag = false;
			var flag2 = false;
			var transactedInstaller = new TransactedInstaller();
			var flag3 = false;
			try
			{
				var arrayList = new ArrayList();
				for (var i = 0; i < args.Length; i++)
				{
					if (args[i].StartsWith("/", StringComparison.Ordinal) || args[i].StartsWith("-", StringComparison.Ordinal))
					{
						var strA = args[i].Substring(1);
						if (string.Compare(strA, "u", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA, "uninstall", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag = true;
						}
						else if (string.Compare(strA, "?", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(strA, "help", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag3 = true;
						}
						else if (string.Compare(strA, "AssemblyName", StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag2 = true;
						}
						else
						{
							arrayList.Add(args[i]);
						}
					}
					else
					{
						Assembly assembly = null;
						try
						{
							assembly = ((!flag2) ? Assembly.LoadFrom(args[i]) : Assembly.Load(args[i]));
						}
						catch (Exception innerException)
						{
							if (args[i].IndexOf('=') != -1)
							{
								throw new ArgumentException(Res.GetString("InstallFileDoesntExistCommandLine", args[i]), innerException);
							}
							throw;
						}
						var value = new AssemblyInstaller(assembly, (string[])arrayList.ToArray(typeof(string)));
						transactedInstaller.Installers.Add(value);
					}
				}
				if (flag3 || transactedInstaller.Installers.Count == 0)
				{
					flag3 = true;
					transactedInstaller.Installers.Add(new AssemblyInstaller());
					throw new InvalidOperationException(GetHelp(transactedInstaller));
				}
				transactedInstaller.Context = new InstallContext("InstallUtil.InstallLog", (string[])arrayList.ToArray(typeof(string)));
			}
			catch (Exception ex)
			{
				if (flag3)
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
				else if (!flag)
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
