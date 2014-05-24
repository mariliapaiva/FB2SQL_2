using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Firebird2Sql.Properties;
using FirebirdSql.Data.FirebirdClient;

namespace Firebird2Sql
{
    public class RecuperaTabelas
    {
        private readonly FrmFirebirdToSql _frmFirebirdToSql;

        public RecuperaTabelas(FrmFirebirdToSql frmFirebirdToSql)
        {
            _frmFirebirdToSql = frmFirebirdToSql;
        }

        public List<string> TabelasFb()
        {

            var connectStringBuilder = _frmFirebirdToSql.FbConnectionStringBuilder();
            var listaTabelas = new List<string>();
            var qtdTabelasFb = 0;
            try
            {
                using (var conexao = new FbConnection(connectStringBuilder.ConnectionString))
                {
                    conexao.Open();
                    using (var fbQuery = new FbCommand())
                    {
                        fbQuery.Connection = conexao;
                        fbQuery.CommandText = "SELECT RDB$RELATION_NAME FROM RDB$RELATIONS WHERE RDB$SYSTEM_FLAG = 0 AND RDB$VIEW_BLR IS NULL ORDER BY RDB$RELATION_NAME";
                        FbDataReader retornoQuery = fbQuery.ExecuteReader();
                        while (retornoQuery.Read())
                        {
                            listaTabelas.Add(retornoQuery.GetString(0).Trim());
                            qtdTabelasFb++;
                        }
                    }

                }
            }
            catch
            {
                MessageBox.Show(string.Format("FB: " + Resources.FrmFirebirdToSql_RecuperaTabelasFB_Conexão_inválida_));
            }
            _frmFirebirdToSql.lblQtdTabelasFB.Text = string.Format("FIREBIRD: " + qtdTabelasFb);
            return listaTabelas;
        }

        public List<string> TabelasSqlServer()
        {

            var connectionStringBuilder = _frmFirebirdToSql.SqlConnectionStringBuilder();
            var listaTabelas = new List<string>();
            var qtdTabelasSql = 0;

            try
            {
                using (var conexao = new SqlConnection(connectionStringBuilder.ConnectionString))
                {
                    conexao.Open();
                    using (var sqlQuery = new SqlCommand())
                    {
                        sqlQuery.Connection = conexao;
                        sqlQuery.CommandText = "SELECT TABLE_NAME FROM information_schema.tables";
                        SqlDataReader retornoQuery = sqlQuery.ExecuteReader();
                        while (retornoQuery.Read())
                        {
                            listaTabelas.Add(retornoQuery.GetString(0));
                            qtdTabelasSql++;

                        }
                    }

                }
            }
            catch
            {
                MessageBox.Show(string.Format("SqlServer: " + Resources.FrmFirebirdToSql_RecuperaTabelasFB_Conexão_inválida_));
            }
            _frmFirebirdToSql.lblQtdTabelasSql.Text = string.Format("MSSQL: " + qtdTabelasSql);
            return listaTabelas;
        }



      
    }
}