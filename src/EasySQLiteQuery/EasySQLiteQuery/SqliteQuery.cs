using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasySQLiteQuery
{
    public class SqliteQuery : IDisposable
    {
        private readonly SQLiteConnection _connection;

        public SqliteQuery(SQLiteConnection connection)
        {
            _connection = connection;
        }

        public void OpenConnection()
        {
            _connection.Open();
        }

        public IEnumerable<T> Select<T>()
            where T : new()
        {
            var type = typeof(T);
            var name = GetTableViewName(type);

            var fields = type.GetProperties().Select(o => o.Name);

            var query = string.Format("SELECT {0} FROM {1}", string.Join(", ", fields), name);

            var command = new SQLiteCommand(query, _connection);
            var reader = command.ExecuteReader();

            var results = new List<T>();
            while (reader.Read())
            {
                var item = new T();
                foreach (var prop in fields)
                {
                    var value = reader[prop];
                    var property = type.GetProperty(prop);
                    property.SetValue(item, GetTypedValue(value, property.PropertyType));
                }
                results.Add(item);
            }

            return results;
        }

        private string GetTableViewName(Type type)
        {
            var tableAttr = type.GetCustomAttributes(true).FirstOrDefault(o => o.GetType() == typeof(SqliteTableAttribute)) as SqliteTableAttribute;
            var viewAttr = type.GetCustomAttributes(true).FirstOrDefault(o => o.GetType() == typeof(SqliteViewAttribute)) as SqliteViewAttribute;
            var name = tableAttr != null ? tableAttr.Name : viewAttr != null ? viewAttr.Name : null;
            if (string.IsNullOrWhiteSpace(name))
                name = type.Name;
            return name;
        }

        public T Insert<T>(T obj)
        {
            var type = typeof(T);
            var name = GetTableViewName(type);

            var fields = type.GetProperties()
                .Where(o => !o.GetCustomAttributes(true).Any(x => x is SqlitePrimaryKeyAttribute && (x as SqlitePrimaryKeyAttribute).IsAutoIncrement));

            var fieldNames = fields.Select(o => o.Name);
            var fieldValues = fields.Select(o => ToTypedString(o.GetValue(obj)));

            var query = string.Format("INSERT INTO {0} ({1}) VALUES ({2}); SELECT last_insert_rowid();", name, string.Join(", ", fieldNames), string.Join(", ", fieldValues));

            var command = new SQLiteCommand(query, _connection);
            var value = command.ExecuteScalar();

            var primaryKey = type.GetProperties().FirstOrDefault(o => o.GetCustomAttributes(true).Any(x => x is SqlitePrimaryKeyAttribute));
            if (primaryKey != null)
                primaryKey.SetValue(obj, GetTypedValue(value, primaryKey.PropertyType));

            return obj;
        }

        public void Update<T>(T obj)
        {
            var type = typeof(T);
            var name = GetTableViewName(type);

            var fields = type.GetProperties()
                .Where(o => !o.GetCustomAttributes(true).Any(x => x is SqlitePrimaryKeyAttribute && (x as SqlitePrimaryKeyAttribute).IsAutoIncrement));
            var primaryKey = type.GetProperties()
                .First(o => o.GetCustomAttributes(true).Any(x => x is SqlitePrimaryKeyAttribute && (x as SqlitePrimaryKeyAttribute).IsAutoIncrement));

            var updateData = new List<string>();

            foreach (var field in fields)
            {
                var f = string.Format("{0} = {1}", field.Name, ToTypedString(field.GetValue(obj)));
                updateData.Add(f);
            }

            var query = string.Format("UPDATE TABLE {0} SET {1} WHERE {2} = {3}", name, string.Join(", ", updateData), primaryKey.Name, ToTypedString(primaryKey.GetValue(obj)));

            var command = new SQLiteCommand(query, _connection);
            var value = command.ExecuteNonQuery();
        }

        public void Drop<T>(T obj)
        {
            var type = typeof(T);
            var name = GetTableViewName(type);

            var primaryKey = type.GetProperties()
                .First(o => o.GetCustomAttributes(true).Any(x => x is SqlitePrimaryKeyAttribute && (x as SqlitePrimaryKeyAttribute).IsAutoIncrement));

            var query = string.Format("DELETE FROM {0} WHERE {1} = {2}", name, primaryKey.Name, ToTypedString(primaryKey.GetValue(obj)));

            var command = new SQLiteCommand(query, _connection);
            var value = command.ExecuteNonQuery();
        }

        public void CreateTable<T>()
        {
            var type = typeof(T);
            var name = GetTableViewName(type);

            var fields = type.GetProperties();

            var fieldDefinitions = new List<string>();
            var foreignKeys = new List<string>();
            foreach (var field in fields)
            {
                var fieldName = field.Name;
                var propertyType = GetSQLiteType(field);
                var fieldDefinition = string.Format("{0} {1}", fieldName, propertyType);
                var primaryKey = field.GetCustomAttribute(typeof(SqlitePrimaryKeyAttribute)) as SqlitePrimaryKeyAttribute;
                if (primaryKey != null)
                {
                    fieldDefinition += " PRIMARY KEY";
                    if (primaryKey.IsAutoIncrement)
                        fieldDefinition += " AUTOINCREMENT";
                }

                var foreignKey = field.GetCustomAttribute(typeof(SqliteForeignKeyAttribute)) as SqliteForeignKeyAttribute;
                if (foreignKey != null)
                {
                    var referencedTable = GetTableViewName(foreignKey.ReferencedTable);
                    var referencedField = foreignKey.ReferencedProperty ?? fieldName;
                    var foreignKeyDefinition = string.Format("FOREIGN KEY({0}) REFERENCES {1}({2})", fieldName, referencedTable, referencedField);
                    foreignKeys.Add(foreignKeyDefinition);
                }

                fieldDefinitions.Add(fieldDefinition);
            }
            var query = string.Format("CREATE TABLE {0} ({1}{2}{3})", name, string.Join(", ", fieldDefinitions), foreignKeys.Any() ? ", " : "" , string.Join(", ", foreignKeys));

            var command = new SQLiteCommand(query, _connection);
            command.ExecuteNonQuery();
        }

        private string GetSQLiteType(PropertyInfo field)
        {
            if (field.PropertyType == typeof(int) ||
                field.PropertyType == typeof(long) ||
                field.PropertyType == typeof(DateTime))
                return "integer";

            if (field.PropertyType == typeof(double) ||
                field.PropertyType == typeof(decimal))
                return "real";

            return "text";
        }

        private object GetTypedValue(object value, Type type)
        {
            if (type == typeof(DateTime))
                return ((long)value).ToDateTime();

            if (type == typeof(Guid))
                return Guid.Parse((string)value);

            return Convert.ChangeType(value, type);
        }

        private string ToTypedString(object value)
        {
            if (value is string || value is Guid)
                return string.Format("'{0}'", value);

            if (value is DateTime)
                return ((DateTime)value).ToUnixTime().ToString();

            return value.ToString();
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}
