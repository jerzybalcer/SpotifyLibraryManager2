using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.ViewModels;
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

            var suggestion = viewModel?.GetSuggestionTag();

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

        private void NewTagButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NewTagNamePopup.IsOpen = !NewTagNamePopup.IsOpen;

            if (NewTagNamePopup.IsOpen)
            {
                NewTag.Focus();
            }
            else
            {
                Keyboard.ClearFocus();
            }
        }

        private void NewTag_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            NewTagNamePopup.IsOpen = false;
            NewTag.Text = "";
        }

        private void Page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!NewTagButton.IsMouseOver)
            {
                Keyboard.ClearFocus();
            }
        }

        private void NewTag_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(NewSuggestionTextBorder.Visibility == Visibility.Visible)
                {
                    NewSuggestionTextBorder.InputBindings[0].Command.Execute(NewSuggestionText.Text);
                    Keyboard.ClearFocus();
                }
            }
            else if (e.Key == Key.Escape)
            {
                Keyboard.ClearFocus();
            }
            else if (e.Key == Key.Tab)
            {
                if(SuggestionTextBorder.Visibility == Visibility.Visible)
                {
                    SuggestionTextBorder.InputBindings[0].Command.Execute(SuggestionText.Text);
                    Keyboard.ClearFocus();
                }
            }
        }

        private void TagList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ItemsControl tagsList = (ItemsControl)sender;

            if(tagsList.Items.Count == 0)
            {
                NoTagsText.Visibility = Visibility.Visible;
            }
            else
            {
                NoTagsText.Visibility = Visibility.Collapsed;
            }
        }

        private void Title_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Artists.Text.Length == 0)
            {
                NoAlbumSelected.Visibility = Visibility.Visible;
            }
            else
            {
                NoAlbumSelected.Visibility = Visibility.Collapsed;
            }
        }
    }
}
