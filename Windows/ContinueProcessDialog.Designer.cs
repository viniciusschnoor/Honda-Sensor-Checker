namespace HondaSensorChecker
{
    partial class ContinueProcessDialog
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutRoot, layoutButtons;
        private Panel pnlHeader, buttonPanel;
        private Label lblTitle, lblSubtitle, lblInstruction;
        private ListBox listBox;
        private Button btnCancel, btnOk;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container(); layoutRoot = new TableLayoutPanel(); layoutButtons = new TableLayoutPanel(); pnlHeader = new Panel(); buttonPanel = new Panel();
            lblTitle = new Label(); lblSubtitle = new Label(); lblInstruction = new Label(); listBox = new ListBox(); btnCancel = new Button(); btnOk = new Button();
            layoutRoot.SuspendLayout(); layoutButtons.SuspendLayout(); pnlHeader.SuspendLayout(); buttonPanel.SuspendLayout(); SuspendLayout();

            layoutRoot.ColumnCount = 1; layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layoutRoot.Dock = DockStyle.Fill; layoutRoot.Margin = Padding.Empty;
            layoutRoot.RowCount = 4; layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F)); layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            layoutRoot.Controls.Add(pnlHeader, 0, 0); layoutRoot.Controls.Add(lblInstruction, 0, 1); layoutRoot.Controls.Add(listBox, 0, 2); layoutRoot.Controls.Add(buttonPanel, 0, 3);

            pnlHeader.BackColor = Color.FromArgb(21, 74, 124); pnlHeader.Dock = DockStyle.Fill; pnlHeader.Margin = Padding.Empty; pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });
            lblTitle.AutoSize = true; lblTitle.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold); lblTitle.ForeColor = Color.White; lblTitle.Location = new Point(22, 14); lblTitle.Text = "Continuar processo";
            lblSubtitle.AutoSize = true; lblSubtitle.ForeColor = Color.FromArgb(218, 232, 245); lblSubtitle.Location = new Point(27, 54); lblSubtitle.Text = "Retome uma caixa que ainda está em andamento";

            lblInstruction.BackColor = Color.White; lblInstruction.Dock = DockStyle.Fill; lblInstruction.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold); lblInstruction.ForeColor = Color.FromArgb(70, 80, 90);
            lblInstruction.Margin = Padding.Empty; lblInstruction.Padding = new Padding(24, 18, 0, 0); lblInstruction.Text = "SELECIONE O PROCESSO QUE DESEJA CONTINUAR";
            listBox.BackColor = Color.White; listBox.BorderStyle = BorderStyle.None; listBox.Dock = DockStyle.Fill; listBox.DisplayMember = "Display"; listBox.Font = new Font("Segoe UI", 11F);
            listBox.IntegralHeight = false; listBox.ItemHeight = 28; listBox.Margin = Padding.Empty; listBox.Padding = new Padding(12); listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;

            buttonPanel.BackColor = Color.FromArgb(247, 249, 252); buttonPanel.Dock = DockStyle.Fill; buttonPanel.Margin = Padding.Empty; buttonPanel.Padding = new Padding(24, 17, 16, 17); buttonPanel.Controls.Add(layoutButtons);
            layoutButtons.ColumnCount = 3; layoutButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layoutButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 122F)); layoutButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 122F));
            layoutButtons.Dock = DockStyle.Fill; layoutButtons.Margin = Padding.Empty; layoutButtons.Controls.Add(btnCancel, 1, 0); layoutButtons.Controls.Add(btnOk, 2, 0);
            btnCancel.Dock = DockStyle.Fill; btnCancel.Margin = new Padding(0, 0, 12, 0); btnCancel.Text = "CANCELAR"; btnCancel.Click += BtnCancel_Click;
            btnOk.Dock = DockStyle.Fill; btnOk.Enabled = false; btnOk.Margin = Padding.Empty; btnOk.Text = "CONTINUAR"; btnOk.Click += BtnOk_Click;

            ClientSize = new Size(640, 430); Controls.Add(layoutRoot); MaximizeBox = false; MinimizeBox = false; MinimumSize = new Size(580, 400); Name = "ContinueProcessDialog";
            StartPosition = FormStartPosition.CenterParent; Text = "Continuar processo anterior";
            layoutRoot.ResumeLayout(false); layoutButtons.ResumeLayout(false); pnlHeader.ResumeLayout(false); pnlHeader.PerformLayout(); buttonPanel.ResumeLayout(false); ResumeLayout(false);
        }
    }
}
