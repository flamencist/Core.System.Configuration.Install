using System.Runtime.InteropServices;

namespace System.Configuration.Install
{
	/// <summary>Provides an interface for a managed installer.</summary>
	[ComImport]
	[Guid("1E233FE7-C16D-4512-8C3B-2E9988F08D38")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IManagedInstaller
	{
		/// <summary>Executes a managed installation.</summary>
		/// <returns>The return code for installutil.exe. A successful installation returns 0. Other values indicate failure.</returns>
		/// <param name="commandLine">The command line that specifies the installation.</param>
		/// <param name="hInstall">The handle to the installation.</param>
		[return: MarshalAs(UnmanagedType.I4)]
		int ManagedInstall([In] [MarshalAs(UnmanagedType.BStr)] string commandLine, [In] [MarshalAs(UnmanagedType.I4)] int hInstall);
	}
}
