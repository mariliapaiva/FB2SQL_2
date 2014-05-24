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
        private readonly RecuperaTabelas _recuperaTabelas;

        public FrmFirebirdToSql()
        {
            InitializeComponent();
            PreencheParametros();
            _recuperaTabelas = new RecuperaTabelas(this);
        }

        public RecuperaTabelas RecuperaTabelas
        {
            get { return _recuperaTabelas; }
        }

        private void PreencheParametros()
        {
            textBoxUsu.Text = "SYSDBA";
            textBoxSenha.Text = "masterkey";
            textBoxDatabase.Text = @"C:\Marilia\Dados\CARGAS32.GDB";
            textBoxIP.Text = "127.0.0.1";
            textBoxPorta.Text = "3050";
            //sqlserver
            txtDatabaseSql.Text = "cargas";
            txtServerSql.Text = "(LocalDb)\\v11.0"; //".\\SqlExpress"; //
        }

        public FbConnectionStringBuilder FbConnectionStringBuilder()
        {
            var connectStringBuilder = new FbConnectionStringBuilder();
            connectStringBuilder.UserID = textBoxUsu.Text;
            connectStringBuilder.Password = textBoxSenha.Text;
            connectStringBuilder.Database = textBoxDatabase.Text;
            connectStringBuilder.DataSource = textBoxIP.Text;
            connectStringBuilder.Port = int.Parse(textBoxPorta.Text);
            return connectStringBuilder;
        }


        public SqlConnectionStringBuilder SqlConnectionStringBuilder()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.IntegratedSecurity = true;
            connectionStringBuilder.DataSource = txtServerSql.Text;
            connectionStringBuilder.InitialCatalog = txtDatabaseSql.Text;
            return connectionStringBuilder;
        }

        private List<Tabela> RecuperaForeignKeyTabelasSql()
        {
            var treeNodes = tvTabelasCorrepondentes.Nodes.Cast<TreeNode>().Where(t => t.Checked);
            var connectionStringBuilder = SqlConnectionStringBuilder();
            var listaFk = new List<Tabela>();

            using (var conexao = new SqlConnection(connectionStringBuilder.ConnectionString))
            {
                conexao.Open();

                foreach (var treeNode in treeNodes)
                {
                    using (var sqlQuery = new SqlCommand())
                    {
                        sqlQuery.Connection = conexao;
                        const string sql = @" SELECT DISTINCT t.name  AS TableWithForeignKey, fk.constraint_column_id AS FK_PartNo
                                     FROM   sys.foreign_key_columns AS fk  
                                            INNER JOIN sys.tables AS t ON fk.parent_object_id = t.object_id  
                                            INNER JOIN sys.columns AS c ON fk.parent_object_id = c.object_id AND fk.parent_column_id = c.column_id
                                     WHERE  fk.referenced_object_id = (SELECT object_id FROM   sys.tables WHERE  name = '{0}')                      
                                     ORDER  BY tablewithforeignkey, FK_PartNo";
                        sqlQuery.CommandText = string.Format(sql, treeNode.Text);
                        using (SqlDataReader retornoQuery = sqlQuery.ExecuteReader())
                        {
                            var t = new Tabela
                                {
                                    Nome = treeNode.Text,
                                    DependenciasList = new List<string>()
                                };
                            while (retornoQuery.Read())
                                t.DependenciasList.Add(retornoQuery.GetString(0));

                            listaFk.Add(t);
                        }
                    }
                }
            }

            return listaFk;

        }

        private void btnMigrar_Click(object sender, EventArgs e)
        {
            var treeNodes = tvTabelasCorrepondentes.Nodes.Cast<TreeNode>().Where(x => x.Checked);
            if (treeNodes.Count() != 0)
            {
                MigrarDados();
            }
            else
            {
                MessageBox.Show(string.Format("Não há tabelas marcadas."));
            }
        }

        private void MigrarDados()
        {
            var treeNodes = tvTabelasCorrepondentes.Nodes.Cast<TreeNode>().Where(t => t.Checked);
            var connectStringBuilderFb = FbConnectionStringBuilder();
            var connectStringBuilderSql = SqlConnectionStringBuilder();
            using (var conexaoFb = new FbConnection(connectStringBuilderFb.ConnectionString))
            {
                using (var conexaoSql = new SqlConnection(connectStringBuilderSql.ConnectionString))
                {
                    conexaoFb.Open();
                    conexaoSql.Open();

                    var transacaoSql = conexaoSql.BeginTransaction();
                    var successo = true;
                    Exception ex = null;

                    foreach (var treeNode in treeNodes)
                    {
                        using (var queryFb = new FbCommand())
                        {
                            queryFb.Connection = conexaoFb;
                            queryFb.CommandText = string.Format("select * from {0}", treeNode.Text);
                            FbDataReader retornoQueryFb = queryFb.ExecuteReader();


                            while (retornoQueryFb.Read())
                            {
                                var parametros = Enumerable.Range(1, retornoQueryFb.FieldCount).Select(i => "@" + i).ToList();
                                var queryInsertsql = new SqlCommand();
                                queryInsertsql.Connection = conexaoSql;
                                queryInsertsql.CommandText = string.Format("insert into {0} values ({1})", treeNode.Text, string.Join(",", parametros));

                                #region Monta parametros Insert
                                for (int i = 0; i < parametros.Count; i++)
                                {
                                    var parametro = parametros[i];
                                    //var fieldType = retornoFbQuery.GetFieldType(i);
                                    var dataTypeName = retornoQueryFb.GetDataTypeName(i);
                                    if (dataTypeName != "BLOB")
                                    {
                                        queryInsertsql.Parameters.Add(new SqlParameter(parametro, retornoQueryFb.GetValue(i)));
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
                                            queryInsertsql.Parameters.Add(sqlParameter);
                                        }
                                        else
                                        {
                                            var bytes = retornoQueryFb.GetValue(i) as byte[] ?? new byte[1];
                                            var value = Encoding.Default.GetString(bytes);
                                            queryInsertsql.Parameters.Add(new SqlParameter(parametro, value));
                                        }
                                    }
                                }

                                #endregion

                                try
                                {
                                    queryInsertsql.Transaction = transacaoSql;
                                    queryInsertsql.ExecuteNonQuery();
                                }
                                catch (Exception exception)
                                {
                                    ex = exception;
                                    successo = false;
                                }

                            }
                        }
                    }

                    if (successo)
                    {
                        transacaoSql.Commit();
                        MessageBox.Show(string.Format("Migração dos Dados realizada com sucesso."));
                    }
                    else
                    {
                        transacaoSql.Rollback();
                        MessageBox.Show(string.Format("Erro na Migração dos Dados: {0}", ex.Message));
                    }


                }
            }
        }

        private void bbtMostrarTabelaFB_Click(object sender, EventArgs e)
        {
            MostrarTabelas();
        }

        private void MostrarTabelas()
        {
            var recuperaTabelasFb = RecuperaTabelas.TabelasFb();
            var recuperaTabelasSqlServer = RecuperaTabelas.TabelasSqlServer();

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
            var enumerable = treeNodes as IList<TreeNode> ?? treeNodes.ToList();
            if (enumerable.Count() != 0)
            {
                foreach (var treeNode in enumerable)
                {
                    if (treeNode.Checked)
                    {
                        treeNode.Checked = false;
                        bbtMarcarTodos.Text = string.Format("&Marcar Todos");
                    }
                    else
                    {
                        treeNode.Checked = true;
                        bbtMarcarTodos.Text = string.Format("&Desmarcar Todos");
                    }

                }
            }
            else
            {
                MessageBox.Show(string.Format("Não há tabelas para marcar."));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RecuperaFkDasTabelas();
        }

        private void RecuperaFkDasTabelas()
        {
            var recuperaTabelasFk = RecuperaForeignKeyTabelasSql();
            tvFK.Nodes.Clear();
            foreach (var tabela in recuperaTabelasFk)
            {
                var treeNode = new TreeNode(tabela.Nome);
                foreach (var item in tabela.DependenciasList)
                    treeNode.Nodes.Add(item);
                tvFK.Nodes.Add(treeNode);
            }
        }
    }
}
