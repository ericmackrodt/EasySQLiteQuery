using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteTest
{
    [SqliteTable]
    public class AutomatedTestsRun
    {
        [SqlitePrimaryKey(IsAutoIncrement=true)]
        public int AutomatedTestsRunID { get; set; }
        public Guid RunID { get; set; }
        public DateTime RunStartTime { get; set; }
        public DateTime RunEndTime { get; set; }
        public string Environment { get; set; }
    }
}
