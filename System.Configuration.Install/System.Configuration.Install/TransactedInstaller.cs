using System.Collections;

namespace System.Configuration.Install
{
	/// <summary>Defines an installer that either succeeds completely or fails and leaves the computer in its initial state.</summary>
	public class TransactedInstaller : Installer
	{
		/// <summary>Performs the installation.</summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary" /> in which this method saves information needed to perform a commit, rollback, or uninstall operation. </param>
		/// <exception cref="T:System.ArgumentException">The <paramref name="savedState" /> parameter is null. </exception>
		/// <exception cref="T:System.Exception">The installation failed, and is being rolled back. </exception>
		public override void Install(IDictionary savedState)
		{
			if (base.Context == null)
			{
				base.Context = new InstallContext();
			}
			base.Context.LogMessage(Environment.NewLine + Res.GetString("InstallInfoTransacted"));
			try
			{
				bool flag = true;
				try
				{
					base.Context.LogMessage(Environment.NewLine + Res.GetString("InstallInfoBeginInstall"));
					base.Install(savedState);
				}
				catch (Exception ex)
				{
					flag = false;
					base.Context.LogMessage(Environment.NewLine + Res.GetString("InstallInfoException"));
					Installer.LogException(ex, base.Context);
					base.Context.LogMessage(Environment.NewLine + Res.GetString("InstallInfoBeginRollback"));
					try
					{
						Rollback(savedState);
					}
					catch (Exception)
					{
					}
					base.Context.LogMessage(Environment.NewLine + Res.GetString("InstallInfoRollbackDone"));
					throw new InvalidOperationException(Res.GetString("InstallRollback"), ex);
				}
				if (flag)
				{
					base.Context.LogMessage(Environment.NewLine + Res.GetString("InstallInfoBeginCommit"));
					try
					{
						Commit(savedState);
					}
					finally
					{
						base.Context.LogMessage(Environment.NewLine + Res.GetString("InstallInfoCommitDone"));
					}
				}
			}
			finally
			{
				base.Context.LogMessage(Environment.NewLine + Res.GetString("InstallInfoTransactedDone"));
			}
		}

		/// <summary>Removes an installation.</summary>
		/// <param name="savedState">An <see cref="T:System.Collections.IDictionary" /> that contains the state of the computer after the installation completed. </param>
		public override void Uninstall(IDictionary savedState)
		{
			if (base.Context == null)
			{
				base.Context = new InstallContext();
			}
			base.Context.LogMessage(Environment.NewLine + Environment.NewLine + Res.GetString("InstallInfoBeginUninstall"));
			try
			{
				base.Uninstall(savedState);
			}
			finally
			{
				base.Context.LogMessage(Environment.NewLine + Res.GetString("InstallInfoUninstallDone"));
			}
		}
	}
}
