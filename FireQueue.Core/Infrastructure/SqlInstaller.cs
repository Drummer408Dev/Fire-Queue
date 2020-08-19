using FireQueue.Core.Database;
using System.IO;
using System.Reflection;

namespace FireQueue.Core.Infrastructure
{
    internal class SqlInstaller
    {
        private SqlClient sqlClient;

        public SqlInstaller(SqlClient sqlClient)
        {
            this.sqlClient = sqlClient;
        }

        public void Install()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "FireQueue.Core.Database.InstallFireQueue.sql";

            string sql;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                sql = reader.ReadToEnd();
            }

            sqlClient.ExecuteSql(sql);
        }
    }
}
