﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTecControls.UI.Interfaces;
using Xfp.DataTypes;

namespace Xfp.UI.Interfaces
{
    interface IPanelToolsViewModel : IAppViewModel
    {
        /// <summary>Populate the viewmodel according to the given data</summary>
        void PopulateView(XfpData data);

        /// <summary>Update the view's bindings in case data has changed elsewhere</summary>
        void RefreshView();

        public delegate bool ChangesAreAllowedChecker();
    }
}
