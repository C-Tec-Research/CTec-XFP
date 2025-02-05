using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CTecControls.ViewModels;
using Xfp.DataTypes;

namespace Xfp.UI.Interfaces
{
    interface IConfigToolsPageViewModel
    {
        /// <summary>Build a queue of download request commands to send to the panel</summary>
        void EnqueuePanelDownloadCommands(bool allPages);

        /// <summary>Build a queue of upload commands for sending data to the panel</summary>
        void EnqueuePanelUploadCommands(bool allPages);

        /// <summary>Indicates whether data has changed relating to this page</summary>
        bool DataEquals(XfpData otherData);

        /// <summary>Returns <see langword="true"/> if the data on this page has errors or warnings</summary>
        bool HasErrorsOrWarnings();

        /// <summary>Returns <see langword="true"/> if the data on this page has errors</summary>
        bool HasErrors();

        /// <summary>Returns <see langword="true"/> if the data on this page has warnings</summary>
        bool HasWarnings();
    }
}
