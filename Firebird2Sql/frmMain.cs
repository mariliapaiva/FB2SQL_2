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
        private MigradorDados migradorDados;

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
            var todosNodes = tvTabelasCorrepondentes.Nodes.Cast<TreeNode>();
            var treeNodes = todosNodes.Where(x => x.Checked);
            if (treeNodes.Any())
            {
                try
                {
                    migradorDados.MigrarDados(treeNodes.Select(treeNode => treeNode.Text));
                }
                catch (DependenciasNaoSatisfeitasException ex)
                {
                    var dialogResult = MessageBox.Show(ex.Message + "\nVocê deseja que elas sejam adicionadas?", "Firebird2Sql", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        var nodesDeDependencia = todosNodes.Where(t => ex.Dependencias.Any(d => t.Text == d));
                        foreach (var node in nodesDeDependencia)
                            node.Checked = true;
                    }
                    else
                    {
                        var node = todosNodes.Single(t => t.Text == ex.Tabela);
                        node.Checked = false;
                    }
                }
            }
            else
                MessageBox.Show(string.Format("Não há tabelas marcadas."));
        }

        private void bbtMostrarTabelaFB_Click(object sender, EventArgs e)
        {
            Conexao con = new Conexao();
            con.FbConnectionStringBuilder(textBoxUsu.Text, textBoxSenha.Text, textBoxDatabase.Text, textBoxIP.Text, textBoxPorta.Text);
            con.SqlConnectionStringBuilder(txtServerSql.Text, txtDatabaseSql.Text);
            migradorDados = new MigradorDados(con);
            MostrarTabelas(tvTabelasCorrepondentes);
            lblQtdTabelasFB.Text = string.Format("FIREBIRD: " + migradorDados.QtdTabelasFb);
            lblQtdTabelasSql.Text = string.Format("MSSQL: " + migradorDados.QtdTabelasMsSql);

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
            if (treeNodes.Any())
            {
                foreach (var treeNode in treeNodes)
                {
                    treeNode.Checked = !treeNode.Checked;
                    bbtMarcarTodos.Text = treeNode.Checked ? "&Marcar Todos" : "&Desmarcar Todos"; //Expressão ternária
                }
            }
            else
                MessageBox.Show("Não há tabelas para marcar.");
        }




    }
}
