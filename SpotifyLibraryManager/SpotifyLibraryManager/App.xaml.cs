using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace SpotifyLibraryManager
{
	public partial class App : Application
	{
		protected void ApplicationStartup(object sender, StartupEventArgs e)
		{
			Process currentProcess = Process.GetCurrentProcess();
			int appInstancesCount = Process.GetProcesses().Where(p => p.ProcessName == currentProcess.ProcessName).Count();

			if (appInstancesCount > 1)
			{
				MessageBox.Show("Already an instance of Spotify Library Manager is running.");
				App.Current.Shutdown();
			}

			// Launch main window of the application.
			var appMainWindow = new MainWindow();
			Current.MainWindow = appMainWindow;
			appMainWindow.Show();
		}
	}
}