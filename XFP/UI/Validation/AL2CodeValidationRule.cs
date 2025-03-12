using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xfp.DataTypes.PanelData;

namespace Xfp.UI.Validation
{
    internal class AL2CodeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is string code)
                if (!PanelConfigData.ValidateAccessCode(code))
                    return new ValidationResult(false, Cultures.Resources.Error_Invalid_AL2_Code);

            return new ValidationResult(true, null);
        }
    }
}
