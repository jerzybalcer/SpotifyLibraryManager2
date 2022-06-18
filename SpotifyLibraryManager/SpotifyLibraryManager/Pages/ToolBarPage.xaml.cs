using System.Windows.Controls;
using System.Windows.Input;

namespace SpotifyLibraryManager.Pages
{
    /// <summary>
    /// Interaction logic for ToolBarPage.xaml
    /// </summary>
    public partial class ToolBarPage : Page
    {
        public ToolBarPage()
        {
            InitializeComponent();
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
    }
}
