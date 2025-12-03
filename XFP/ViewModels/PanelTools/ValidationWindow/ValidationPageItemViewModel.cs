using System.Windows;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.ViewModels.PanelTools.ValidationWindow
{
    public class ValidationPageItemViewModel : ValidationTreeViewItemViewModel
    {
        public ValidationPageItemViewModel(ConfigErrorPageItems item, ValidationTreeViewItemViewModel parentPage)
            : base(parentPage)
        {
            Name = item.Name;
            ValidationCodes = new();
            foreach (var e in item.ValidationCodes)
                if (!ValidationCodesContains(e))
                    ValidationCodes.Add(new(e));
            RefreshView();
        }


        public void AddValidationCode(ValidationCodeViewModel newCode) { ValidationCodes.Add(newCode); OnPropertyChanged(nameof(TotalErrors)); RefreshView(); }


        public override int TotalErrors { get => ValidationCodes.Count; }

        public override ErrorLevels ErrorLevel => GetHighestErrorLevel(ValidationCodes);


        public bool ValidationCodesContains(ValidationCodes errorLevel)
        {
            if (ValidationCodes != null)
                foreach (var v in ValidationCodes)
                    if (v.ValidationCode == errorLevel)
                        return true;
            return false;
        }
    }
}