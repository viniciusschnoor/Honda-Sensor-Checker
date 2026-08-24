namespace HondaSensorChecker
{
    partial class AdminAuthorizationDialog
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Panel pnlContent;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblAdminRe;
        private TextBox txtAdminRe;
        private Button btnCancel;
        private Button btnAuthorize;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlContent = new Panel();
            lblAdminRe = new Label();
            txtAdminRe = new TextBox();
            btnCancel = new Button();
            btnAuthorize = new Button();
            pnlHeader.SuspendLayout();
            pnlContent.SuspendLayout();
            SuspendLayout();
            // pnlHeader
            pnlHeader.BackColor = Color.FromArgb(21, 74, 124);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(480, 82);
            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 17F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(319, 40);
            lblTitle.Text = "Autorização necessária";
            // lblSubtitle
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = Color.FromArgb(218, 232, 245);
            lblSubtitle.Location = new Point(24, 51);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(409, 20);
            lblSubtitle.Text = "Informe o RE de qualquer administrador cadastrado";
            // pnlContent
            pnlContent.BackColor = Color.White;
            pnlContent.Controls.Add(lblAdminRe);
            pnlContent.Controls.Add(txtAdminRe);
            pnlContent.Controls.Add(btnCancel);
            pnlContent.Controls.Add(btnAuthorize);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 82);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(480, 178);
            // lblAdminRe
            lblAdminRe.AutoSize = true;
            lblAdminRe.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblAdminRe.ForeColor = Color.FromArgb(80, 90, 100);
            lblAdminRe.Location = new Point(24, 18);
            lblAdminRe.Name = "lblAdminRe";
            lblAdminRe.Size = new Size(173, 20);
            lblAdminRe.Text = "RE DO ADMINISTRADOR";
            // txtAdminRe
            txtAdminRe.BorderStyle = BorderStyle.FixedSingle;
            txtAdminRe.CharacterCasing = CharacterCasing.Upper;
            txtAdminRe.Font = new Font("Segoe UI", 13F);
            txtAdminRe.Location = new Point(24, 43);
            txtAdminRe.Name = "txtAdminRe";
            txtAdminRe.Size = new Size(432, 36);
            txtAdminRe.TabIndex = 0;
            // btnCancel
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(218, 102);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(110, 38);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "CANCELAR";
            // btnAuthorize
            btnAuthorize.DialogResult = DialogResult.OK;
            btnAuthorize.Location = new Point(340, 102);
            btnAuthorize.Name = "btnAuthorize";
            btnAuthorize.Size = new Size(116, 38);
            btnAuthorize.TabIndex = 2;
            btnAuthorize.Text = "AUTORIZAR";
            // AdminAuthorizationDialog
            AcceptButton = btnAuthorize;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(247, 249, 252);
            CancelButton = btnCancel;
            ClientSize = new Size(480, 260);
            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AdminAuthorizationDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Autorização administrativa";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            ResumeLayout(false);
        }
    }
}
