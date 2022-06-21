using SpotifyLibraryManager.Models;
using SpotifyLibraryManager.Pages;
using SpotifyLibraryManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpotifyLibraryManager.UserControls
{
    /// <summary>
    /// Interaction logic for AlbumCoverButton.xaml
    /// </summary>
    public partial class AlbumCoverButton : UserControl
    {
        public AlbumCoverButton()
        {
            InitializeComponent();
        }

        private void AlbumBtn_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ContextMenuPopup.IsOpen = true;
        }

        private void ContextMenuPopup_MouseLeave(object sender, MouseEventArgs e)
        {
            ContextMenuPopup.IsOpen = false;
        }

        private void OpenWithSpotifyBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Album album = (Album)ThisControl.DataContext;
            AlbumsListViewModel vm = (AlbumsListViewModel)AlbumBtn.DataContext;
            ContextMenuPopup.IsOpen = false;
            vm.OpenWithSpotifyCommand.Execute(album);
            e.Handled = true;
        }

        private void CopyLinkBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Album album = (Album)ThisControl.DataContext;
            AlbumsListViewModel vm = (AlbumsListViewModel)AlbumBtn.DataContext;
            ContextMenuPopup.IsOpen = false;
            vm.CopyLinkCommand.Execute(album);
            e.Handled = true;
        }
    }
}
