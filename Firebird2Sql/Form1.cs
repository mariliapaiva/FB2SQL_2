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
            textBoxDatabase.Text = "C:\\Marilia\\Dados\\CARGAS32.GDB";
            textBoxIP.Text = "127.0.0.1";
            textBoxPorta.Text = "3050";
            txtDatabaseSql.Text ="cargas";
            txtServerSql.Text ="(LocalDb)\\v11.0";

        }


        private void bbtMostrarTabelaFB_Click_1(object sender, EventArgs e)
        {

            var connectStringBuilder = new FbConnectionStringBuilder();
            connectStringBuilder.UserID = textBoxUsu.Text;
            connectStringBuilder.Password = textBoxSenha.Text;
            connectStringBuilder.Database = textBoxDatabase.Text;
            connectStringBuilder.DataSource = textBoxIP.Text;
            connectStringBuilder.Port = int.Parse(textBoxPorta.Text);

            try
            {
                var conexao = new FbConnection(connectStringBuilder.ConnectionString);
                conexao.Open();
                var fbQuery = new FbCommand();
                fbQuery.Connection = conexao;
                fbQuery.CommandText = "SELECT RDB$RELATION_NAME FROM RDB$RELATIONS WHERE RDB$SYSTEM_FLAG = 0 AND RDB$VIEW_BLR IS NULL ORDER BY RDB$RELATION_NAME";
                var retornoQuery = fbQuery.ExecuteReader();
                while (retornoQuery.Read())
                {
                    treeViewFB.Nodes.Add(retornoQuery.GetString(0));
                }
                retornoQuery.Close();
                retornoQuery.Dispose();
                conexao.Close();
                conexao.Dispose();
            }
            catch
            {
                MessageBox.Show("Conexão inválida.");
            }
        }


        private void bbtMostrarTabelasSql_Click(object sender, EventArgs e)
        {
            var connectionStringBuilder = new SqlConnectionStringBuilder();
            connectionStringBuilder.IntegratedSecurity = true;
            connectionStringBuilder.DataSource = txtServerSql.Text;
            connectionStringBuilder.InitialCatalog = txtDatabaseSql.Text;
            
            try
            {
                
                var conexao = new SqlConnection(connectionStringBuilder.ConnectionString);
                conexao.Open();
                var sqlQuery = new SqlCommand();
                sqlQuery.Connection = conexao;
                sqlQuery.CommandText = "SELECT TABLE_NAME FROM information_schema.tables";
                var retornoQuery = sqlQuery.ExecuteReader();
                while (retornoQuery.Read())
                {
                    treeViewSql.Nodes.Add(retornoQuery.GetString(0));
                }

                retornoQuery.Close();
                retornoQuery.Dispose();
                conexao.Close();
                conexao.Dispose();
            }
            catch
            {
                MessageBox.Show("Conexão inválida.");
            }
 
        }

        private void btnMigrar_Click(object sender, EventArgs e)
        {

        }
    }
}
