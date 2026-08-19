namespace HondaSensorChecker
{
    partial class Products
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutRoot, layoutForm, layoutFooter;
        private Panel pnlHeader, pnlForm, pnlFooter;
        private Label label1, lblSubtitle, label2, label3, label4, lblHint;
        private Button btnAdd, btnRemove;
        private TextBox txtPrefix, txtPn, txtElsen;
        private DataGridView dgvSensors;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            layoutRoot = new TableLayoutPanel();
            pnlHeader = new Panel();
            label1 = new Label();
            lblSubtitle = new Label();
            pnlForm = new Panel();
            layoutForm = new TableLayoutPanel();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            txtPrefix = new TextBox();
            txtPn = new TextBox();
            txtElsen = new TextBox();
            btnAdd = new Button();
            dgvSensors = new DataGridView();
            pnlFooter = new Panel();
            layoutFooter = new TableLayoutPanel();
            lblHint = new Label();
            btnRemove = new Button();
            layoutRoot.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlForm.SuspendLayout();
            layoutForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSensors).BeginInit();
            pnlFooter.SuspendLayout();
            layoutFooter.SuspendLayout();
            SuspendLayout();
            // 
            // layoutRoot
            // 
            layoutRoot.ColumnCount = 1;
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRoot.Controls.Add(pnlHeader, 0, 0);
            layoutRoot.Controls.Add(pnlForm, 0, 1);
            layoutRoot.Controls.Add(dgvSensors, 0, 2);
            layoutRoot.Controls.Add(pnlFooter, 0, 3);
            layoutRoot.Dock = DockStyle.Fill;
            layoutRoot.Location = new Point(0, 0);
            layoutRoot.Margin = new Padding(0);
            layoutRoot.Name = "layoutRoot";
            layoutRoot.RowCount = 4;
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 117F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 139F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 83F));
            layoutRoot.Size = new Size(1029, 800);
            layoutRoot.TabIndex = 0;
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(21, 74, 124);
            pnlHeader.Controls.Add(label1);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Fill;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Margin = new Padding(0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1029, 117);
            pnlHeader.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(25, 19);
            label1.Name = "label1";
            label1.Size = new Size(330, 46);
            label1.TabIndex = 0;
            label1.Text = "Produtos e sensores";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = Color.FromArgb(218, 232, 245);
            lblSubtitle.Location = new Point(31, 72);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(502, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Cadastre a correspondência entre prefixo, PartNumber ZF e ELSEN/ELMOD";
            // 
            // pnlForm
            // 
            pnlForm.BackColor = Color.White;
            pnlForm.Controls.Add(layoutForm);
            pnlForm.Dock = DockStyle.Fill;
            pnlForm.Location = new Point(0, 117);
            pnlForm.Margin = new Padding(0);
            pnlForm.Name = "pnlForm";
            pnlForm.Padding = new Padding(27, 16, 27, 19);
            pnlForm.Size = new Size(1029, 139);
            pnlForm.TabIndex = 1;
            // 
            // layoutForm
            // 
            layoutForm.ColumnCount = 4;
            layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 19F));
            layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 27F));
            layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54F));
            layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 157F));
            layoutForm.Controls.Add(label2, 0, 0);
            layoutForm.Controls.Add(label3, 1, 0);
            layoutForm.Controls.Add(label4, 2, 0);
            layoutForm.Controls.Add(txtPrefix, 0, 1);
            layoutForm.Controls.Add(txtPn, 1, 1);
            layoutForm.Controls.Add(txtElsen, 2, 1);
            layoutForm.Controls.Add(btnAdd, 3, 1);
            layoutForm.Dock = DockStyle.Fill;
            layoutForm.Location = new Point(27, 16);
            layoutForm.Margin = new Padding(0);
            layoutForm.Name = "layoutForm";
            layoutForm.RowCount = 2;
            layoutForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            layoutForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 51F));
            layoutForm.Size = new Size(975, 104);
            layoutForm.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(70, 80, 90);
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(67, 20);
            label2.TabIndex = 0;
            label2.Text = "PREFIXO";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(70, 80, 90);
            label3.Location = new Point(158, 0);
            label3.Name = "label3";
            label3.Size = new Size(128, 20);
            label3.TabIndex = 1;
            label3.Text = "PARTNUMBER ZF";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(70, 80, 90);
            label4.Location = new Point(378, 0);
            label4.Name = "label4";
            label4.Size = new Size(117, 20);
            label4.TabIndex = 2;
            label4.Text = "ELSEN / ELMOD";
            // 
            // txtPrefix
            // 
            txtPrefix.BorderStyle = BorderStyle.FixedSingle;
            txtPrefix.Dock = DockStyle.Fill;
            txtPrefix.Location = new Point(0, 33);
            txtPrefix.Margin = new Padding(0, 0, 14, 11);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.Size = new Size(141, 27);
            txtPrefix.TabIndex = 3;
            // 
            // txtPn
            // 
            txtPn.BorderStyle = BorderStyle.FixedSingle;
            txtPn.Dock = DockStyle.Fill;
            txtPn.Location = new Point(155, 33);
            txtPn.Margin = new Padding(0, 0, 14, 11);
            txtPn.Name = "txtPn";
            txtPn.Size = new Size(206, 27);
            txtPn.TabIndex = 4;
            // 
            // txtElsen
            // 
            txtElsen.BorderStyle = BorderStyle.FixedSingle;
            txtElsen.Dock = DockStyle.Fill;
            txtElsen.Location = new Point(375, 33);
            txtElsen.Margin = new Padding(0, 0, 14, 11);
            txtElsen.Name = "txtElsen";
            txtElsen.Size = new Size(427, 27);
            txtElsen.TabIndex = 5;
            // 
            // btnAdd
            // 
            btnAdd.Dock = DockStyle.Fill;
            btnAdd.Location = new Point(816, 33);
            btnAdd.Margin = new Padding(0);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(159, 71);
            btnAdd.TabIndex = 6;
            btnAdd.Text = "ADICIONAR";
            btnAdd.Click += btnAdd_Click;
            // 
            // dgvSensors
            // 
            dgvSensors.ColumnHeadersHeight = 29;
            dgvSensors.Dock = DockStyle.Fill;
            dgvSensors.Location = new Point(0, 256);
            dgvSensors.Margin = new Padding(0);
            dgvSensors.MultiSelect = false;
            dgvSensors.Name = "dgvSensors";
            dgvSensors.RowHeadersWidth = 51;
            dgvSensors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSensors.Size = new Size(1029, 461);
            dgvSensors.TabIndex = 2;
            dgvSensors.CellEndEdit += dgvSensors_CellEndEdit;
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.FromArgb(247, 249, 252);
            pnlFooter.Controls.Add(layoutFooter);
            pnlFooter.Dock = DockStyle.Fill;
            pnlFooter.Location = new Point(0, 717);
            pnlFooter.Margin = new Padding(0);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Padding = new Padding(27, 17, 27, 17);
            pnlFooter.Size = new Size(1029, 83);
            pnlFooter.TabIndex = 3;
            // 
            // layoutFooter
            // 
            layoutFooter.ColumnCount = 2;
            layoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
            layoutFooter.Controls.Add(lblHint, 0, 0);
            layoutFooter.Controls.Add(btnRemove, 1, 0);
            layoutFooter.Dock = DockStyle.Fill;
            layoutFooter.Location = new Point(27, 17);
            layoutFooter.Margin = new Padding(0);
            layoutFooter.Name = "layoutFooter";
            layoutFooter.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            layoutFooter.Size = new Size(975, 49);
            layoutFooter.TabIndex = 0;
            // 
            // lblHint
            // 
            lblHint.Anchor = AnchorStyles.Left;
            lblHint.AutoSize = true;
            lblHint.ForeColor = Color.FromArgb(80, 90, 100);
            lblHint.Location = new Point(3, 14);
            lblHint.Name = "lblHint";
            lblHint.Size = new Size(458, 20);
            lblHint.TabIndex = 0;
            lblHint.Text = "Selecione uma linha para editar diretamente ou remover o produto.";
            // 
            // btnRemove
            // 
            btnRemove.Dock = DockStyle.Fill;
            btnRemove.Location = new Point(820, 0);
            btnRemove.Margin = new Padding(0);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(155, 49);
            btnRemove.TabIndex = 1;
            btnRemove.Text = "REMOVER";
            btnRemove.Click += btnRemove_Click;
            // 
            // Products
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1029, 800);
            Controls.Add(layoutRoot);
            Margin = new Padding(3, 4, 3, 4);
            MinimizeBox = false;
            MinimumSize = new Size(935, 704);
            Name = "Products";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Gerenciamento de Produtos";
            Load += Products_Load;
            layoutRoot.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlForm.ResumeLayout(false);
            layoutForm.ResumeLayout(false);
            layoutForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSensors).EndInit();
            pnlFooter.ResumeLayout(false);
            layoutFooter.ResumeLayout(false);
            layoutFooter.PerformLayout();
            ResumeLayout(false);
        }
    }
}
