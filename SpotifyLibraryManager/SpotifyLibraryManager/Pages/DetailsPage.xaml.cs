using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using SpotifyLibraryManager.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SpotifyLibraryManager.Pages
{
    /// <summary>
    /// Interaction logic for DetailsPage.xaml
    /// </summary>
    public partial class DetailsPage : Page
    {
        public DetailsPage()
        {
            InitializeComponent();
        }

        private void NewTag_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NewTag.Text.Length == 0)
            {
                SuggestionPopup.IsOpen = false;
                return;
            }

            var viewModel = DataContext as DetailsPanelViewModel;

            var suggestion = viewModel.AllTags.Where(tag => tag.Name.ToLower().StartsWith(NewTag.Text.ToLower())).FirstOrDefault();

            if (suggestion is not null)
            {
                SuggestionTextBorder.Visibility = Visibility.Visible;
                SuggestionText.Text = suggestion.Name;
                SuggestionTextBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(suggestion.ColorHex));
                SuggestionText.Foreground = ContrastCalculator.GetContrastingBrush((Color)ColorConverter.ConvertFromString(suggestion.ColorHex));

                if (suggestion.Name.ToLower() != NewTag.Text.ToLower())
                {
                    NewSuggestionTextBorder.Visibility = Visibility.Visible;
                    NewSuggestionText.Text = NewTag.Text;
                }
                else
                {
                    NewSuggestionTextBorder.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                SuggestionTextBorder.Visibility = Visibility.Collapsed;
                NewSuggestionTextBorder.Visibility = Visibility.Visible;
                NewSuggestionText.Text = NewTag.Text;
            }

            SuggestionPopup.IsOpen = true;
        }
    }
}
