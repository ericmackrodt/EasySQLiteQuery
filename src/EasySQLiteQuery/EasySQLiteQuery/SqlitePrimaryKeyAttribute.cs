﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySQLiteQuery
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SqlitePrimaryKeyAttribute : Attribute
    {
        public bool IsAutoIncrement { get; set; }
    }
}
