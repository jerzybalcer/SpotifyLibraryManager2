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

        private void NewTagButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NewTagNamePopup.IsOpen = !NewTagNamePopup.IsOpen;

            if (NewTagNamePopup.IsOpen)
            {
                NewTag.Focus();
            }
            else
            {
                //Keyboard.ClearFocus();
                Keyboard.Focus(Window.GetWindow(this));
            }
        }

        private void NewTag_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            NewTag.Text = "";
        }

        private void Page_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!NewTagButton.IsMouseOver)
            {
                //Keyboard.ClearFocus();
                Keyboard.Focus(Window.GetWindow(this));
            }
        }

        private void NewTag_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(NewSuggestionTextBorder.Visibility == Visibility.Visible && SuggestionPopup.IsOpen)
                {
                    NewSuggestionTextBorder.InputBindings[0].Command.Execute(NewSuggestionText.Text);
                    NewTag.Text = "";
                }
            }
            else if (e.Key == Key.Escape)
            {
                //Keyboard.ClearFocus();
                Keyboard.Focus(Window.GetWindow(this));
            }
            else if (e.Key == Key.Tab)
            {
                if(SuggestionTextBorder.Visibility == Visibility.Visible && SuggestionPopup.IsOpen)
                {
                    SuggestionTextBorder.InputBindings[0].Command.Execute(SuggestionText.Text);
                    NewTag.Text = "";
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.KeyDown += OnGlobalKeyDown;
        }

        private void OnGlobalKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                if (!SuggestionPopup.IsOpen)
                {
                    if (!NewTagNamePopup.IsOpen && NoAlbumSelected.Visibility != Visibility.Visible)
                    {
                        NewTagNamePopup.IsOpen = true;

                        e.Handled = true;
                    }

                    NewTag.Focus();
                }
                else
                {
                    NewTag_PreviewKeyDown(sender, e);
                }
            }
        }
    }
}
