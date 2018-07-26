using System.Runtime.InteropServices;

namespace System.Configuration.Install
{
	internal static class NativeMethods
	{
		public const int INSTALLMESSAGE_ERROR = 16777216;

		[DllImport("msi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int MsiCreateRecord(int cParams);

		[DllImport("msi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int MsiRecordSetInteger(int hRecord, int iField, int iValue);

		[DllImport("msi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int MsiRecordSetStringW(int hRecord, int iField, string szValue);

		[DllImport("msi.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int MsiProcessMessage(int hInstall, int messageType, int hRecord);
	}
}
