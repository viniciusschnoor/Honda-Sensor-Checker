namespace HondaSensorChecker
{
    partial class Users
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutRoot, layoutForm, layoutFooter;
        private Panel pnlHeader, pnlForm, pnlFooter;
        private Label label1, lblSubtitle, label2, label3, label4, lblHint;
        private DataGridView dgvUsers;
        private TextBox txtRe, txtZfId, txtNome;
        private CheckBox checkBoxAdmin;
        private Button btnSalvar, btnRemover;

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
            txtRe = new TextBox();
            txtZfId = new TextBox();
            txtNome = new TextBox();
            checkBoxAdmin = new CheckBox();
            btnSalvar = new Button();
            dgvUsers = new DataGridView();
            pnlFooter = new Panel();
            layoutFooter = new TableLayoutPanel();
            lblHint = new Label();
            btnRemover = new Button();
            layoutRoot.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlForm.SuspendLayout();
            layoutForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
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
            layoutRoot.Controls.Add(dgvUsers, 0, 2);
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
            label1.Size = new Size(154, 46);
            label1.TabIndex = 0;
            label1.Text = "Usuários";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = Color.FromArgb(218, 232, 245);
            lblSubtitle.Location = new Point(31, 72);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(405, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Cadastre operadores e defina as permissões administrativas";
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
            layoutForm.ColumnCount = 5;
            layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F));
            layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
            layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 144F));
            layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
            layoutForm.Controls.Add(label2, 0, 0);
            layoutForm.Controls.Add(label3, 1, 0);
            layoutForm.Controls.Add(label4, 2, 0);
            layoutForm.Controls.Add(txtRe, 0, 1);
            layoutForm.Controls.Add(txtZfId, 1, 1);
            layoutForm.Controls.Add(txtNome, 2, 1);
            layoutForm.Controls.Add(checkBoxAdmin, 3, 1);
            layoutForm.Controls.Add(btnSalvar, 4, 1);
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
            label2.Size = new Size(26, 20);
            label2.TabIndex = 0;
            label2.Text = "RE";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(70, 80, 90);
            label3.Location = new Point(124, 0);
            label3.Name = "label3";
            label3.Size = new Size(133, 20);
            label3.TabIndex = 1;
            label3.Text = "ZF-ID (Z#######)";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(70, 80, 90);
            label4.Location = new Point(272, 0);
            label4.Name = "label4";
            label4.Size = new Size(54, 20);
            label4.TabIndex = 2;
            label4.Text = "NOME";
            // 
            // txtRe
            // 
            txtRe.BorderStyle = BorderStyle.FixedSingle;
            txtRe.Dock = DockStyle.Fill;
            txtRe.Location = new Point(0, 33);
            txtRe.Margin = new Padding(0, 0, 14, 11);
            txtRe.Name = "txtRe";
            txtRe.Size = new Size(107, 27);
            txtRe.TabIndex = 3;
            // 
            // txtZfId
            // 
            txtZfId.BorderStyle = BorderStyle.FixedSingle;
            txtZfId.Dock = DockStyle.Fill;
            txtZfId.Location = new Point(121, 33);
            txtZfId.Margin = new Padding(0, 0, 14, 11);
            txtZfId.Name = "txtZfId";
            txtZfId.Size = new Size(134, 27);
            txtZfId.TabIndex = 4;
            // 
            // txtNome
            // 
            txtNome.BorderStyle = BorderStyle.FixedSingle;
            txtNome.Dock = DockStyle.Fill;
            txtNome.Location = new Point(269, 33);
            txtNome.Margin = new Padding(0, 0, 14, 11);
            txtNome.Name = "txtNome";
            txtNome.Size = new Size(391, 27);
            txtNome.TabIndex = 5;
            // 
            // checkBoxAdmin
            // 
            checkBoxAdmin.Anchor = AnchorStyles.Left;
            checkBoxAdmin.AutoSize = true;
            checkBoxAdmin.Location = new Point(677, 56);
            checkBoxAdmin.Margin = new Padding(3, 4, 3, 4);
            checkBoxAdmin.Name = "checkBoxAdmin";
            checkBoxAdmin.Size = new Size(126, 24);
            checkBoxAdmin.TabIndex = 6;
            checkBoxAdmin.Text = "Administrador";
            // 
            // btnSalvar
            // 
            btnSalvar.Dock = DockStyle.Fill;
            btnSalvar.Location = new Point(818, 33);
            btnSalvar.Margin = new Padding(0);
            btnSalvar.Name = "btnSalvar";
            btnSalvar.Size = new Size(157, 71);
            btnSalvar.TabIndex = 7;
            btnSalvar.Text = "ADICIONAR";
            btnSalvar.Click += btnSalvar_Click;
            // 
            // dgvUsers
            // 
            dgvUsers.ColumnHeadersHeight = 29;
            dgvUsers.Dock = DockStyle.Fill;
            dgvUsers.Location = new Point(0, 256);
            dgvUsers.Margin = new Padding(0);
            dgvUsers.MultiSelect = false;
            dgvUsers.Name = "dgvUsers";
            dgvUsers.RowHeadersWidth = 51;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.Size = new Size(1029, 461);
            dgvUsers.TabIndex = 2;
            dgvUsers.CellEndEdit += dgvUsers_CellEndEdit;
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
            layoutFooter.Controls.Add(btnRemover, 1, 0);
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
            lblHint.Size = new Size(452, 20);
            lblHint.TabIndex = 0;
            lblHint.Text = "Selecione uma linha para editar diretamente ou remover o usuário.";
            // 
            // btnRemover
            // 
            btnRemover.Dock = DockStyle.Fill;
            btnRemover.Location = new Point(820, 0);
            btnRemover.Margin = new Padding(0);
            btnRemover.Name = "btnRemover";
            btnRemover.Size = new Size(155, 49);
            btnRemover.TabIndex = 1;
            btnRemover.Text = "REMOVER";
            btnRemover.Click += btnRemover_Click;
            // 
            // Users
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1029, 800);
            Controls.Add(layoutRoot);
            FormScreenCaptureMode = ScreenCaptureMode.HideWindow;
            Margin = new Padding(3, 4, 3, 4);
            MinimizeBox = false;
            MinimumSize = new Size(935, 704);
            Name = "Users";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Gerenciamento de Usuários";
            Load += Users_Load;
            layoutRoot.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlForm.ResumeLayout(false);
            layoutForm.ResumeLayout(false);
            layoutForm.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            pnlFooter.ResumeLayout(false);
            layoutFooter.ResumeLayout(false);
            layoutFooter.PerformLayout();
            ResumeLayout(false);
        }
    }
}
