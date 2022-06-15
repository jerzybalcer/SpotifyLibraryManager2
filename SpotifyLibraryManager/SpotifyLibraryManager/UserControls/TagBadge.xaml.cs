using SpotifyLibraryManager.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SpotifyLibraryManager.UserControls
{
    /// <summary>
    /// Interaction logic for TagBadge.xaml
    /// </summary>
    public partial class TagBadge : UserControl
    {
        public TagBadge()
        {
            InitializeComponent();
        }
        private void ThisControl_Loaded(object sender, RoutedEventArgs e)
        {
            SolidColorBrush contrastingBrush = ContrastCalculator.GetContrastingBrush((TagBadgeBorder.Background as SolidColorBrush)!.Color);
            TagNameTxt.Foreground = contrastingBrush;
            Cross.Foreground = contrastingBrush;
        }
    }
}
