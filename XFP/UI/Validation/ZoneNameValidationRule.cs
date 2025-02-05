//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;
//using Xfp.DataTypes.PanelData;

//namespace Xfp.UI.Validation
//{
//    internal class ZoneNameValidationRule : ValidationRule
//    {
//        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
//        {
//            if (value is string name)
//                if (string.IsNullOrWhiteSpace(name))
//                    return new ValidationResult(false, Cultures.Resources.Error_Invalid_Zone_Name);
                
//            return new ValidationResult(true, null);
//        }
//    }
//}
