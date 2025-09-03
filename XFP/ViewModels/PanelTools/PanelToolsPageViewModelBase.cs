using System.Windows;
using System.Windows.Xps;
using CTecControls.ViewModels;
using Xfp.DataTypes;

namespace Xfp.ViewModels.PanelTools
{
    public class PanelToolsPageViewModelBase : PageViewModelBase
    {
        public PanelToolsPageViewModelBase(FrameworkElement parent)
        {
            Parent = parent;
        }


        protected XfpData _data;


        public FrameworkElement Parent { get; set; }


        private int _panelNumber = XfpData.MinPanelNumber;
        public int PanelNumber { get => _panelNumber; set { _panelNumber = value; OnPropertyChanged(); OnPropertyChanged(nameof(PanelDesc)); } }
        public string PanelDesc => string.Format(Cultures.Resources.Panel_x_Configuration, PanelNumber);


        public delegate bool ChangesAreAllowedChecker();
        public ChangesAreAllowedChecker CheckChangesAreAllowed;

        public virtual void SetChangesAreAllowedChecker(ChangesAreAllowedChecker checker) { CheckChangesAreAllowed = checker; }


        /// <summary>
        /// Print the page.
        /// </summary>
        /// <returns>True if no errors.</returns>
        public virtual bool PrintPage(XpsDocumentWriter documentWriter) => true;
    }
}
