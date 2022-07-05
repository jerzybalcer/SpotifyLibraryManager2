using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SpotifyLibraryManager.Pages
{
	public partial class ToolBarPage : Page
	{
		// Lower value -> faster search | Higher value -> more optimal
		private const int SEARCH_INTERVAL_MS = 100;

		private DispatcherTimer searchCheckTimer;
		private bool isTimerOn = false;
		private string lastSearchText;

		public ToolBarPage()
		{
			InitializeComponent();
			SetupSearchTimer();
		}

		private void SearchText_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SearchBtn.Command.Execute(SearchText.Text);
				Keyboard.ClearFocus();
			}
			else if (e.Key == Key.Escape)
			{
				Keyboard.ClearFocus();
			}
		}

		private void SearchText_GotKeyboardFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			SearchText.Dispatcher.BeginInvoke(() => SearchText.SelectAll());
		}

		private void SearchText_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (isTimerOn == false)
			{
				// If search text changed and timer is off, start the timer.
				// Timer checks every <SEARCH_INTERVAL_MS> whether search text has changed compared to last check.
				// If not, it automatically executes search.
				StartSearchTimer(SearchText.Text);
			}
		}

		private void SetupSearchTimer()
		{
			searchCheckTimer = new DispatcherTimer
			{
				Interval = new TimeSpan(0, 0, 0, 0, SEARCH_INTERVAL_MS)
			};
			searchCheckTimer.Tick += SearchTimer_Tick;
		}

		private void StartSearchTimer(string searchText)
		{
			lastSearchText = searchText;
			searchCheckTimer.Start();
			isTimerOn = true;
		}

		private void StopSearchTimer()
		{
			searchCheckTimer.Stop();
			isTimerOn = false;
		}

		private void SearchTimer_Tick(object source, EventArgs e)
		{
			bool isSearchTextChanged = lastSearchText != SearchText.Text;

			if (isSearchTextChanged)
			{
				lastSearchText = SearchText.Text;
			}
			else
			{
				StopSearchTimer();
				SearchBtn.Command.Execute(SearchText.Text);
			}
		}
	}
}