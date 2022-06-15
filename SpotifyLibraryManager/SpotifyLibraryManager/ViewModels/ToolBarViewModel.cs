using Microsoft.EntityFrameworkCore;
using SpotifyLibraryManager.Database;
using SpotifyLibraryManager.Helpers;
using SpotifyLibraryManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyLibraryManager.ViewModels
{
    public class ToolBarViewModel : ViewModelBase
    {
        public bool IsSortingAscending { get; set; }
        public SortBy SortBy { get; set; }
        public ObservableCollection<Tag> Filters { get; set; }
        public List<Tag> AllTags { get; set; }
        public string SearchPhrase { get; set; }
        public LibraryManager LibraryManager { get; set; }
        public Command SearchCommand { get; set; }

        public void SearchAlbums(object obj)
        {
            LibraryManager?.SearchAlbums(SearchPhrase);
        }

        public void FilterAlbums(object obj)
        {
            LibraryManager?.FilterAlbums();
        }

        public ToolBarViewModel()
        {
            Filters = new ObservableCollection<Tag>();
            Filters.CollectionChanged += (s, e) => FilterAlbums(null);
            SearchCommand = new Command(SearchAlbums);
        }
    }
}
