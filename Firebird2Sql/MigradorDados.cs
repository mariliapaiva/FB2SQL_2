using Firebird2Sql.Properties;
using FirebirdSql.Data.FirebirdClient;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Linq;
using System;
using System.Data;
using System.Text;
using System.Data.Common;

namespace Firebird2Sql
{
    public class MigradorDados
    {
        private readonly Conexao conexaoMgr;

        public int QtdTabelasFb { get; set; }
        public int QtdTabelasMsSql { get; set; }

        public MigradorDados(Conexao conexao)
        {
            conexaoMgr = conexao;
        }

        //private void RecuperaFkDasTabelas()
        //{
        //    var recuperaTabelasFk = RecuperaForeignKeyTabelasSql();
        //    //tvFK.Nodes.Clear();
        //    foreach (var tabela in recuperaTabelasFk)
        //    {
        //        if (tabela.DependenciasList.Any())
        //        {
        //            var treeNode = new TreeNode(tabela.Nome);
        //            foreach (var item in tabela.DependenciasList)
        //                treeNode.Nodes.Add(item);
        //            //tvFK.Nodes.Add(treeNode);   
        //        }

        //    }
        //}

        public List<string> TabelasFb()
        {
            var connectStringBuilder = conexaoMgr.ConnectionStringFb;
            var listaTabelas = new List<string>();
            var qtdTabelasFb = 0;
            try
            {
                var comando = @"SELECT RDB$RELATION_NAME 
                                                FROM RDB$RELATIONS 
                                                WHERE RDB$SYSTEM_FLAG = 0 AND 
                                                        RDB$VIEW_BLR IS NULL 
                                                ORDER BY RDB$RELATION_NAME";
                qtdTabelasFb = RecuperaTabelas(new FbConnection(connectStringBuilder), new FbCommand(), listaTabelas, qtdTabelasFb, comando);
            }
            catch
            {
                MessageBox.Show(string.Format("FB: " + Resources.FrmFirebirdToSql_RecuperaTabelasFB_Conexão_inválida_));
            }
            QtdTabelasFb = qtdTabelasFb;
            return listaTabelas;
        }

        public List<string> TabelasSqlServer()
        {
            var connectionString = conexaoMgr.ConnectionStringSql;
            var listaTabelas = new List<string>();
            var qtdTabelasSql = 0;

            try
            {
                var comando = "SELECT TABLE_NAME FROM information_schema.tables";
                qtdTabelasSql = RecuperaTabelas(new SqlConnection(connectionString), new SqlCommand(), listaTabelas, qtdTabelasSql, comando);
            }
            catch
            {
                MessageBox.Show(string.Format("SqlServer: " + Resources.FrmFirebirdToSql_RecuperaTabelasFB_Conexão_inválida_));
            }
            QtdTabelasMsSql = qtdTabelasSql;
            return listaTabelas;
        }

        private static int RecuperaTabelas(DbConnection conexao, DbCommand query, List<string> listaTabelas, int qtdTabelasFb, string comando)
        {
            using (conexao)
            {
                conexao.Open();
                using (query)
                {
                    query.Connection = conexao;
                    query.CommandText = comando;
                    var retornoQuery = query.ExecuteReader();
                    while (retornoQuery.Read())
                    {
                        listaTabelas.Add(retornoQuery.GetString(0).Trim());
                        qtdTabelasFb++;
                    }
                }
            }
            return qtdTabelasFb;
        }


        public List<Tabela> RecuperaForeignKeyTabelasSql(List<string> listaTabelas)
        {
            //var treeNodes = tv.Nodes.Cast<TreeNode>().Where(t => t.Checked);
            var connectionStringBuilder = conexaoMgr.ConnectionStringSql;
            var listaFk = new List<Tabela>();

            using (var conexao = new SqlConnection(connectionStringBuilder))
            {
                conexao.Open();

                foreach (var tabela in listaTabelas)
                {
                    using (var sqlQuery = new SqlCommand())
                    {
                        sqlQuery.Connection = conexao;
                        const string sql = @"SELECT DISTINCT t.name
                                             FROM   sys.foreign_key_columns AS fk  
	                                             INNER JOIN sys.tables AS t ON fk.referenced_object_id = t.object_id  
                                             WHERE  fk.parent_object_id = (SELECT object_id FROM   sys.tables WHERE  name = @nometabela)";
                        sqlQuery.CommandText = sql;
                        sqlQuery.Parameters.AddWithValue("nometabela", tabela);
                        using (SqlDataReader retornoQuery = sqlQuery.ExecuteReader())
                        {
                            var t = new Tabela
                            {
                                Nome = tabela,
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

        public void MigrarDados(IEnumerable<string> listaTabelasMigrar)
        {
            var connectStringBuilderFb = conexaoMgr.ConnectionStringFb;
            var connectStringBuilderSql = conexaoMgr.ConnectionStringSql;

            var listaTabelaOrdenadaPorDependencia = OrdenaTabelasPorDepencencia(listaTabelasMigrar.ToList());

            using (var conexaoFb = new FbConnection(connectStringBuilderFb))
            {
                using (var conexaoSql = new SqlConnection(connectStringBuilderSql))
                {
                    conexaoFb.Open();
                    conexaoSql.Open();

                    var transacaoSql = conexaoSql.BeginTransaction();
                    var successo = true;
                    Exception ex = null;

                    foreach (var tabela in listaTabelaOrdenadaPorDependencia)
                    {
                        using (var queryFb = new FbCommand())
                        {
                            queryFb.Connection = conexaoFb;
                            queryFb.CommandText = string.Format("select * from {0}", tabela);
                            FbDataReader retornoQueryFb = queryFb.ExecuteReader();


                            while (retornoQueryFb.Read())
                            {
                                var parametros = Enumerable.Range(1, retornoQueryFb.FieldCount).Select(i => "@" + i).ToList();
                                var queryInsertsql = new SqlCommand();
                                queryInsertsql.Connection = conexaoSql;
                                queryInsertsql.CommandText = string.Format("insert into {0} values ({1})", tabela, string.Join(",", parametros));

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
                                        using (var sqlQuery = new SqlCommand(string.Format("select * from {0}", tabela), conexaoSql, transacaoSql))
                                        using (var sqlDataReader = sqlQuery.ExecuteReader())
                                            dataTypeNameSql = sqlDataReader.GetDataTypeName(i);

                                        if (dataTypeNameSql == "image")
                                        {
                                            var value = retornoQueryFb.GetValue(i) as byte[];
                                            var sqlParameter = new SqlParameter(parametro, SqlDbType.Image);
                                            if (value != null)
                                                sqlParameter.Value = value;
                                            else
                                                sqlParameter.Value = DBNull.Value;
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

        private List<string> OrdenaTabelasPorDepencencia(List<string> todasTabelasParaMigrar)
        {
            var listaTabelaOrdenadaPorDependencia = new List<string>();
            var listaFk = RecuperaForeignKeyTabelasSql(todasTabelasParaMigrar);
            var existeTabelaNaoAdicionada = true;
            do
            {
                foreach (var tabela in listaFk.OrderBy(t => t.DependenciasList.Count))
                {
                    var tabelaJaAdicionada = listaTabelaOrdenadaPorDependencia.Contains(tabela.Nome);
                    if (tabelaJaAdicionada)
                        continue;

                    var tabelaNaoPossuiDependencia = !tabela.DependenciasList.Any();
                    if (tabelaNaoPossuiDependencia)
                        listaTabelaOrdenadaPorDependencia.Add(tabela.Nome);
                    else
                    {
                        var todasDependenciasForamAdicionadas = tabela.DependenciasList.All(t => todasTabelasParaMigrar.Contains(t));
                        if (!todasDependenciasForamAdicionadas)
                        {
                            var tabelasNaAdicionadas = tabela.DependenciasList.Where(t => !todasTabelasParaMigrar.Contains(t));
                            throw new DependenciasNaoSatisfeitasException(tabela.Nome, tabelasNaAdicionadas);
                        }

                        var todasDependenciasSatisfeitas = tabela.DependenciasList.All(t => listaTabelaOrdenadaPorDependencia.Contains(t));
                        if (todasDependenciasSatisfeitas)
                            listaTabelaOrdenadaPorDependencia.Add(tabela.Nome);
                    }
                }
                existeTabelaNaoAdicionada = listaTabelaOrdenadaPorDependencia.Any(t => !todasTabelasParaMigrar.Contains(t));
            } while (existeTabelaNaoAdicionada);
            return listaTabelaOrdenadaPorDependencia;
        }
    }
}