using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteTest
{
    [SqliteTable]
    public class TestResults
    {
        [SqlitePrimaryKey(IsAutoIncrement=true)]
        public int TestResultsID { get; set; }
        [SqliteForeignKey(typeof(AutomatedTestsRun))]
        public int AutomatedTestsRunID { get; set; }
        public DateTime RunStartTime { get; set; }
        public DateTime RunEndTime { get; set; }
        public string Module { get; set; }
        public string Scenario { get; set; }
        public string TestCase { get; set; }
        public string Status { get; set; }
        public string ScriptName { get; set; }
    }
}
