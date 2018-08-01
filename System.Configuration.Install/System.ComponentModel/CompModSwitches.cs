using System.Diagnostics;

namespace System.ComponentModel
{
	internal static class CompModSwitches
	{
		private static TraceSwitch _installerDesign;

		public static TraceSwitch InstallerDesign => _installerDesign ?? (_installerDesign = new TraceSwitch("InstallerDesign",
			                                             "Enable tracing for design-time code for installers"));
	}
}
