namespace System.Configuration.Install
{
	/// <summary>Represents the method that will handle the <see cref="E:System.Configuration.Install.Installer.BeforeInstall" />, <see cref="E:System.Configuration.Install.Installer.AfterInstall" />, <see cref="E:System.Configuration.Install.Installer.Committing" />, <see cref="E:System.Configuration.Install.Installer.Committed" />, <see cref="E:System.Configuration.Install.Installer.BeforeRollback" />, <see cref="E:System.Configuration.Install.Installer.AfterRollback" />, <see cref="E:System.Configuration.Install.Installer.BeforeUninstall" />, or <see cref="E:System.Configuration.Install.Installer.AfterUninstall" /> event of an <see cref="T:System.Configuration.Install.Installer" />.</summary>
	/// <param name="sender">The source of the event. </param>
	/// <param name="e">An <see cref="T:System.Configuration.Install.InstallEventArgs" /> that contains the event data. </param>
	public delegate void InstallEventHandler(object sender, InstallEventArgs e);
}
