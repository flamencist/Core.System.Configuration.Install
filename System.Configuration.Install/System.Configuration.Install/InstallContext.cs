using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;

namespace System.Configuration.Install
{
	/// <summary>Contains information about the current installation.</summary>
	public class InstallContext
	{
		/// <summary>Gets the command-line parameters that were entered when InstallUtil.exe was run.</summary>
		/// <returns>A <see cref="T:System.Collections.Specialized.StringDictionary" /> that represents the command-line parameters that were entered when the installation executable was run.</returns>
		public StringDictionary Parameters { get; }

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.InstallContext" /> class.</summary>
		public InstallContext()
			: this(null, null)
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.InstallContext" /> class, and creates a log file for the installation.</summary>
		/// <param name="logFilePath">The path to the log file for this installation, or null if no log file should be created. </param>
		/// <param name="commandLine">The command-line parameters entered when running the installation program, or null if none were entered. </param>
		public InstallContext(string logFilePath, string[] commandLine)
		{
			Parameters = ParseCommandLine(commandLine);
			if (Parameters["logfile"] == null && logFilePath != null)
			{
				Parameters["logfile"] = logFilePath;
			}
		}

		/// <summary>Determines whether the specified command-line parameter is true.</summary>
		/// <returns>true if the specified parameter is set to "yes", "true", "1", or an empty string (""); otherwise, false.</returns>
		/// <param name="paramName">The name of the command-line parameter to check. </param>
		public bool IsParameterTrue(string paramName)
		{
			var text = Parameters[paramName.ToLower(CultureInfo.InvariantCulture)];
			if (text == null)
			{
				return false;
			}
			if (string.Compare(text, "true", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(text, "yes", StringComparison.OrdinalIgnoreCase) != 0 && string.Compare(text, "1", StringComparison.OrdinalIgnoreCase) != 0)
			{
				return "".Equals(text);
			}
			return true;
		}

		internal void LogMessageHelper(string message)
		{
			StreamWriter streamWriter = null;
			try
			{
				if (!string.IsNullOrEmpty(Parameters["logfile"]))
				{
					streamWriter = new StreamWriter(Parameters["logfile"], true, Encoding.UTF8);
					streamWriter.WriteLine(message);
				}
			}
			finally
			{
				streamWriter?.Close();
			}
		}

		/// <summary>Writes a message to the console and to the log file for the installation.</summary>
		/// <param name="message">The message to write. </param>
		public void LogMessage(string message)
		{
			try
			{
				LogMessageHelper(message);
			}
			catch (Exception)
			{
				try
				{
					Parameters["logfile"] = Path.Combine(Path.GetTempPath(), Path.GetFileName(Parameters["logfile"]));
					LogMessageHelper(message);
				}
				catch (Exception)
				{
					Parameters["logfile"] = null;
				}
			}
			if (IsParameterTrue("LogToConsole") || Parameters["logtoconsole"] == null)
			{
				Console.WriteLine(message);
			}
		}

		/// <summary>Parses the command-line parameters into a string dictionary.</summary>
		/// <returns>A <see cref="T:System.Collections.Specialized.StringDictionary" /> containing the parsed command-line parameters.</returns>
		/// <param name="args">An array containing the command-line parameters. </param>
		protected static StringDictionary ParseCommandLine(string[] args)
		{
			var stringDictionary = new StringDictionary();
			if (args == null)
			{
				return stringDictionary;
			}
			for (var i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith("-", StringComparison.Ordinal))
				{
					args[i] = args[i].Substring(1);
				}
				var num = args[i].IndexOf('=');
				if (num < 0)
				{
					stringDictionary[args[i].ToLower(CultureInfo.InvariantCulture)] = "";
				}
				else
				{
					stringDictionary[args[i].Substring(0, num).ToLower(CultureInfo.InvariantCulture)] = args[i].Substring(num + 1);
				}
			}
			return stringDictionary;
		}
	}
}
