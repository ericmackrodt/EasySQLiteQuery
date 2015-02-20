using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteTest
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SqliteViewAttribute : Attribute
    {
        public string Name { get; set; }

        public SqliteViewAttribute()
        {
        }

        public SqliteViewAttribute(string name)
        {
            Name = name;
        }
    }
}
