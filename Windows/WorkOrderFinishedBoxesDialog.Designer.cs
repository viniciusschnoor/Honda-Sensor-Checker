namespace HondaSensorChecker
{
    partial class WorkOrderFinishedBoxesDialog
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutRoot;
        private Panel pnlHeader;
        private Panel pnlSearch;
        private Panel pnlFooter;
        private TableLayoutPanel layoutSearch;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblWorkOrder;
        private Label lblResult;
        private Label lblHint;
        private TextBox txtWorkOrder;
        private Button btnSearch;
        private Button btnOpenBox;
        private ListView listBoxes;
        private ColumnHeader colHu;
        private ColumnHeader colBatch;
        private ColumnHeader colQuantity;
        private ColumnHeader colPartNumber;
        private ColumnHeader colOperator;

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
            lblWorkOrder = new Label();
            txtWorkOrder = new TextBox();
            btnSearch = new Button();
            lblResult = new Label();
            listBoxes = new ListView();
            colHu = new ColumnHeader();
            colBatch = new ColumnHeader();
            colQuantity = new ColumnHeader();
            colPartNumber = new ColumnHeader();
            colOperator = new ColumnHeader();
            pnlFooter = new Panel();
            lblHint = new Label();
            btnOpenBox = new Button();
            layoutRoot.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlSearch.SuspendLayout();
            layoutSearch.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            // layoutRoot
            layoutRoot.ColumnCount = 1;
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRoot.Controls.Add(pnlHeader, 0, 0);
            layoutRoot.Controls.Add(pnlSearch, 0, 1);
            layoutRoot.Controls.Add(listBoxes, 0, 2);
            layoutRoot.Controls.Add(pnlFooter, 0, 3);
            layoutRoot.Dock = DockStyle.Fill;
            layoutRoot.Location = new Point(0, 0);
            layoutRoot.Margin = new Padding(0);
            layoutRoot.Name = "layoutRoot";
            layoutRoot.RowCount = 4;
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            layoutRoot.Size = new Size(980, 590);
            // pnlHeader
            pnlHeader.BackColor = Color.FromArgb(21, 74, 124);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Fill;
            pnlHeader.Margin = new Padding(0);
            pnlHeader.Name = "pnlHeader";
            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(22, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Text = "Caixas finalizadas por Work Order";
            // lblSubtitle
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = Color.FromArgb(218, 232, 245);
            lblSubtitle.Location = new Point(27, 55);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Text = "Localize as HUs finalizadas e consulte os componentes de cada caixa";
            // pnlSearch
            pnlSearch.BackColor = Color.White;
            pnlSearch.Controls.Add(layoutSearch);
            pnlSearch.Controls.Add(lblResult);
            pnlSearch.Dock = DockStyle.Fill;
            pnlSearch.Margin = new Padding(0);
            pnlSearch.Name = "pnlSearch";
            pnlSearch.Padding = new Padding(24, 12, 24, 8);
            // layoutSearch
            layoutSearch.ColumnCount = 2;
            layoutSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutSearch.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
            layoutSearch.Controls.Add(lblWorkOrder, 0, 0);
            layoutSearch.Controls.Add(txtWorkOrder, 0, 1);
            layoutSearch.Controls.Add(btnSearch, 1, 1);
            layoutSearch.Dock = DockStyle.Top;
            layoutSearch.Location = new Point(24, 12);
            layoutSearch.Margin = new Padding(0);
            layoutSearch.Name = "layoutSearch";
            layoutSearch.RowCount = 2;
            layoutSearch.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            layoutSearch.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            layoutSearch.Size = new Size(932, 63);
            // lblWorkOrder
            lblWorkOrder.AutoSize = true;
            lblWorkOrder.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblWorkOrder.ForeColor = Color.FromArgb(70, 80, 90);
            lblWorkOrder.Location = new Point(3, 0);
            lblWorkOrder.Name = "lblWorkOrder";
            lblWorkOrder.Text = "WORK ORDER";
            // txtWorkOrder
            txtWorkOrder.BorderStyle = BorderStyle.FixedSingle;
            txtWorkOrder.CharacterCasing = CharacterCasing.Upper;
            txtWorkOrder.Dock = DockStyle.Fill;
            txtWorkOrder.Font = new Font("Segoe UI", 11F);
            txtWorkOrder.Location = new Point(0, 25);
            txtWorkOrder.Margin = new Padding(0, 0, 12, 5);
            txtWorkOrder.Name = "txtWorkOrder";
            txtWorkOrder.PlaceholderText = "Digite ou escaneie a ordem, com ou sem a letra O";
            txtWorkOrder.KeyPress += TxtWorkOrder_KeyPress;
            // btnSearch
            btnSearch.Dock = DockStyle.Fill;
            btnSearch.Location = new Point(792, 25);
            btnSearch.Margin = new Padding(0);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(140, 38);
            btnSearch.TabIndex = 1;
            btnSearch.Text = "PESQUISAR";
            btnSearch.Click += BtnSearch_Click;
            // lblResult
            lblResult.AutoSize = true;
            lblResult.ForeColor = Color.FromArgb(80, 90, 100);
            lblResult.Location = new Point(24, 82);
            lblResult.Name = "lblResult";
            lblResult.Text = "Informe uma Work Order para iniciar a consulta.";
            // listBoxes
            listBoxes.BackColor = Color.White;
            listBoxes.BorderStyle = BorderStyle.None;
            listBoxes.Columns.AddRange(new ColumnHeader[] { colHu, colBatch, colQuantity, colPartNumber, colOperator });
            listBoxes.Dock = DockStyle.Fill;
            listBoxes.Font = new Font("Segoe UI", 10F);
            listBoxes.FullRowSelect = true;
            listBoxes.GridLines = true;
            listBoxes.HideSelection = false;
            listBoxes.Location = new Point(0, 200);
            listBoxes.Margin = new Padding(0);
            listBoxes.MultiSelect = false;
            listBoxes.Name = "listBoxes";
            listBoxes.UseCompatibleStateImageBehavior = false;
            listBoxes.View = View.Details;
            listBoxes.SelectedIndexChanged += ListBoxes_SelectedIndexChanged;
            listBoxes.DoubleClick += ListBoxes_DoubleClick;
            // columns
            colHu.Text = "NÚMERO ÚNICO (HU)";
            colHu.Width = 210;
            colBatch.Text = "LOTE";
            colBatch.Width = 180;
            colQuantity.Text = "COMPONENTES";
            colQuantity.Width = 140;
            colPartNumber.Text = "PARTNUMBER FINAL";
            colPartNumber.Width = 190;
            colOperator.Text = "USUÁRIO DA CAIXA";
            colOperator.Width = 240;
            // pnlFooter
            pnlFooter.BackColor = Color.White;
            pnlFooter.Controls.Add(lblHint);
            pnlFooter.Controls.Add(btnOpenBox);
            pnlFooter.Dock = DockStyle.Fill;
            pnlFooter.Margin = new Padding(0);
            pnlFooter.Name = "pnlFooter";
            // lblHint
            lblHint.AutoSize = true;
            lblHint.ForeColor = Color.FromArgb(80, 90, 100);
            lblHint.Location = new Point(24, 24);
            lblHint.Name = "lblHint";
            lblHint.Text = "Selecione uma linha e clique em abrir, ou dê um duplo clique.";
            // btnOpenBox
            btnOpenBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnOpenBox.Enabled = false;
            btnOpenBox.Location = new Point(778, 14);
            btnOpenBox.Name = "btnOpenBox";
            btnOpenBox.Size = new Size(178, 40);
            btnOpenBox.TabIndex = 3;
            btnOpenBox.Text = "ABRIR COMPONENTES";
            btnOpenBox.Click += BtnOpenBox_Click;
            // WorkOrderFinishedBoxesDialog
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 590);
            Controls.Add(layoutRoot);
            MinimumSize = new Size(850, 500);
            Name = "WorkOrderFinishedBoxesDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Consultar caixas finalizadas";
            layoutRoot.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlSearch.ResumeLayout(false);
            pnlSearch.PerformLayout();
            layoutSearch.ResumeLayout(false);
            layoutSearch.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
            ResumeLayout(false);
        }
    }
}
