using System.Windows.Controls;
using Xfp.DataTypes.PanelData;

namespace Xfp.UI.Validation
{
    internal class AL3CodeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is string code)
                if (!PanelConfigData.ValidateAccessCode(code))
                    return new ValidationResult(false, Cultures.Resources.Error_Invalid_AL3_Code);

            return new ValidationResult(true, null);
        }
    }
}
