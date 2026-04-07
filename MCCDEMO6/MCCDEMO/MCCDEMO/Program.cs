using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace MCCDEMO;

internal static class Program
{
	[STAThread]
	private static void Main()
	{
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(false);
		Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
		Application.Run((Form)(object)new MainFrm());
	}
}
