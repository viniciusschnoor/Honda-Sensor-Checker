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
            layoutRoot = new TableLayoutPanel();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            pnlSearch = new Panel();
            layoutSearch = new TableLayoutPanel();
            lblSerial = new Label();
            txtSerial = new TextBox();
            btnSearch = new Button();
            listView = new ListView();
            colSerial = new ColumnHeader();
            colDataHora = new ColumnHeader();
            colWorkOrder = new ColumnHeader();
            colSupplierBox = new ColumnHeader();
            colZfBox = new ColumnHeader();
            colOperator = new ColumnHeader();
            colStatus = new ColumnHeader();
            layoutRoot.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlSearch.SuspendLayout();
            layoutSearch.SuspendLayout();
            SuspendLayout();
            // 
            // layoutRoot
            // 
            layoutRoot.ColumnCount = 1;
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRoot.Controls.Add(pnlHeader, 0, 0);
            layoutRoot.Controls.Add(pnlSearch, 0, 1);
            layoutRoot.Controls.Add(listView, 0, 2);
            layoutRoot.Dock = DockStyle.Fill;
            layoutRoot.Location = new Point(0, 0);
            layoutRoot.Margin = new Padding(0);
            layoutRoot.Name = "layoutRoot";
            layoutRoot.RowCount = 3;
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutRoot.Size = new Size(1015, 480);
            layoutRoot.TabIndex = 0;
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(21, 74, 124);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Fill;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Margin = new Padding(0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1015, 88);
            pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(22, 14);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(371, 46);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Consultar componente";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = Color.FromArgb(218, 232, 245);
            lblSubtitle.Location = new Point(27, 54);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(341, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Consulte a rastreabilidade e o status de um sensor";
            // 
            // pnlSearch
            // 
            pnlSearch.BackColor = Color.White;
            pnlSearch.Controls.Add(layoutSearch);
            pnlSearch.Dock = DockStyle.Fill;
            pnlSearch.Location = new Point(0, 88);
            pnlSearch.Margin = new Padding(0);
            pnlSearch.Name = "pnlSearch";
            pnlSearch.Padding = new Padding(24, 12, 24, 14);
            pnlSearch.Size = new Size(1015, 88);
            pnlSearch.TabIndex = 1;
            // 
            // layoutSearch
            // 
            layoutSearch.ColumnCount = 2;
            layoutSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136F));
            layoutSearch.Controls.Add(lblSerial, 0, 0);
            layoutSearch.Controls.Add(txtSerial, 0, 1);
            layoutSearch.Controls.Add(btnSearch, 1, 1);
            layoutSearch.Dock = DockStyle.Fill;
            layoutSearch.Location = new Point(24, 12);
            layoutSearch.Margin = new Padding(0);
            layoutSearch.Name = "layoutSearch";
            layoutSearch.RowCount = 2;
            layoutSearch.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            layoutSearch.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            layoutSearch.Size = new Size(967, 62);
            layoutSearch.TabIndex = 0;
            // 
            // lblSerial
            // 
            lblSerial.AutoSize = true;
            lblSerial.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblSerial.ForeColor = Color.FromArgb(70, 80, 90);
            lblSerial.Location = new Point(3, 0);
            lblSerial.Name = "lblSerial";
            lblSerial.Size = new Size(141, 20);
            lblSerial.TabIndex = 0;
            lblSerial.Text = "SERIAL DO SENSOR";
            // 
            // txtSerial
            // 
            txtSerial.BorderStyle = BorderStyle.FixedSingle;
            txtSerial.Dock = DockStyle.Fill;
            txtSerial.Font = new Font("Segoe UI", 11F);
            txtSerial.Location = new Point(0, 25);
            txtSerial.Margin = new Padding(0, 0, 12, 8);
            txtSerial.Name = "txtSerial";
            txtSerial.PlaceholderText = "Digite ou escaneie o serial do sensor";
            txtSerial.Size = new Size(819, 32);
            txtSerial.TabIndex = 1;
            txtSerial.KeyPress += TxtSerial_KeyPress;
            // 
            // btnSearch
            // 
            btnSearch.Dock = DockStyle.Fill;
            btnSearch.Location = new Point(831, 25);
            btnSearch.Margin = new Padding(0);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(136, 38);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "PESQUISAR";
            btnSearch.Click += BtnSearch_Click;
            // 
            // listView
            // 
            listView.BackColor = Color.White;
            listView.BorderStyle = BorderStyle.None;
            listView.Columns.AddRange(new ColumnHeader[] { colSerial, colDataHora, colWorkOrder, colSupplierBox, colZfBox, colOperator, colStatus });
            listView.Dock = DockStyle.Fill;
            listView.Font = new Font("Segoe UI", 9.5F);
            listView.FullRowSelect = true;
            listView.GridLines = true;
            listView.Location = new Point(0, 176);
            listView.Margin = new Padding(0);
            listView.Name = "listView";
            listView.Size = new Size(1015, 304);
            listView.TabIndex = 2;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            // 
            // colSerial
            // 
            colSerial.Text = "SERIAL";
            colSerial.Width = 145;
            // 
            // colDataHora
            // 
            colDataHora.Text = "DATA E HORA";
            colDataHora.Width = 155;
            // 
            // colWorkOrder
            // 
            colWorkOrder.Text = "WORK ORDER";
            colWorkOrder.Width = 130;
            // 
            // colSupplierBox
            // 
            colSupplierBox.Text = "SUPPLIER BOX";
            colSupplierBox.Width = 135;
            // 
            // colZfBox
            // 
            colZfBox.Text = "ZF BOX";
            colZfBox.Width = 125;
            // 
            // colOperator
            // 
            colOperator.Text = "USUÁRIO DO SCAN";
            colOperator.Width = 175;
            // 
            // colStatus
            // 
            colStatus.Text = "STATUS";
            colStatus.Width = 110;
            // 
            // ComponentHistoryDialog
            // 
            ClientSize = new Size(1015, 480);
            Controls.Add(layoutRoot);
            MaximizeBox = false;
            MinimumSize = new Size(900, 430);
            Name = "ComponentHistoryDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Consultar componente";
            layoutRoot.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlSearch.ResumeLayout(false);
            layoutSearch.ResumeLayout(false);
            layoutSearch.PerformLayout();
            ResumeLayout(false);
        }
    }
}
