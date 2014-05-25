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
        private  MigradorDados migradorDados;

        public FrmFirebirdToSql()
        {
            InitializeComponent();
            PreencheParametros();

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

  
        private void btnMigrar_Click(object sender, EventArgs e)
        {
            var treeNodes = tvTabelasCorrepondentes.Nodes.Cast<TreeNode>().Where(x => x.Checked);
            if (treeNodes.Count() != 0)
            {
                migradorDados.MigrarDados(tvTabelasCorrepondentes);
            }
            else
            {
                MessageBox.Show(string.Format("Não há tabelas marcadas."));
            }
        }

  
        private void bbtMostrarTabelaFB_Click(object sender, EventArgs e)
        {
            Conexao con = new Conexao();
            con.FbConnectionStringBuilder(textBoxUsu.Text, textBoxSenha.Text, textBoxDatabase.Text,textBoxIP.Text, textBoxPorta.Text);
            con.SqlConnectionStringBuilder(txtServerSql.Text, txtDatabaseSql.Text);
            migradorDados = new MigradorDados(con);
            MostrarTabelas(tvTabelasCorrepondentes);
        }

        private void MostrarTabelas(TreeView tv)
        {
            var recuperaTabelasFb = migradorDados.TabelasFb();
            var recuperaTabelasSqlServer = migradorDados.TabelasSqlServer();

            var tabelasCorrespondentes = recuperaTabelasFb.Intersect(recuperaTabelasSqlServer);
            /*var pares = Enumerable.Range(1, 100).Where(i => i % 2 == 0);*/
            //números pares de uma lista
            /*var treeNodes = new List<TreeNode>();
            foreach (var s in tabelasCorrespondentes)
                treeNodes.Add(new TreeNode(s));*/
            /*var enumerable = from t in tabelasCorrespondentes //LINQ
                             select new TreeNode(t);*/
            //var treeNodes = tabelasCorrespondentes.Select(t => new TreeNode(t)); //com lambda expression
            //.Select(tabelasCorrespondentes,ConverteStringToTreeNode)
            //cada item de tabelasCorrespondentes é passado como parâmetro para a função ConverteStringToTreeNode
            
            var treeNodes = tabelasCorrespondentes.Select(ConverteStringToTreeNode);
            tv.Nodes.AddRange(treeNodes.ToArray());
        }

        public TreeNode ConverteStringToTreeNode(string s)
        {
            return new TreeNode(s);
        }

        private void bbtMarcarTodos_Click(object sender, EventArgs e)
        {
            MarcarTodos(tvTabelasCorrepondentes);
        }

        private void MarcarTodos(TreeView tv)
        {
            var treeNodes = tv.Nodes.Cast<TreeNode>();
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
           // migradorDados.RecuperaFkDasTabelas();
        }

     
    }
}
