using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteTest
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SqliteTableAttribute : Attribute
    {
        public string Name { get; set; }

        public SqliteTableAttribute() { }

        public SqliteTableAttribute(string name)
        {
            Name = name;
        }
    }
}
