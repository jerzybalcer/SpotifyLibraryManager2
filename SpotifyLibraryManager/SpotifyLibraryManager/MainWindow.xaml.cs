using SpotifyLibraryManager.Models;
using SpotifyLibraryManager.Pages;
using SpotifyLibraryManager.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
        private bool _isSortingAscending = true;

        public MainWindow()
        {
            InitializeComponent();
            var albumsList = new AlbumsListViewModel();
            var detailsPanel = new DetailsPanelViewModel();
            var toolBar = new ToolBarViewModel();

            var libraryManager = new LibraryManager(albumsList, detailsPanel, toolBar);
            albumsList.LibraryManager = libraryManager;
            detailsPanel.LibraryManager = libraryManager;
            toolBar.LibraryManager = libraryManager;

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

        private async void SyncBtn_Click(object sender, RoutedEventArgs e)
        {
            //Albums.ItemsSource = await AlbumsManager.SyncAlbums();
        }

        private void SortDirectionChanger_Click(object sender, RoutedEventArgs e)
        {
            _isSortingAscending = !_isSortingAscending;
            //SortDirection.Text = _isSortingAscending ? "⋀" : "⋁";
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
