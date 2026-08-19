namespace HondaSensorChecker
{
    partial class ComponentHistoryDialog
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutRoot, layoutSearch;
        private Panel pnlHeader, pnlSearch;
        private Label lblTitle, lblSubtitle, lblSerial;
        private TextBox txtSerial;
        private Button btnSearch;
        private ListView listView;
        private ColumnHeader colSerial, colDataHora, colWorkOrder, colSupplierBox, colZfBox, colOperator, colStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            layoutRoot = new TableLayoutPanel(); layoutSearch = new TableLayoutPanel(); pnlHeader = new Panel(); pnlSearch = new Panel();
            lblTitle = new Label(); lblSubtitle = new Label(); lblSerial = new Label(); txtSerial = new TextBox(); btnSearch = new Button(); listView = new ListView();
            colSerial = new ColumnHeader(); colDataHora = new ColumnHeader(); colWorkOrder = new ColumnHeader(); colSupplierBox = new ColumnHeader(); colZfBox = new ColumnHeader(); colOperator = new ColumnHeader(); colStatus = new ColumnHeader();
            layoutRoot.SuspendLayout(); layoutSearch.SuspendLayout(); pnlHeader.SuspendLayout(); pnlSearch.SuspendLayout(); SuspendLayout();

            layoutRoot.ColumnCount = 1; layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layoutRoot.Dock = DockStyle.Fill; layoutRoot.Margin = Padding.Empty;
            layoutRoot.RowCount = 3; layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F)); layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F)); layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutRoot.Controls.Add(pnlHeader, 0, 0); layoutRoot.Controls.Add(pnlSearch, 0, 1); layoutRoot.Controls.Add(listView, 0, 2);

            pnlHeader.BackColor = Color.FromArgb(21, 74, 124); pnlHeader.Dock = DockStyle.Fill; pnlHeader.Margin = Padding.Empty; pnlHeader.Controls.AddRange(new Control[] { lblTitle, lblSubtitle });
            lblTitle.AutoSize = true; lblTitle.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold); lblTitle.ForeColor = Color.White; lblTitle.Location = new Point(22, 14); lblTitle.Text = "Consultar componente";
            lblSubtitle.AutoSize = true; lblSubtitle.ForeColor = Color.FromArgb(218, 232, 245); lblSubtitle.Location = new Point(27, 54); lblSubtitle.Text = "Consulte a rastreabilidade e o status de um sensor";

            pnlSearch.BackColor = Color.White; pnlSearch.Dock = DockStyle.Fill; pnlSearch.Margin = Padding.Empty; pnlSearch.Padding = new Padding(24, 12, 24, 14); pnlSearch.Controls.Add(layoutSearch);
            layoutSearch.ColumnCount = 2; layoutSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); layoutSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136F));
            layoutSearch.Dock = DockStyle.Fill; layoutSearch.Margin = Padding.Empty; layoutSearch.RowCount = 2; layoutSearch.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F)); layoutSearch.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            layoutSearch.Controls.Add(lblSerial, 0, 0); layoutSearch.Controls.Add(txtSerial, 0, 1); layoutSearch.Controls.Add(btnSearch, 1, 1);
            lblSerial.AutoSize = true; lblSerial.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold); lblSerial.ForeColor = Color.FromArgb(70, 80, 90); lblSerial.Text = "SERIAL DO SENSOR";
            txtSerial.BorderStyle = BorderStyle.FixedSingle; txtSerial.Dock = DockStyle.Fill; txtSerial.Font = new Font("Segoe UI", 11F); txtSerial.Margin = new Padding(0, 0, 12, 8);
            txtSerial.PlaceholderText = "Digite ou escaneie o serial do sensor"; txtSerial.KeyPress += TxtSerial_KeyPress;
            btnSearch.Dock = DockStyle.Fill; btnSearch.Margin = Padding.Empty; btnSearch.Text = "PESQUISAR"; btnSearch.Click += BtnSearch_Click;

            listView.BackColor = Color.White; listView.BorderStyle = BorderStyle.None; listView.Columns.AddRange(new ColumnHeader[] { colSerial, colDataHora, colWorkOrder, colSupplierBox, colZfBox, colOperator, colStatus });
            listView.Dock = DockStyle.Fill; listView.Font = new Font("Segoe UI", 9.5F); listView.FullRowSelect = true; listView.GridLines = true; listView.HideSelection = false; listView.Margin = Padding.Empty;
            listView.UseCompatibleStateImageBehavior = false; listView.View = View.Details;
            colSerial.Text = "SERIAL"; colSerial.Width = 145; colDataHora.Text = "DATA E HORA"; colDataHora.Width = 155; colWorkOrder.Text = "WORK ORDER"; colWorkOrder.Width = 130;
            colSupplierBox.Text = "SUPPLIER BOX"; colSupplierBox.Width = 135; colZfBox.Text = "ZF BOX"; colZfBox.Width = 125;
            colOperator.Text = "USUÁRIO DO SCAN"; colOperator.Width = 175; colStatus.Text = "STATUS"; colStatus.Width = 110;

            ClientSize = new Size(1015, 480); Controls.Add(layoutRoot); MaximizeBox = false; MinimumSize = new Size(900, 430); Name = "ComponentHistoryDialog";
            StartPosition = FormStartPosition.CenterParent; Text = "Consultar componente";
            layoutRoot.ResumeLayout(false); layoutSearch.ResumeLayout(false); layoutSearch.PerformLayout(); pnlHeader.ResumeLayout(false); pnlHeader.PerformLayout(); pnlSearch.ResumeLayout(false); ResumeLayout(false);
        }
    }
}
