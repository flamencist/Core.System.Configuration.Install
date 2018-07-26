using System.Collections;

namespace System.Configuration.Install
{
	/// <summary>Provides data for the events: <see cref="E:System.Configuration.Install.Installer.BeforeInstall" />, <see cref="E:System.Configuration.Install.Installer.AfterInstall" />, <see cref="E:System.Configuration.Install.Installer.Committing" />, <see cref="E:System.Configuration.Install.Installer.Committed" />, <see cref="E:System.Configuration.Install.Installer.BeforeRollback" />, <see cref="E:System.Configuration.Install.Installer.AfterRollback" />, <see cref="E:System.Configuration.Install.Installer.BeforeUninstall" />, <see cref="E:System.Configuration.Install.Installer.AfterUninstall" />.</summary>
	public class InstallEventArgs : EventArgs
	{
		private IDictionary savedState;

		/// <summary>Gets an <see cref="T:System.Collections.IDictionary" /> that represents the current state of the installation.</summary>
		/// <returns>An <see cref="T:System.Collections.IDictionary" /> that represents the current state of the installation.</returns>
		public IDictionary SavedState => savedState;

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.InstallEventArgs" /> class, and leaves the <see cref="P:System.Configuration.Install.InstallEventArgs.SavedState" /> property empty.</summary>
		public InstallEventArgs()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Configuration.Install.InstallEventArgs" /> class, and specifies the value for the <see cref="P:System.Configuration.Install.InstallEventArgs.SavedState" /> property.</summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary" /> that represents the current state of the installation. </param>
		public InstallEventArgs(IDictionary savedState)
		{
			this.savedState = savedState;
		}
	}
}
