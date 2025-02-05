//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;
//using Xfp.DataTypes.PanelData;

//namespace Xfp.UI.Validation
//{
//    internal class ZoneGroupAreaValidationRule : ValidationRule
//    {
//        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
//        {
//            if (value is string zoneGroupArea)
//                if (string.IsNullOrWhiteSpace(zoneGroupArea) || zoneGroupArea == "0")
//                    return new ValidationResult(false, Cultures.Resources.Error_Invalid_Zone_Group_Area_Code);
                
//            return new ValidationResult(true, null);
//        }
//    }
//}
