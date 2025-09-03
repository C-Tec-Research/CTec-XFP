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
