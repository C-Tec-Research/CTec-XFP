using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Xfp.DataTypes;
using Xfp.DataTypes.PanelData;

namespace Xfp.UI.Validation
{
    internal class PrintPaelRangeValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is string eqnStr)
            {
                //if (!PagingConfigData.CapCodeItem.ValidateRadioPagerCapCode(code))
                //    return new ValidationResult(false, string.Format(Cultures.Resources.Error_Equation_Problem_x, result.ToString()));

                var result = new EquationData(EquationTypes.Area, null, EquationData.MaxAreaEquationValue).ParseEquation(eqnStr);
                if (result != EquationParseResult.Ok)
                {
                    return new ValidationResult(false, string.Format(Cultures.Resources.Error_Equation_Problem_x, result.ToString()));
                }
            }

            return new ValidationResult(true, null);
        }
    }
}
