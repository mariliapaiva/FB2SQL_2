using Firebird2Sql.Properties;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Firebird2Sql
{
    public partial class FrmFirebirdToSql : Form
    {

        public FrmFirebirdToSql()
        {
            InitializeComponent();
            PreencheParametros();
        }

        private void PreencheParametros()
        {
            textBoxUsu.Text = "SYSDBA";
            textBoxSenha.Text = "masterkey";
            textBoxDatabase.Text = @"D:\Downloads\Marília\CARGAS32_62.GDB";
            textBoxIP.Text = "127.0.0.1";
            textBoxPorta.Text = "3050";
            //sqlserver
            txtDatabaseSql.Text = "cargas32";
            txtServerSql.Text = "(LocalDb)\\v11.0"; //".\\SqlExpress"; //
        }


        private FbConnectionStringBuilder FbConnectionStringBuilder()
        {
            var connectStringBuilder = new FbConnectionStringBuilder();
            connectStringBuilder.UserID = textBoxUsu.Text;
            connectStringBuilder.Password = textBoxSenha.Text;
            connectStringBuilder.Database = textBoxDatabase.Text;
            connectStringBuilder.DataSource = textBoxIP.Text;
            connectStringBuilder.Port = int.Parse(textBoxPorta.Text);
            return connectStringBuilder;
        }

        private List<string> RecuperaTabelasFB()
        {

            var connectStringBuilder = FbConnectionStringBuilder();
            var listaTabelas = new List<string>();
            var qtdTabelasFb = 0;
            try
            {
                using (var conexao = new FbConnection(connectStringBuilder.ConnectionString))
                {
                    conexao.Open();
                    FbDataReader retornoQuery;
                    using (var fbQuery = new FbCommand())
                    {
                        fbQuery.Connection = conexao;
                        fbQuery.CommandText = "SELECT RDB$RELATION_NAME FROM RDB$RELATIONS WHERE RDB$SYSTEM_FLAG = 0 AND RDB$VIEW_BLR IS NULL ORDER BY RDB$RELATION_NAME";
                        retornoQuery = fbQuery.ExecuteReader();
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
                MessageBox.Show("FB: " + Resources.FrmFirebirdToSql_RecuperaTabelasFB_Conexão_inválida_);
            }
            lblQtdTabelasFB.Text = "Qtd de Tabelas Firebird: " + qtdTabelasFb.ToString();
            return listaTabelas;
        }

        private SqlConnectionStringBuilder SqlConnectionStringBuilder()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.IntegratedSecurity = true;
            connectionStringBuilder.DataSource = txtServerSql.Text;
            connectionStringBuilder.InitialCatalog = txtDatabaseSql.Text;
            return connectionStringBuilder;
        }

        private List<string> RecuperaTabelasSqlServer()
        {
            var connectionStringBuilder = SqlConnectionStringBuilder();
            var listaTabelas = new List<string>();
            var qtdTabelasSql = 0;

            try
            {
                using (var conexao = new SqlConnection(connectionStringBuilder.ConnectionString))
                {
                    conexao.Open();
                    SqlDataReader retornoQuery;
                    using (var sqlQuery = new SqlCommand())
                    {
                        sqlQuery.Connection = conexao;
                        sqlQuery.CommandText = "SELECT TABLE_NAME FROM information_schema.tables";
                        retornoQuery = sqlQuery.ExecuteReader();
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
                MessageBox.Show("SqlServer: " + Resources.FrmFirebirdToSql_RecuperaTabelasFB_Conexão_inválida_);
            }
            lblQtdTabelasSql.Text = "Qtd de Tabelas Sql: " + qtdTabelasSql.ToString();
            return listaTabelas;
        }


        private void btnMigrar_Click(object sender, EventArgs e)
        {
            MigrarDados();
        }

        private void MigrarDados()
        {
            var treeNodes = tvTabelasCorrepondentes.Nodes.Cast<TreeNode>().Where(t => t.Checked);
            var connectStringBuilderFB = FbConnectionStringBuilder();
            var connectStringBuilderSql = SqlConnectionStringBuilder();
            using (var conexaoFB = new FbConnection(connectStringBuilderFB.ConnectionString))
            {
                using (var conexaoSql = new SqlConnection(connectStringBuilderSql.ConnectionString))
                {
                    conexaoFB.Open();

                    conexaoSql.Open();

                    var transacaoSql = conexaoSql.BeginTransaction();

                    foreach (var treeNode in treeNodes)
                    {
                        FbDataReader retornoQueryFb;
                        using (var QueryFb = new FbCommand())
                        {
                            QueryFb.Connection = conexaoFB;
                            QueryFb.CommandText = string.Format("select * from {0}", treeNode.Text);
                            retornoQueryFb = QueryFb.ExecuteReader();


                            while (retornoQueryFb.Read())
                            {
                                var parametros = Enumerable.Range(1, retornoQueryFb.FieldCount).Select(i => "@" + i).ToList();
                                var QueryInsertsql = new SqlCommand();
                                QueryInsertsql.Connection = conexaoSql;
                                QueryInsertsql.CommandText = string.Format("insert into {0} values ({1})", treeNode.Text, string.Join(",", parametros));

                                for (int i = 0; i < parametros.Count; i++)
                                {
                                    var parametro = parametros[i];
                                    //var fieldType = retornoFbQuery.GetFieldType(i);
                                    var dataTypeName = retornoQueryFb.GetDataTypeName(i);
                                    if (dataTypeName != "BLOB")
                                    {
                                        QueryInsertsql.Parameters.Add(new SqlParameter(parametro, retornoQueryFb.GetValue(i)));
                                    }
                                    else
                                    {
                                        string dataTypeNameSql;
                                        using (var sqlQuery = new SqlCommand())
                                        {
                                            sqlQuery.Connection = conexaoSql;
                                            sqlQuery.CommandText = string.Format("select * from {0}", treeNode.Text);
                                            using (var sqlDataReader = sqlQuery.ExecuteReader())
                                            {
                                                dataTypeNameSql = sqlDataReader.GetDataTypeName(i);
                                            }
                                        }

                                        if (dataTypeNameSql == "image")
                                        {
                                            var value = retornoQueryFb.GetValue(i) as byte[] ?? null;
                                            var sqlParameter = new SqlParameter(parametro, SqlDbType.Image);
                                            sqlParameter.Value = value;
                                            sqlParameter.IsNullable = true;
                                            QueryInsertsql.Parameters.Add(sqlParameter);
                                        }
                                        else
                                        {
                                            var bytes = retornoQueryFb.GetValue(i) as byte[] ?? new byte[1];
                                            var value = Encoding.Default.GetString(bytes);
                                            QueryInsertsql.Parameters.Add(new SqlParameter(parametro, value));
                                        }
                                    }
                                }
                                try
                                {
                                    QueryInsertsql.Transaction = transacaoSql;
                                    QueryInsertsql.ExecuteNonQuery();
                                }
                                catch (SqlException ex)
                                {

                                    transacaoSql.Rollback();
                                    MessageBox.Show("Erro na Migração dos Dados: "+ex.Message);
                                }
                                
                            }

                            transacaoSql.Commit();
                            MessageBox.Show("Migração realizada com sucesso.");

                        }
                    }
                }
            }
        }


        private void bbtMostrarTabelaFB_Click(object sender, EventArgs e)
        {
            var recuperaTabelasFb = RecuperaTabelasFB();
            var recuperaTabelasSqlServer = RecuperaTabelasSqlServer();

            var tabelasCorrespondentes = recuperaTabelasFb.Intersect(recuperaTabelasSqlServer);
            /*var pares = Enumerable.Range(1, 100).Where(i => i % 2 == 0);*/
            //números pares de uma lista
            /*var treeNodes = new List<TreeNode>();
            foreach (var s in tabelasCorrespondentes)
                treeNodes.Add(new TreeNode(s));*/
            /*var enumerable = from t in tabelasCorrespondentes //LINQ
                             select new TreeNode(t);*/
            //var treeNodes = tabelasCorrespondentes.Select(t => new TreeNode(t)); //com lambda expression
            var treeNodes = tabelasCorrespondentes.Select(ConverteStringToTreeNode);
            tvTabelasCorrepondentes.Nodes.AddRange(treeNodes.ToArray());
        }

        public TreeNode ConverteStringToTreeNode(string s)
        {
            return new TreeNode(s);
        }

        private void bbtMarcarTodos_Click(object sender, EventArgs e)
        {
            var treeNodes = tvTabelasCorrepondentes.Nodes.Cast<TreeNode>();
            //var enumerable = treeNodes as TreeNode[] ?? treeNodes.ToArray();

            if (treeNodes.Count() != 0)
            {
                foreach (var treeNode in treeNodes)
                {
                    if (treeNode.Checked)
                    {
                        treeNode.Checked = false;
                        bbtMarcarTodos.Text = "&Marcar Todos";
                    }
                    else
                    {
                        treeNode.Checked = true;
                        bbtMarcarTodos.Text = "&Desmarcar Todos";
                    }

                }
            }
            else
            {
                MessageBox.Show("Não há tabelas para selecionar ");
            }




        }
    }
}
