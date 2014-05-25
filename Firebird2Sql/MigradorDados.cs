using Firebird2Sql.Properties;
using FirebirdSql.Data.FirebirdClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Linq;
using System;
using System.Data;
using System.Text;

namespace Firebird2Sql
{
    public class MigradorDados
    {
        private readonly Conexao conexaoMgr;

        public MigradorDados(Conexao conexao)
        {
            conexaoMgr = conexao;
        }

        private void RecuperaFkDasTabelas()
        {
            var recuperaTabelasFk = RecuperaForeignKeyTabelasSql();
            //tvFK.Nodes.Clear();
            foreach (var tabela in recuperaTabelasFk)
            {
                if (tabela.DependenciasList.Any())
                {
                    var treeNode = new TreeNode(tabela.Nome);
                    foreach (var item in tabela.DependenciasList)
                        treeNode.Nodes.Add(item);
                    //tvFK.Nodes.Add(treeNode);   
                }

            }
        }

        public List<string> TabelasFb()
        {
            var connectStringBuilder = conexaoMgr.ConnectionStringFb;
            var listaTabelas = new List<string>();
            var qtdTabelasFb = 0;
            try
            {
                using (var conexao = new FbConnection(connectStringBuilder))
                {
                    conexao.Open();
                    using (var fbQuery = new FbCommand())
                    {
                        fbQuery.Connection = conexao;
                        fbQuery.CommandText = @"SELECT RDB$RELATION_NAME 
                                                FROM RDB$RELATIONS 
                                                WHERE RDB$SYSTEM_FLAG = 0 AND 
                                                        RDB$VIEW_BLR IS NULL 
                                                ORDER BY RDB$RELATION_NAME";
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
            return listaTabelas;
        }

        public List<string> TabelasSqlServer()
        {

            var connectionStringBuilder = conexaoMgr.ConnectionStringSql;
            var listaTabelas = new List<string>();
            var qtdTabelasSql = 0;

            try
            {
                using (var conexao = new SqlConnection(connectionStringBuilder))
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
            return listaTabelas;
        }


        public List<Tabela> RecuperaForeignKeyTabelasSql()
        {
            var treeNodes = tv.Nodes.Cast<TreeNode>().Where(t => t.Checked);
            var connectionStringBuilder = conexaoMgr.ConnectionStringSql;
            var listaFk = new List<Tabela>();

            using (var conexao = new SqlConnection(connectionStringBuilder))
            {
                conexao.Open();

                foreach (var treeNode in treeNodes)
                {
                    using (var sqlQuery = new SqlCommand())
                    {
                        sqlQuery.Connection = conexao;
                        const string sql = @" SELECT DISTINCT t.name  AS TableWithForeignKey
                                     FROM   sys.foreign_key_columns AS fk  
                                            INNER JOIN sys.tables AS t ON fk.parent_object_id = t.object_id  
                                            INNER JOIN sys.columns AS c ON fk.parent_object_id = c.object_id AND fk.parent_column_id = c.column_id
                                     WHERE  fk.referenced_object_id = (SELECT object_id FROM   sys.tables WHERE  name = '{0}')                      
                                     ORDER  BY tablewithforeignkey";
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

        public void MigrarDados(TreeView tv)
        {
            var treeNodes = tv.Nodes.Cast<TreeNode>().Where(t => t.Checked);
            var connectStringBuilderFb = conexaoMgr.ConnectionStringFb;
            var connectStringBuilderSql = conexaoMgr.ConnectionStringSql;
            using (var conexaoFb = new FbConnection(connectStringBuilderFb))
            {
                using (var conexaoSql = new SqlConnection(connectStringBuilderSql))
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
    }
}