using SpotifyLibraryManager.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (await Spotify.CheckClient())
            {
                Albums.ItemsSource = await AlbumsManager.GetAlbumsFromDb();
            }
        }

        private async void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            await Spotify.StartAuthentication();

            Albums.ItemsSource = await AlbumsManager.GetAlbumsFromDb();
        }

        private async void SyncBtn_Click(object sender, RoutedEventArgs e)
        {
            Albums.ItemsSource = await AlbumsManager.SyncAlbums();
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
    }
}
