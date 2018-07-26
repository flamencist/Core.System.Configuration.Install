using System.Diagnostics;

namespace System.ComponentModel
{
	internal static class CompModSwitches
	{
		private static TraceSwitch installerDesign;

		public static TraceSwitch InstallerDesign
		{
			get
			{
				if (installerDesign == null)
				{
					installerDesign = new TraceSwitch("InstallerDesign", "Enable tracing for design-time code for installers");
				}
				return installerDesign;
			}
		}
	}
}
