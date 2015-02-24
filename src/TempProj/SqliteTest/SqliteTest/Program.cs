using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqliteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteConnection.CreateFile("Mydatabase.sqlite");
            using (var connection = new SQLiteConnection("Data Source=Mydatabase.sqlite;Version=3"))
            {
                SQLiteCommand
                connection.Open();

                //var createRunTable = "CREATE TABLE  AutomatedTestsRun (AutomatedTestsRunID integer PRIMARY KEY AUTOINCREMENT, RunID text, RunStartTime integer, RunEndTime integer, Environment text)";

                //var createTestResultTable = "CREATE TABLE  TestResults (TestResultsID integer PRIMARY KEY AUTOINCREMENT, AutomatedTestsRunID integer, RunStartTime integer, RunEndTime integer, Module text, Scenario text, TestCase text, Status text, ScriptName text, FOREIGN KEY(AutomatedTestsRunID) REFERENCES AutomatedTestsRun(AutomatedTestsRunID))";

                //var createTestResultsView = "CREATE VIEW TestResultsView AS SELECT a.RunID, a.Environment, r.RunStartTime, r.RunEndTime, r.Module, r.Scenario, r.TestCase, r.Status, r.ScriptName FROM TestResults r, AutomatedTestsRun a WHERE r.AutomatedTestsRunID = a.AutomatedTestsRunID";

                //var cmd = new SQLiteCommand(createRunTable, connection);
                //cmd.ExecuteNonQuery();
                //cmd = new SQLiteCommand(createTestResultTable, connection);
                //cmd.ExecuteNonQuery();
                //cmd = new SQLiteCommand(createTestResultsView, connection);
                //cmd.ExecuteNonQuery();

                var atr = new AutomatedTestsRun()
                {
                    Environment = "http://localhost/RM.WebApplication",
                    RunEndTime = DateTime.Now.AddMinutes(2),
                    RunStartTime = DateTime.Now,
                    RunID = Guid.NewGuid()
                };

                var query = new SqliteQuery(connection);
                query.CreateTable<AutomatedTestsRun>();
                query.CreateTable<TestResults>();
                query.Insert(atr);
                var result = query.Select<AutomatedTestsRun>();
            }
        }
    }
}
