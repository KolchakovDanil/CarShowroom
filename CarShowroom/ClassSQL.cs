using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShowroom
{
    class ClassSQL
    {
        public static string GetConnSQL()
        {
            return @"Data Source = DESKTOP-MJM6IQP\SQLEXPRESS;Initial Catalog=Avtosalon;Integrated Security=True";
        }
    }
}
