using SpotifyLibraryManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyLibraryManager.ViewModels
{
    public class ToolBarViewModel : ViewModelBase
    {
        public bool IsSortingAscending { get; set; }
        public SortBy SortBy { get; set; }
        public List<Tag> Filters { get; set; }
        public string SearchPhrase { get; set; }

        private readonly LibraryManager _libraryManager;
        public ToolBarViewModel(LibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
        }
    }
}
