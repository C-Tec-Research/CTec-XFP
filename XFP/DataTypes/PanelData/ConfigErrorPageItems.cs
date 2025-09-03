using System.Collections.Generic;
using System.Linq;

namespace Xfp.DataTypes.PanelData
{
    public class ConfigErrorPageItems
    {
        internal ConfigErrorPageItems(int index, string name)
        {
            Index = index;
            Name = name;
            ValidationCodes = new();
        }


        internal ConfigErrorPageItems(string name, int index, ValidationCodes code)
            : this(index, name)
        {
            ValidationCodes.Add(code);
        }


        internal ConfigErrorPageItems(string name, List<ValidationCodes> codes)
            : this(0, name)
        {
            ValidationCodes.AddRange(from c in codes select c);
        }


        internal int Index { get; set; }
        internal string Name { get; private set; }



        internal List<ValidationCodes> ValidationCodes { get; set; }

        internal ValidationCodes ValidationCode
        {
            get => ValidationCodes.Count > 0 ? ValidationCodes[0] : DataTypes.ValidationCodes.Ok; 
            set { ValidationCodes ??= new(); ValidationCodes.Add(value); }
        }


        internal bool Contains(ValidationCodes code) => ValidationCodes.Contains(code);
    }
}
