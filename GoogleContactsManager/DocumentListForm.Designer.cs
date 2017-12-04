namespace GoogleContactsManager
{
    partial class DocumentListForm
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentListForm));
            this.gridDocs = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TemAlteracoes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.googleContactsDocListBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonConta = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonPasta = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSaveLocal = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonCancelar = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonSinc = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonExport = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.gridDocs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.googleContactsDocListBindingSource)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridDocs
            // 
            this.gridDocs.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridDocs.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gridDocs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridDocs.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.TemAlteracoes,
            this.dataGridViewTextBoxColumn2});
            this.gridDocs.DataSource = this.googleContactsDocListBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridDocs.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridDocs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDocs.Location = new System.Drawing.Point(0, 25);
            this.gridDocs.Name = "gridDocs";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridDocs.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.gridDocs.Size = new System.Drawing.Size(812, 260);
            this.gridDocs.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "Status";
            this.dataGridViewTextBoxColumn1.HeaderText = "Status";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 62;
            // 
            // TemAlteracoes
            // 
            this.TemAlteracoes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.TemAlteracoes.DataPropertyName = "TemAlteracoes";
            this.TemAlteracoes.HeaderText = "Alterações";
            this.TemAlteracoes.Name = "TemAlteracoes";
            this.TemAlteracoes.ReadOnly = true;
            this.TemAlteracoes.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TemAlteracoes.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TemAlteracoes.Width = 63;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "ContaId";
            this.dataGridViewTextBoxColumn2.HeaderText = "ContaId";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // googleContactsDocListBindingSource
            // 
            this.googleContactsDocListBindingSource.DataSource = typeof(GoogleContactsManager.GoogleContactsDoc);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 285);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(812, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonConta,
            this.toolStripButtonSave,
            this.toolStripSeparator1,
            this.toolStripButtonPasta,
            this.toolStripButtonSaveLocal,
            this.toolStripSeparator2,
            this.toolStripButtonCancelar,
            this.toolStripButtonSinc,
            this.toolStripButtonExport});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(812, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonConta
            // 
            this.toolStripButtonConta.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonConta.Image")));
            this.toolStripButtonConta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonConta.Name = "toolStripButtonConta";
            this.toolStripButtonConta.Size = new System.Drawing.Size(111, 22);
            this.toolStripButtonConta.Text = "&Adicionar conta";
            this.toolStripButtonConta.ToolTipText = "Fazer ligação ao Google para obter os dados de uma conta";
            this.toolStripButtonConta.Click += new System.EventHandler(this.toolStripButtonConta_Click);
            // 
            // toolStripButtonSave
            // 
            this.toolStripButtonSave.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSave.Image")));
            this.toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSave.Name = "toolStripButtonSave";
            this.toolStripButtonSave.Size = new System.Drawing.Size(125, 22);
            this.toolStripButtonSave.Text = "Guardar alterações";
            this.toolStripButtonSave.ToolTipText = "Guardar alterações no Google";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonPasta
            // 
            this.toolStripButtonPasta.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonPasta.Image")));
            this.toolStripButtonPasta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPasta.Name = "toolStripButtonPasta";
            this.toolStripButtonPasta.Size = new System.Drawing.Size(84, 22);
            this.toolStripButtonPasta.Text = "Abrir &pasta";
            this.toolStripButtonPasta.ToolTipText = "Abrir os ficheiros guardados numa pasta";
            this.toolStripButtonPasta.Click += new System.EventHandler(this.toolStripButtonPasta_Click);
            // 
            // toolStripButtonSaveLocal
            // 
            this.toolStripButtonSaveLocal.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSaveLocal.Image")));
            this.toolStripButtonSaveLocal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSaveLocal.Name = "toolStripButtonSaveLocal";
            this.toolStripButtonSaveLocal.Size = new System.Drawing.Size(134, 22);
            this.toolStripButtonSaveLocal.Text = "Guardar numa pasta";
            this.toolStripButtonSaveLocal.ToolTipText = "Guardar os contactos da conta selecionada numa pasta do computador";
            this.toolStripButtonSaveLocal.Click += new System.EventHandler(this.toolStripButtonSaveLocal_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonCancelar
            // 
            this.toolStripButtonCancelar.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonCancelar.Image")));
            this.toolStripButtonCancelar.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCancelar.Name = "toolStripButtonCancelar";
            this.toolStripButtonCancelar.Size = new System.Drawing.Size(73, 22);
            this.toolStripButtonCancelar.Text = "Cancelar";
            this.toolStripButtonCancelar.Click += new System.EventHandler(this.toolStripButtonCancelar_Click);
            // 
            // toolStripButtonSinc
            // 
            this.toolStripButtonSinc.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonSinc.Image")));
            this.toolStripButtonSinc.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonSinc.Name = "toolStripButtonSinc";
            this.toolStripButtonSinc.Size = new System.Drawing.Size(85, 22);
            this.toolStripButtonSinc.Text = "&Sincronizar";
            this.toolStripButtonSinc.ToolTipText = "Sincronizar contactos entre duas contas";
            this.toolStripButtonSinc.Click += new System.EventHandler(this.toolStripButtonSinc_Click);
            // 
            // toolStripButtonExport
            // 
            this.toolStripButtonExport.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonExport.Image")));
            this.toolStripButtonExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonExport.Name = "toolStripButtonExport";
            this.toolStripButtonExport.Size = new System.Drawing.Size(70, 22);
            this.toolStripButtonExport.Text = "E&xportar";
            this.toolStripButtonExport.Click += new System.EventHandler(this.toolStripButtonExport_Click);
            // 
            // DocumentListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 307);
            this.Controls.Add(this.gridDocs);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DocumentListForm";
            this.Text = "DocumentListForm";
            ((System.ComponentModel.ISupportInitialize)(this.gridDocs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.googleContactsDocListBindingSource)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource googleContactsDocListBindingSource;
        private System.Windows.Forms.DataGridView gridDocs;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonPasta;
        private System.Windows.Forms.ToolStripButton toolStripButtonConta;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButtonCancelar;
        private System.Windows.Forms.ToolStripButton toolStripButtonSaveLocal;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolStripButtonSinc;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn TemAlteracoes;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.ToolStripButton toolStripButtonExport;
    }
}