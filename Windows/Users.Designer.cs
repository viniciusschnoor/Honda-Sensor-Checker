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
            layoutRoot = new TableLayoutPanel(); layoutForm = new TableLayoutPanel(); layoutFooter = new TableLayoutPanel();
            pnlHeader = new Panel(); pnlForm = new Panel(); pnlFooter = new Panel(); label1 = new Label(); lblSubtitle = new Label();
            label2 = new Label(); label3 = new Label(); label4 = new Label(); lblHint = new Label(); dgvUsers = new DataGridView();
            txtRe = new TextBox(); txtZfId = new TextBox(); txtNome = new TextBox(); checkBoxAdmin = new CheckBox(); btnSalvar = new Button(); btnRemover = new Button();
            layoutRoot.SuspendLayout(); layoutForm.SuspendLayout(); layoutFooter.SuspendLayout(); pnlHeader.SuspendLayout(); pnlForm.SuspendLayout(); pnlFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit(); SuspendLayout();

            layoutRoot.ColumnCount = 1; layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layoutRoot.Dock = DockStyle.Fill; layoutRoot.Margin = Padding.Empty;
            layoutRoot.RowCount = 4; layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F)); layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 104F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            layoutRoot.Controls.Add(pnlHeader, 0, 0); layoutRoot.Controls.Add(pnlForm, 0, 1); layoutRoot.Controls.Add(dgvUsers, 0, 2); layoutRoot.Controls.Add(pnlFooter, 0, 3);

            pnlHeader.BackColor = Color.FromArgb(21, 74, 124); pnlHeader.Dock = DockStyle.Fill; pnlHeader.Margin = Padding.Empty; pnlHeader.Controls.AddRange(new Control[] { label1, lblSubtitle });
            label1.AutoSize = true; label1.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold); label1.ForeColor = Color.White; label1.Location = new Point(22, 14); label1.Text = "Usuários";
            lblSubtitle.AutoSize = true; lblSubtitle.ForeColor = Color.FromArgb(218, 232, 245); lblSubtitle.Location = new Point(27, 54); lblSubtitle.Text = "Cadastre operadores e defina as permissões administrativas";

            pnlForm.BackColor = Color.White; pnlForm.Dock = DockStyle.Fill; pnlForm.Margin = Padding.Empty; pnlForm.Padding = new Padding(24, 12, 24, 14); pnlForm.Controls.Add(layoutForm);
            layoutForm.ColumnCount = 5; layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F)); layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
            layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F)); layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 126F)); layoutForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136F));
            layoutForm.Dock = DockStyle.Fill; layoutForm.Margin = Padding.Empty; layoutForm.RowCount = 2; layoutForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F)); layoutForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            layoutForm.Controls.Add(label2, 0, 0); layoutForm.Controls.Add(label3, 1, 0); layoutForm.Controls.Add(label4, 2, 0);
            layoutForm.Controls.Add(txtRe, 0, 1); layoutForm.Controls.Add(txtZfId, 1, 1); layoutForm.Controls.Add(txtNome, 2, 1); layoutForm.Controls.Add(checkBoxAdmin, 3, 1); layoutForm.Controls.Add(btnSalvar, 4, 1);
            label2.AutoSize = true; label2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold); label2.ForeColor = Color.FromArgb(70, 80, 90); label2.Text = "RE";
            label3.AutoSize = true; label3.Font = label2.Font; label3.ForeColor = label2.ForeColor; label3.Text = "ZF-ID (Z#######)";
            label4.AutoSize = true; label4.Font = label2.Font; label4.ForeColor = label2.ForeColor; label4.Text = "NOME";
            txtRe.Dock = DockStyle.Fill; txtRe.Margin = new Padding(0, 0, 12, 8); txtRe.BorderStyle = BorderStyle.FixedSingle;
            txtZfId.Dock = DockStyle.Fill; txtZfId.Margin = new Padding(0, 0, 12, 8); txtZfId.BorderStyle = BorderStyle.FixedSingle;
            txtNome.Dock = DockStyle.Fill; txtNome.Margin = new Padding(0, 0, 12, 8); txtNome.BorderStyle = BorderStyle.FixedSingle;
            checkBoxAdmin.Anchor = AnchorStyles.Left; checkBoxAdmin.AutoSize = true; checkBoxAdmin.Text = "Administrador";
            btnSalvar.Dock = DockStyle.Fill; btnSalvar.Margin = Padding.Empty; btnSalvar.Text = "ADICIONAR"; btnSalvar.Click += btnSalvar_Click;

            dgvUsers.Dock = DockStyle.Fill; dgvUsers.Margin = Padding.Empty; dgvUsers.MultiSelect = false; dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect; dgvUsers.CellEndEdit += dgvUsers_CellEndEdit;

            pnlFooter.BackColor = Color.FromArgb(247, 249, 252); pnlFooter.Dock = DockStyle.Fill; pnlFooter.Margin = Padding.Empty; pnlFooter.Padding = new Padding(24, 13, 24, 13); pnlFooter.Controls.Add(layoutFooter);
            layoutFooter.ColumnCount = 2; layoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layoutFooter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136F)); layoutFooter.Dock = DockStyle.Fill; layoutFooter.Margin = Padding.Empty;
            layoutFooter.Controls.Add(lblHint, 0, 0); layoutFooter.Controls.Add(btnRemover, 1, 0);
            lblHint.AutoSize = true; lblHint.Anchor = AnchorStyles.Left; lblHint.ForeColor = Color.FromArgb(80, 90, 100); lblHint.Text = "Selecione uma linha para editar diretamente ou remover o usuário.";
            btnRemover.Dock = DockStyle.Fill; btnRemover.Margin = Padding.Empty; btnRemover.Text = "REMOVER"; btnRemover.Click += btnRemover_Click;

            AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; ClientSize = new Size(900, 600); Controls.Add(layoutRoot);
            FormScreenCaptureMode = ScreenCaptureMode.HideWindow; MinimizeBox = false; MinimumSize = new Size(820, 540); Name = "Users"; StartPosition = FormStartPosition.CenterParent;
            Text = "Gerenciamento de Usuários"; Load += Users_Load;
            layoutRoot.ResumeLayout(false); layoutForm.ResumeLayout(false); layoutForm.PerformLayout(); layoutFooter.ResumeLayout(false); layoutFooter.PerformLayout();
            pnlHeader.ResumeLayout(false); pnlHeader.PerformLayout(); pnlForm.ResumeLayout(false); pnlFooter.ResumeLayout(false); ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit(); ResumeLayout(false);
        }
    }
}
