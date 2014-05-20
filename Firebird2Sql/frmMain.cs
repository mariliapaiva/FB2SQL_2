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
            textBoxUsu.Text = "SYSDBA";
            textBoxSenha.Text = "masterkey";
            textBoxDatabase.Text = "C:\\Marília\\Dados\\CARGAS32.GDB";
            textBoxIP.Text = "127.0.0.1";
            textBoxPorta.Text = "3050";
            //sqlserver
            txtDatabaseSql.Text = "Cargas32";
            txtServerSql.Text =".\\SqlExpress"; //"(LocalDb)\\v11.0";

        }


        private List<string> RecuperaTabelasFB()
        {

            var connectStringBuilder = new FbConnectionStringBuilder();
            connectStringBuilder.UserID = textBoxUsu.Text;
            connectStringBuilder.Password = textBoxSenha.Text;
            connectStringBuilder.Database = textBoxDatabase.Text;
            connectStringBuilder.DataSource = textBoxIP.Text;
            connectStringBuilder.Port = int.Parse(textBoxPorta.Text);
            var listaTabelas = new List<string>();

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
                    }
                    while (retornoQuery.Read())
                        listaTabelas.Add(retornoQuery.GetString(0));
                }
            }
            catch
            {
                MessageBox.Show("FB: " +Resources.FrmFirebirdToSql_RecuperaTabelasFB_Conexão_inválida_);
            }
            return listaTabelas;
        }


        private List<string> RecuperaTabelasSqlServer()
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.IntegratedSecurity = true;
            connectionStringBuilder.DataSource = txtServerSql.Text;
            connectionStringBuilder.InitialCatalog = txtDatabaseSql.Text;
            var listaTabelas = new List<string>();
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
                    }
                    while (retornoQuery.Read())
                        listaTabelas.Add(retornoQuery.GetString(0));
                }
            }
            catch
            {
                MessageBox.Show("SqlServer: " + Resources.FrmFirebirdToSql_RecuperaTabelasFB_Conexão_inválida_);
            }
            return listaTabelas;
        }

        private void btnMigrar_Click(object sender, EventArgs e)
        {

        }

      
        private void bbtMostrarTabelaFB_Click(object sender, EventArgs e)
        {
            var recuperaTabelasFb = RecuperaTabelasFB();
            var recuperaTabelasSqlServer = RecuperaTabelasSqlServer();

            var tabelasCorrespondentes = recuperaTabelasFb.Intersect(recuperaTabelasSqlServer);
            /*var pares = Enumerable.Range(1, 100).Where(i => i % 2 == 0);*/ //números pares de uma lista
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
    }
}
