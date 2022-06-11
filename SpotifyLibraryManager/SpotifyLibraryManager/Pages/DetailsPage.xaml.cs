using Microsoft.EntityFrameworkCore;
using SpotifyLibraryManager.Database;
using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using SpotifyLibraryManager.UserControls;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SpotifyLibraryManager.Pages
{
    /// <summary>
    /// Interaction logic for DetailsPage.xaml
    /// </summary>
    public partial class DetailsPage : Page
    {
        private Album _album;
        private List<Tag> _allTags;

        public DetailsPage(Album album)
        {
            InitializeComponent();
            DataContext = this;
            _album = album;
            LoadAllTags();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Title.Text = _album.Title;

            Artists.Text = _album.Artists[0].Name;

            for (var i = 1; i < _album.Artists.Count; i++)
            {
                Artists.Text += ", " + _album.Artists[i].Name;
            }

            TagList.Children.Clear();

            foreach (var tag in _album.Tags)
            {
                TagList.Children.Add(new TagBadge { TagName = tag.Name, TagColor = tag.ColorHex });
            }
        }

        private void NewTag_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (NewTag.Text.Length == 0)
            {
                SuggestionPopup.IsOpen = false;
                return;
            }

            var suggestion = _allTags.Where(tag => tag.Name.ToLower().StartsWith(NewTag.Text.ToLower())).FirstOrDefault();

            if (suggestion is not null)
            {
                SuggestionTextBorder.Visibility = Visibility.Visible;
                SuggestionText.Text = suggestion.Name;
                SuggestionTextBorder.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(suggestion.ColorHex));
                SuggestionText.Foreground = ContrastCalculator.GetContrastingBrush((Color)ColorConverter.ConvertFromString(suggestion.ColorHex));

                if (suggestion.Name.ToLower() != NewTag.Text.ToLower())
                {
                    NewSuggestionTextBorder.Visibility = Visibility.Visible;
                    NewSuggestionText.Text = "New: " + NewTag.Text;
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
                NewSuggestionText.Text = "New: " + NewTag.Text;
            }

            SuggestionPopup.IsOpen = true;
        }

        private async void AddTag(string tagName)
        {
            using (var db = new LibraryContext())
            {
                var thisAlbum = db.Albums
                    .Include(album => album.Tags)
                    .Include(album => album.Artists)
                    .First(album => album.AlbumId == _album.AlbumId);

                var tagExists = _allTags.Exists(tag => tag.Name.ToLower() == tagName.ToLower());

                if (!tagExists)
                {
                    thisAlbum.Tags.Add(new Tag { Name = tagName, ColorHex = "#5050ff" });
                }
                else
                {
                    if (thisAlbum.Tags.FirstOrDefault(tag => tag.Name.ToLower() == tagName.ToLower()) is null)
                    {
                        thisAlbum.Tags.Add(_allTags.First(tag => tag.Name.ToLower() == tagName.ToLower()));
                    }
                }
                await db.SaveChangesAsync();
                _album = thisAlbum;
                _allTags = await db.Tags.ToListAsync();
            }

            NavigationService.Refresh();
        }

        private async void LoadAllTags()
        {
            using (var db = new LibraryContext())
            {
                _allTags = await db.Tags.ToListAsync();
            }
        }

        private void NewSuggestionTextBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SuggestionPopup.IsOpen = false;
            AddTag(NewTag.Text);
        }

        private void SuggestionTextBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SuggestionPopup.IsOpen = false;
            AddTag(SuggestionText.Text);
        }
    }
}
