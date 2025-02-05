using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xfp.DataTypes.PanelData;

namespace Xfp.UI.Interfaces
{
    public interface IConfigData
    {
        ConfigErrorPage GetPageErrorDetails();

        void ClearErrors();

        bool HasErrorsOrWarnings();

        bool HasErrors();

        bool HasWarnings();
    }
}
