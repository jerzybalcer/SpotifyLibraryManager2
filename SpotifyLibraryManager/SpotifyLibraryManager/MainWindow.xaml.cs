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
        private const int DefaultWidth = 1280;
        private const int DefaultHeight = 720;
        private bool _isMaximized = false;
        private AlbumsListViewModel _albumsListViewModel;

        public MainWindow()
        {
            InitializeComponent();

            var libraryManager = new LibraryManager();

            _albumsListViewModel = new AlbumsListViewModel(libraryManager);
            var detailsPanel = new DetailsPanelViewModel(libraryManager);
            var toolBar = new ToolBarViewModel(libraryManager);

            this.DataContext = libraryManager;
            AlbumsList.DataContext = _albumsListViewModel;
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

        private void ToggleMaximized()
        {
            if (_isMaximized)
            {
                App.Current.MainWindow.Top = SystemParameters.WorkArea.Bottom / 2 - DefaultHeight / 2;
                App.Current.MainWindow.Left = SystemParameters.WorkArea.Right / 2 - DefaultWidth / 2;
                App.Current.MainWindow.Width = DefaultWidth;
                App.Current.MainWindow.Height = DefaultHeight;
                _albumsListViewModel.Columns = 4;
            }
            else
            {
                App.Current.MainWindow.Top = SystemParameters.WorkArea.Top;
                App.Current.MainWindow.Left = SystemParameters.WorkArea.Left;
                App.Current.MainWindow.Width = SystemParameters.WorkArea.Width;
                App.Current.MainWindow.Height = SystemParameters.WorkArea.Height;
                _albumsListViewModel.Columns = 6;
            }
            _isMaximized = !_isMaximized;
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
