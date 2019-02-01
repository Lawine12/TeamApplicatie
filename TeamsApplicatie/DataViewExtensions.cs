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
        public static List<int> ToStringList(this DataView view)
        {
            var list = new List<int>();
            foreach (DataRowView item in view)
            {
                list.Add((int) item.Row[0]);
            }
            return list;
        }
    }
}
