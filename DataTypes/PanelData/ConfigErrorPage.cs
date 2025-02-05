using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xfp.DataTypes.PanelData
{
    public class ConfigErrorPage
    {
        internal ConfigErrorPage(string name)
        {
            Name  = name;
            Items = new();
        }


        internal string Name { get; private set; }


        internal List<ConfigErrorPageItems> Items { get; set; }


        internal bool Contains(string name)
        {
            foreach (var _ in from i in Items where i.Name == name select new { })
                return true;
            return false;
        }
    }
}
