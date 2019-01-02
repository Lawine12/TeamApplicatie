using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsApplicatie
{
    public static class DataViewExtensions
    {
        public static List<string> ToStringList(this DataView view)
        {
            var list = new List<string>();
            foreach (DataRowView item in view)
            {
                list.Add(item.Row[0].ToString());
            }
            return list;
        }
    }
}
