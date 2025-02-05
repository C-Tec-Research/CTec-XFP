//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;
//using Xfp.DataTypes.PanelData;

//namespace Xfp.UI.Validation
//{
//    internal class AreaValidationRule : ValidationRule
//    {
//        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
//        {
//            if (value is string area)
//                if (string.IsNullOrWhiteSpace(area))
//                    return new ValidationResult(false, Cultures.Resources.Error_Invalid_Group_Num);
                
//            return new ValidationResult(true, null);
//        }
//    }
//}
