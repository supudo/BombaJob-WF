using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(SearchResultsViewModel))]
    public class SearchResultsViewModel : Screen
    {
        private TabberViewModel tabm;
        public string SearchQuery { get; set; }

        public SearchResultsViewModel(TabberViewModel _tabm, string sQuery)
        {
            this.tabm = _tabm;
            this.SearchQuery = sQuery;
            this.DisplayName = " | " + Properties.Resources.menu_Search;
        }
    }
}
