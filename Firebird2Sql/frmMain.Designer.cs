namespace Firebird2Sql
{
    partial class FrmFirebirdToSql
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxFB = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxPorta = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDatabase = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSenha = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUsu = new System.Windows.Forms.TextBox();
            this.btnMigrar = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtServerSql = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtDatabaseSql = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSenhaSql = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtUsuSql = new System.Windows.Forms.TextBox();
            this.bbtMostrarTabelaFB = new System.Windows.Forms.Button();
            this.tvTabelasCorrepondentes = new System.Windows.Forms.TreeView();
            this.groupBoxFB.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxFB
            // 
            this.groupBoxFB.Controls.Add(this.label5);
            this.groupBoxFB.Controls.Add(this.textBoxPorta);
            this.groupBoxFB.Controls.Add(this.label4);
            this.groupBoxFB.Controls.Add(this.textBoxIP);
            this.groupBoxFB.Controls.Add(this.label3);
            this.groupBoxFB.Controls.Add(this.textBoxDatabase);
            this.groupBoxFB.Controls.Add(this.label2);
            this.groupBoxFB.Controls.Add(this.textBoxSenha);
            this.groupBoxFB.Controls.Add(this.label1);
            this.groupBoxFB.Controls.Add(this.textBoxUsu);
            this.groupBoxFB.Location = new System.Drawing.Point(12, 12);
            this.groupBoxFB.Name = "groupBoxFB";
            this.groupBoxFB.Size = new System.Drawing.Size(274, 163);
            this.groupBoxFB.TabIndex = 6;
            this.groupBoxFB.TabStop = false;
            this.groupBoxFB.Text = "Firebird:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(132, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Porta:";
            // 
            // textBoxPorta
            // 
            this.textBoxPorta.Location = new System.Drawing.Point(135, 123);
            this.textBoxPorta.Name = "textBoxPorta";
            this.textBoxPorta.Size = new System.Drawing.Size(117, 20);
            this.textBoxPorta.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "IP:";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(9, 123);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(117, 20);
            this.textBoxIP.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Banco:";
            // 
            // textBoxDatabase
            // 
            this.textBoxDatabase.Location = new System.Drawing.Point(9, 81);
            this.textBoxDatabase.Name = "textBoxDatabase";
            this.textBoxDatabase.Size = new System.Drawing.Size(240, 20);
            this.textBoxDatabase.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(129, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Senha:";
            // 
            // textBoxSenha
            // 
            this.textBoxSenha.Location = new System.Drawing.Point(132, 42);
            this.textBoxSenha.Name = "textBoxSenha";
            this.textBoxSenha.PasswordChar = '*';
            this.textBoxSenha.Size = new System.Drawing.Size(117, 20);
            this.textBoxSenha.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Usuário:";
            // 
            // textBoxUsu
            // 
            this.textBoxUsu.Location = new System.Drawing.Point(9, 42);
            this.textBoxUsu.Name = "textBoxUsu";
            this.textBoxUsu.Size = new System.Drawing.Size(117, 20);
            this.textBoxUsu.TabIndex = 7;
            // 
            // btnMigrar
            // 
            this.btnMigrar.Location = new System.Drawing.Point(327, 378);
            this.btnMigrar.Name = "btnMigrar";
            this.btnMigrar.Size = new System.Drawing.Size(124, 23);
            this.btnMigrar.TabIndex = 8;
            this.btnMigrar.Text = "Migrar Dados";
            this.btnMigrar.UseVisualStyleBackColor = true;
            this.btnMigrar.Click += new System.EventHandler(this.btnMigrar_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtServerSql);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtDatabaseSql);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtSenhaSql);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txtUsuSql);
            this.groupBox1.Location = new System.Drawing.Point(292, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(274, 163);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "MSSQL:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(20, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "IP:";
            // 
            // txtServerSql
            // 
            this.txtServerSql.Location = new System.Drawing.Point(9, 123);
            this.txtServerSql.Name = "txtServerSql";
            this.txtServerSql.Size = new System.Drawing.Size(117, 20);
            this.txtServerSql.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 65);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Banco:";
            // 
            // txtDatabaseSql
            // 
            this.txtDatabaseSql.Location = new System.Drawing.Point(9, 81);
            this.txtDatabaseSql.Name = "txtDatabaseSql";
            this.txtDatabaseSql.Size = new System.Drawing.Size(240, 20);
            this.txtDatabaseSql.TabIndex = 12;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(129, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 13);
            this.label9.TabIndex = 11;
            this.label9.Text = "Senha:";
            // 
            // txtSenhaSql
            // 
            this.txtSenhaSql.Location = new System.Drawing.Point(132, 42);
            this.txtSenhaSql.Name = "txtSenhaSql";
            this.txtSenhaSql.PasswordChar = '*';
            this.txtSenhaSql.Size = new System.Drawing.Size(117, 20);
            this.txtSenhaSql.TabIndex = 10;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 26);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "Usuário:";
            // 
            // txtUsuSql
            // 
            this.txtUsuSql.Location = new System.Drawing.Point(9, 42);
            this.txtUsuSql.Name = "txtUsuSql";
            this.txtUsuSql.Size = new System.Drawing.Size(117, 20);
            this.txtUsuSql.TabIndex = 7;
            // 
            // bbtMostrarTabelaFB
            // 
            this.bbtMostrarTabelaFB.Location = new System.Drawing.Point(12, 181);
            this.bbtMostrarTabelaFB.Name = "bbtMostrarTabelaFB";
            this.bbtMostrarTabelaFB.Size = new System.Drawing.Size(170, 42);
            this.bbtMostrarTabelaFB.TabIndex = 10;
            this.bbtMostrarTabelaFB.Text = "Mostrar Tabelas Correspondentes";
            this.bbtMostrarTabelaFB.UseVisualStyleBackColor = true;
            this.bbtMostrarTabelaFB.Click += new System.EventHandler(this.bbtMostrarTabelaFB_Click);
            // 
            // tvTabelasCorrepondentes
            // 
            this.tvTabelasCorrepondentes.Location = new System.Drawing.Point(197, 181);
            this.tvTabelasCorrepondentes.Name = "tvTabelasCorrepondentes";
            this.tvTabelasCorrepondentes.Size = new System.Drawing.Size(369, 191);
            this.tvTabelasCorrepondentes.TabIndex = 11;
            // 
            // FrmFirebirdToSql
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 413);
            this.Controls.Add(this.tvTabelasCorrepondentes);
            this.Controls.Add(this.bbtMostrarTabelaFB);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnMigrar);
            this.Controls.Add(this.groupBoxFB);
            this.Name = "FrmFirebirdToSql";
            this.Text = "FB2SQL";
            this.groupBoxFB.ResumeLayout(false);
            this.groupBoxFB.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxFB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxUsu;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxPorta;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDatabase;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSenha;
        private System.Windows.Forms.Button btnMigrar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtServerSql;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtDatabaseSql;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSenhaSql;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtUsuSql;
        private System.Windows.Forms.Button bbtMostrarTabelaFB;
        private System.Windows.Forms.TreeView tvTabelasCorrepondentes;
    }
}

