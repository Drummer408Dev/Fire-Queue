using Dapper;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FireQueue.Core.Database
{
    internal class SqlClient
    {
        private string connectionString;

        public SqlClient(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public int ExecuteSql(string sql, object sqlParams = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Execute(sql, sqlParams);
            }
        }

        public T QuerySingle<T>(string sql, object sqlParams = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.QuerySingle<T>(sql, sqlParams);
            }
        }

        public IEnumerable<T> Query<T>(string sql, object sqlParams = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                return connection.Query<T>(sql, sqlParams);
            }
        }
    }
}
