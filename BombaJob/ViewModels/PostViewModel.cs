﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

using BombaJob.Utilities.Interfaces;

using Caliburn.Micro;

namespace BombaJob.ViewModels
{
    [Export(typeof(PostViewModel))]
    public class PostViewModel : Screen
    {
        private TabberViewModel tabm;

        public PostViewModel(TabberViewModel _tabm)
        {
            this.tabm = _tabm;
            this.DisplayName = Properties.Resources.menu_Post;
        }

        public PostViewModel()
        {
            this.DisplayName = Properties.Resources.menu_Post;
        }
    }
}
