using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySQLiteQuery
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SqliteForeignKeyAttribute : Attribute
    {
        public string ReferencedProperty { get; set; }
        public Type ReferencedTable { get; set; }

        public SqliteForeignKeyAttribute(Type referencedTable)
        {
            ReferencedTable = referencedTable;
        }

        public SqliteForeignKeyAttribute(Type referencedTable, string referencedProperty)
            : this(referencedTable)
        {
            ReferencedProperty = ReferencedProperty;
        }
    }
}
