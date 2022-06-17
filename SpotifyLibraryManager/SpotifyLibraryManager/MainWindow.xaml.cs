using SpotifyLibraryManager.Database;
using SpotifyLibraryManager.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpotifyLibraryManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var libraryManager = new LibraryManager();

            var albumsList = new AlbumsListViewModel(libraryManager);
            var detailsPanel = new DetailsPanelViewModel(libraryManager);
            var toolBar = new ToolBarViewModel(libraryManager);

            this.DataContext = libraryManager;
            AlbumsList.DataContext = albumsList;
            DetailsPanel.DataContext = detailsPanel;
            ToolBar.DataContext = toolBar;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (await Spotify.CheckClient())
            {
            }
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            await Spotify.StartAuthentication();
        }

        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            App.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void FullScreenBtn_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximized();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                ToggleMaximized();
            }

            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private static void ToggleMaximized()
        {
            if (App.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                App.Current.MainWindow.WindowState = WindowState.Normal;
            }
            else if (App.Current.MainWindow.WindowState == WindowState.Normal)
            {
                App.Current.MainWindow.WindowState = WindowState.Maximized;
            }
        }

        private void Frame_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            var frame = sender as Frame;
            var content = frame!.Content as FrameworkElement;

            if (content is not null)
            {
                content.DataContext = frame.DataContext;
            }
        }
    }
}
