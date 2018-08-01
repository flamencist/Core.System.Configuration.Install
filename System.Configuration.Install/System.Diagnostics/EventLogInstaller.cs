using Microsoft.Win32;
using System.ComponentModel;
using System.Configuration.Install;
using System.Runtime.InteropServices;

namespace System.Diagnostics
{
	/// <summary>Allows you to install and configure an event log that your application reads from or writes to when running. </summary>
	public class EventLogInstaller : ComponentInstaller
	{
		private readonly EventSourceCreationData _sourceData = new EventSourceCreationData(null, null);

		private UninstallAction _uninstallAction;

		/// <summary>Gets or sets the path of the resource file that contains category strings for the source.</summary>
		/// <returns>The path of the category resource file. The default is an empty string ("").</returns>
		[ResDescription("Desc_CategoryResourceFile")]
		public string CategoryResourceFile
		{
			get => _sourceData.CategoryResourceFile;
			set => _sourceData.CategoryResourceFile = value;
		}

		/// <summary>Gets or sets the number of categories in the category resource file.</summary>
		/// <returns>The number of categories in the category resource file. The default value is zero.</returns>
		[ResDescription("Desc_CategoryCount")]
		public int CategoryCount
		{
			get => _sourceData.CategoryCount;
			set => _sourceData.CategoryCount = value;
		}

		/// <summary>Gets or sets the name of the log to set the source to.</summary>
		/// <returns>The name of the log. This can be Application, System, or a custom log name. The default is an empty string ("").</returns>
		[ResDescription("Desc_Log")]
		public string Log
		{
			get => _sourceData.LogName;
			set => _sourceData.LogName = value;
		}

		/// <summary>Gets or sets the path of the resource file that contains message formatting strings for the source.</summary>
		/// <returns>The path of the message resource file. The default is an empty string ("").</returns>
		[ResDescription("Desc_MessageResourceFile")]
		public string MessageResourceFile
		{
			get => _sourceData.MessageResourceFile;
			set => _sourceData.MessageResourceFile = value;
		}

		/// <summary>Gets or sets the path of the resource file that contains message parameter strings for the source.</summary>
		/// <returns>The path of the message parameter resource file. The default is an empty string ("").</returns>
		[ResDescription("Desc_ParameterResourceFile")]
		public string ParameterResourceFile
		{
			get => _sourceData.ParameterResourceFile;
			set => _sourceData.ParameterResourceFile = value;
		}

		/// <summary>Gets or sets the source name to register with the log.</summary>
		/// <returns>The name to register with the event log as a source of entries. The default is an empty string ("").</returns>
		[ResDescription("Desc_Source")]
		public string Source
		{
			get => _sourceData.Source;
			set => _sourceData.Source = value;
		}

		/// <summary>Gets or sets a value that indicates whether the Installutil.exe (Installer Tool) should remove the event log or leave it in its installed state at uninstall time.</summary>
		/// <returns>One of the <see cref="T:System.Configuration.Install.UninstallAction" /> values that indicates what state to leave the event log in when the <see cref="T:System.Diagnostics.EventLog" /> is uninstalled. The default is Remove.</returns>
		/// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">
		///   <see cref="P:System.Diagnostics.EventLogInstaller.UninstallAction" /> contains an invalid value. The only valid values for this property are Remove and NoAction.</exception>
		[DefaultValue(UninstallAction.Remove)]
		[ResDescription("Desc_UninstallAction")]
		public UninstallAction UninstallAction
		{
			get => _uninstallAction;
			set
			{
				if (!Enum.IsDefined(typeof(UninstallAction), value))
				{
					throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(UninstallAction));
				}
				_uninstallAction = value;
			}
		}

		/// <summary>Copies the property values of an <see cref="T:System.Diagnostics.EventLog" /> component that are required at installation time for an event log.</summary>
		/// <param name="component">An <see cref="T:System.ComponentModel.IComponent" /> to use as a template for the <see cref="T:System.Diagnostics.EventLogInstaller" />. </param>
		public override void CopyFromComponent(IComponent component)
		{
		}

		/// <summary>Determines whether an installer and another specified installer refer to the same source.</summary>
		/// <returns>true if this installer and the installer specified by the <paramref name="otherInstaller" /> parameter would install or uninstall the same source; otherwise, false.</returns>
		/// <param name="otherInstaller">The installer to compare. </param>
		public override bool IsEquivalentInstaller(ComponentInstaller otherInstaller)
		{
			var eventLogInstaller = otherInstaller as EventLogInstaller;
			if (eventLogInstaller == null)
			{
				return false;
			}
			return eventLogInstaller.Source == Source;
		}
	}
}
