using SpotifyLibraryManager.Models;
using SpotifyLibraryManager.Pages;
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

        private void AlbumBtn_Click(object sender, RoutedEventArgs e)
        {
            //((MainWindow)App.Current.MainWindow).DetailsPanel.Navigate(new DetailsPage((Album)DataContext));
            
        }
    }
}
