//using CTecUtil;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Text.Json.Serialization;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;

//namespace Xfp.DataTypes.PanelData
//{
//    public enum EquationTypes
//    {
//        Area,
//        Group,
//        Device
//    }

//    public enum EquationParseResult
//    {
//        Ok,
//        InvalidItem,
//        InvalidToken,
//        ParseError,
//        RangeError,
//        TooManyItems,
//    }


//    public class EquationData : ZonePanelConfigData
//    {
//    public static string EquationParseResultToString(EquationParseResult result, int numItems = 0)
//        => result switch
//        {
//            EquationParseResult.InvalidItem  => "Error_Equation_Invalid_Item",
//            EquationParseResult.InvalidToken => "Error_Equation_Invalid",
//            EquationParseResult.ParseError   => "Error_Equation_Parse_Error",
//            EquationParseResult.RangeError   => "Error_Equation_Value_Out_Of_Range",
//            EquationParseResult.TooManyItems => numItems > 0 ? string.Format("Error_Equation_Too_Many_Items_Max_{0}", numItems) : "Error_Equation_Too_Many_Items",
//            _ => ""
//        };


//        private EquationData() => _validationCodes = new();

//        internal EquationData(EquationData original) : this()
//        {
//            EquationType  = original.EquationType;
//            MaxValidValue = original.MaxValidValue;
//            Equation = new();
//            foreach (var e in original.Equation)
//                Equation.Add(e);
//        }

//        internal EquationData(EquationTypes equationType, int maxValidValue) : this()
//        {
//            EquationType  = equationType;
//            MaxValidValue = maxValidValue;
//            Equation = new();
//            for (int i = 0; i < NumEquationItems; i++)
//                Equation.Add(MaxValidValue + 1);
//        }


//        /// <summary>Number of items in the equation</summary>
//        internal int NumEquationItems
//        {
//            get => EquationType switch
//                                {
//                                    EquationTypes.Area => NumAreaEquationItems,
//                                    EquationTypes.Group => NumGroupEquationItems,
//                                    EquationTypes.Device => NumDeviceEquationItems,
//                                    _ => throw new Exception("Equation type is unknown")
//                                };
//        }

//        internal const int NumAreaEquationItems = 16;
//        internal const int NumGroupEquationItems = 16;
//        internal const int NumDeviceEquationItems = 16;


//        /// <summary>Upper limit of a valid area equation value</summary>
//        internal const int MaxAreaEquationValue = 25;

//        /// <summary>Upper limit of a valid group equation value</summary>
//        internal const int MaxGroupEquationValue = 31;

//        /// <summary>Upper limit of a valid device equation value</summary>
//        internal const int MaxDeviceEquationValue = 255;


//        public List<int> Equation { get; set; }


//        // --- for some reason "[JsonIgnore]" doesn't work here if ValidationCodes is a public property
//        private List<ValidationCodes> _validationCodes { get; set; }
//        internal List<ValidationCodes> GetValidationCodes() => _validationCodes;


//        ///// <summary>Is this an equation where the items are areas (A-Z)</summary>
//        //[JsonIgnore]
//        //public bool IsCharEquation;


//        public EquationTypes EquationType { get; set; }


//        /// <summary>Maximum allowed value</summary>
//        public int  MaxValidValue { get; set; }


//        /// <summary>
//        /// Represents the equation items as a string, grouped where appropriate.<br/>
//        /// E.g. A,B,C,D,G,M,N,O would be represented as "A-D, G, M-O" or 1,2,3,4,7,13,14,15 would be "1-4, 7, 13-15".
//        /// </summary>
//        public override string ToString()
//        {
//            NormaliseEquation();

//            var eqStr = new StringBuilder();
//            for (int i = 0; i < Equation.Count; i++)
//            {
//                if (Equation[i] > MaxValidValue)
//                    continue;

//                var seqLen = 1;
//                for (int j = i + 1; j < Equation.Count; j++, seqLen++)
//                    if (Equation[j] > MaxValidValue || Equation[j] != Equation[j - 1] + 1)
//                        break;

//                var newElement = equationElementStr(Equation[i]);
//                if (!eqStr.ToString().Contains(newElement))
//                    eqStr.Append((eqStr.Length > 0 ? ", " : "") + newElement);

//                if (seqLen > 2)
//                    eqStr.Append("-" + equationElementStr(Equation[i += seqLen - 1]));
//            }

//            return eqStr.ToString();
//        }


//        public bool Equals(EquationData otherData)
//        {
//            if (otherData.EquationType != EquationType
//             || otherData.MaxValidValue != MaxValidValue)
//                return false;
//            if (otherData.Equation.Count != Equation.Count)
//                return false;
//            for (int i = 0; i < Equation.Count; i++)
//                if (otherData.Equation[i] != Equation[i])
//                    return false;
//            return true;
//        }


//        //[JsonIgnore]
//        //internal List<ValidationCodes> ErrorFlags = new();


//        public override bool Validate()
//        {
//            _validationCodes = new();
            

//            return _validationCodes.Count == 0;
//        }


//        /// <summary>
//        /// Sets the Equation values according to the given list.  Duplicates are removed and the result is sorted.
//        /// </summary>
//        internal void NormaliseEquation()
//        {
//            //equationItems.Sort();
            
//            List<int> _tmpEquation = new();
//            //_tmpEquation.AddRange(Equation);

//            Equation.Sort();

//            //copy non-duplicate valid values into Equation
//            foreach (var e in Equation)
//            {
//                if (e > MaxValidValue)
//                    break;

//                //if (_tmpEquation.Count >= NumEquationItems)
//                //    break;

//                 if (!_tmpEquation.Contains(e))
//                    _tmpEquation.Add(e);
//            }

//            Equation = new(_tmpEquation);

//            //pad the remainder with 'empty' data
//            while (Equation.Count < NumEquationItems)
//                Equation.Add(MaxValidValue + 1);
//        }


//        internal static bool FilterInput(bool isAreaEquation, RoutedEventArgs e, string text) => e.Handled = isAreaEquation ? new Regex(@"^[a-zA-Z\- ,]+$").IsMatch(text) : new Regex(@"^[0-9\- ,]+$").IsMatch(text);


//        //internal static string ParseEquation(string stringValue, bool isAreaEquation) => isAreaEquation ? parseCharEquation(stringValue) : parseIntEquation(stringValue);
//        //private  static string parseCharEquation(string stringValue) => new EquationData(true, 'Z').ParseEquation(stringValue).ToString();
//        //private  static string parseIntEquation(string stringValue)  => new EquationData(false, 256).ParseEquation(stringValue).ToString();


//        internal EquationParseResult ParseEquation(string stringValue)                          => EquationType == EquationTypes.Area ? parseCharEquation(stringValue, null) : parseIntEquation(stringValue, null);
//        internal EquationParseResult ParseEquation(string stringValue, IList<char> validValues) => parseCharEquation(stringValue, validValues);
//        internal EquationParseResult ParseEquation(string stringValue, IList<int> validValues)  => parseIntEquation(stringValue, validValues);


//        /// <summary>
//        /// Checks the stringValue, which will contain numeric equation elements, and if it is valid sets the Equation accordingly, i.e. group or device numbers.<br/>
//        /// E.g. "1-4, 7, 13-15" would parse to 1,2,3,4,7,13,14,15<br/>
//        /// If validValues is not null the equation is verified against it to check for disallowed items.
//        /// </summary>
//        /// <param name="stringValue">String to be parsed and validated.</param>
//        /// <param name="validValues">List of allowed equation values.</param>
//        /// <returns></returns>
//        private EquationParseResult parseIntEquation(string stringValue, IList<int> validValues)
//        {
//            //semantic check of the text
//            List<RangeUtil.RangeValue> rangeList;
//            var result = parseIntList(stringValue, out rangeList);

//            //if the text makes sense check its values are valid
//            if (result == EquationParseResult.Ok && rangeList.Count > 0 && validValues != null && validValues.Count > 0)
//                foreach (var r in rangeList)
//                    for (int i = r.MinValue; i <= r.MaxValue; i++)
//                        if (!validValues.Contains(i))
//                            return EquationParseResult.InvalidItem;

//            return result;
//        }


//        /// <summary>
//        /// Checks the stringValue, which will contain alphabetic equation elements, and if it is valid sets the Equation accordingly, i.e. Area codes.<br/>
//        /// E.g. "A-D, G, M-O" would parse to A,B,C,D,G,M,N,O<br/>
//        /// If validValues is not null the equation is verified against it to check for disallowed items.
//        /// </summary>
//        /// <param name="stringValue">String to be parsed and validated.</param>
//        /// <param name="validValues">List of allowed equation values.</param>
//        /// <returns></returns>
//        private EquationParseResult parseCharEquation(string stringValue, IList<char> validValues)
//        {
//            //semantic check of the text
//            List<RangeUtil.RangeValue> rangeList;
//            var result = parseCharList(stringValue, out rangeList);

//            //if the text makes sense check its values are valid
//            if (result == EquationParseResult.Ok || result == EquationParseResult.TooManyItems)
//                if (rangeList.Count > 0 && validValues != null && validValues.Count > 0)
//                    foreach (var r in rangeList)
//                        for (int i = r.MinValue; i <= r.MaxValue; i++)
//                            if (!validValues.Contains((char)i))
//                                return EquationParseResult.InvalidItem;

//            return result;
//        }


//        internal string GetParseResultMessage(EquationParseResult parseResult) => EquationParseResultToString(parseResult);


//        private struct Range
//        {
//            public int StartValue;
//            public int EndValue;
//        }


//        /// <summary>
//        /// Set the Equation according to the parsed string, which will contain int-based equation elements, i.e. group or device numbers.<br/>
//        /// E.g. "1-4, 7, 13-15" would parse to 1,2,3,4,7,13,14,15
//        /// </summary>
//        /// <param name="stringValue"></param>
//        /// <param name="rangeList">A list of integer ranges included in the equation</param>
//        /// <returns></returns>
//        private EquationParseResult parseIntList(string stringValue, out List<RangeUtil.RangeValue> rangeList)
//        {
//            rangeList = new();

//            if (string.IsNullOrWhiteSpace(stringValue))
//            {
//                for (int i = 0; i < Equation.Count; i++)
//                    Equation[i] = MaxValidValue + 1;
//                return EquationParseResult.Ok;
//            }

//            if (!checkValidTokens(stringValue, '0', '9'))
//                    return EquationParseResult.InvalidToken;

//            Equation = new();

//            foreach (var seg in splitSegments(stringValue))
//            {
//                var range = seg.Split("-", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
//                if (range.Length == 0)
//                    continue;

//                int rangeStart, rangeEnd;

//                if (int.TryParse(range[0], out rangeStart))
//                {
//                    rangeStart--;

//                    if (rangeStart < 0 || rangeStart > MaxValidValue)
//                        return EquationParseResult.RangeError;

//                    if (range.Length > 1)
//                    {
//                        int.TryParse(range[1], out rangeEnd);
//                        rangeEnd--;
//                    }
//                    else
//                    {
//                        rangeEnd = rangeStart;
//                    }

//                    if (rangeEnd < 0 || rangeEnd > MaxValidValue || rangeEnd < rangeStart)
//                        return EquationParseResult.RangeError;

//                    rangeList.Add(new(rangeStart, rangeEnd));

//                    for (int i = rangeStart; i <= rangeEnd; i++)
//                        Equation.Add(i);
//                }
//            }

//            if (Equation.Count == 0 && !string.IsNullOrWhiteSpace(stringValue))
//                return EquationParseResult.ParseError;

//            NormaliseEquation();

//            return Equation.Count > NumEquationItems ? EquationParseResult.TooManyItems : EquationParseResult.Ok;
//        }


        
//        /// <summary>
//        /// Set the Equation according to the parsed string, which will contain character-based equation elements, i.e. area codes.<br/>
//        /// E.g. "A-D, G, M-O" would parse to A,B,C,D,G,M,N,O
//        /// </summary>
//        /// <param name="stringValue"></param>          
//        /// <param name="rangeList">A list of char ranges included in the equation</param>
//        /// <returns></returns>
//        private EquationParseResult parseCharList(string stringValue, out List<RangeUtil.RangeValue> rangeList)
//        {
//            rangeList = new();
//            Equation = new();

//            if (string.IsNullOrWhiteSpace(stringValue))
//            {
//                for (int i = 0; i < Equation.Count; i++)
//                    Equation[i] = MaxValidValue + 1;
//                return EquationParseResult.Ok;
//            }

//            if (!checkValidTokens(stringValue, 'A', 'Z'))
//                return EquationParseResult.InvalidToken;

//            foreach (var segment in splitSegments(stringValue))
//            {
//                var seg = segment;

//                while (seg.Contains("-"))
//                {
//                    //strip any initial "-" and contained "--"
//                    while (seg.StartsWith("-"))
//                        seg = seg.Substring(1);

//                    int idxDashDash;
//                    while ((idxDashDash = seg.IndexOf("--")) > 0)
//                        seg = seg.Remove(idxDashDash, 1);

//                    if (!seg.Contains("-"))
//                        break;

//                    //get start of range
//                    int idxDash = seg.IndexOf('-');
//                    int rangeStart = seg[idxDash-1] - 'A';
//                    int rangeEnd = rangeStart;

//                    //is there an end of range?
//                    if (idxDash < seg.Length - 1)
//                    {
//                        rangeEnd = idxDash < seg.Length - 1 ? seg[idxDash + 1] - 'A' : rangeStart;

//                        //if (rangeStart < 0 || rangeEnd > MaxValidValue || rangeEnd < rangeStart)
//                        //    return EquationParseResult.RangeError;

//                        rangeList.Add(new(rangeStart, rangeEnd));

//                        if (rangeStart <= rangeEnd)
//                            for (int i = rangeStart; i <= rangeEnd; i++)
//                                Equation.Add(i);
//                        else
//                            for (int i = rangeStart; i <= rangeStart; i++)
//                                Equation.Add(i);
//                    }
//                    else
//                    {
//                        Equation.Add(rangeStart);
//                    }

//                    if (rangeStart < 0 || rangeEnd < 0 || rangeStart > MaxValidValue || rangeEnd > MaxValidValue || rangeEnd < rangeStart)
//                        return EquationParseResult.RangeError;

//                    seg = seg.Substring(idxDash + 1);
//                }

//                foreach (var c in seg)
//                    Equation.Add(c - 'A');
//            }
            
//            if (Equation.Count == 0 && !string.IsNullOrWhiteSpace(stringValue))
//                return EquationParseResult.ParseError;

//            NormaliseEquation();

//            return Equation.Count > NumEquationItems ? EquationParseResult.TooManyItems : EquationParseResult.Ok;
//        }


//        private bool checkValidTokens(string stringValue, char lowLimit, char highLimit)
//        {
//            foreach (var _ in from c in stringValue
//                              where (c < lowLimit || c > highLimit) && c != ','  && c != '-' && c != ' '
//                              select new { })
//                return false;

//            return true;
//        }

//        private string[] splitSegments(string stringValue) => stringValue.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

//        private string equationElementStr(int element) => EquationType == EquationTypes.Area ? ((char)('A' + element)).ToString() : (1 + element).ToString();
//    }
//}
