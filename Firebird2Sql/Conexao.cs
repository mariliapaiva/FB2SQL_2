using FirebirdSql.Data.FirebirdClient;
using System.Data.SqlClient;

namespace Firebird2Sql
{
    public class Conexao
    {
        public string ConnectionStringFb { get; set; }
        public string ConnectionStringSql { get; set; }

        public void FbConnectionStringBuilder(string usuario, string senha, string database, string ip, string porta)
        {
            var connectStringBuilder = new FbConnectionStringBuilder();
            connectStringBuilder.UserID = usuario;
            connectStringBuilder.Password = senha;
            connectStringBuilder.Database = database;
            connectStringBuilder.DataSource = ip;
            connectStringBuilder.Port = int.Parse(porta);
            ConnectionStringFb = connectStringBuilder.ConnectionString;
        }
        public void SqlConnectionStringBuilder(string datasource, string database)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.IntegratedSecurity = true;
            connectionStringBuilder.DataSource = datasource;
            connectionStringBuilder.InitialCatalog = database;
            ConnectionStringSql = connectionStringBuilder.ConnectionString;

        }
    }
}
