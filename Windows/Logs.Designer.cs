namespace HondaSensorChecker
{
    partial class Logs
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblCount;
        private Panel pnlFilters;
        private Label lblSearch;
        private TextBox txtSearch;
        private Label lblOperator;
        private ComboBox cboOperator;
        private Label lblPeriod;
        private ComboBox cboPeriod;
        private Button btnClearFilters;
        private DataGridView dgvLogs;
        private DataGridViewTextBoxColumn colData;
        private DataGridViewTextBoxColumn colOperator;
        private DataGridViewTextBoxColumn colDescription;
        private Panel pnlDetails;
        private Label lblDetailsTitle;
        private Label lblDetailsDate;
        private Label lblDetailsOperator;
        private TextBox txtDetails;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            lblCount = new Label();
            lblSubtitle = new Label();
            lblTitle = new Label();
            pnlFilters = new Panel();
            btnClearFilters = new Button();
            cboPeriod = new ComboBox();
            lblPeriod = new Label();
            cboOperator = new ComboBox();
            lblOperator = new Label();
            txtSearch = new TextBox();
            lblSearch = new Label();
            dgvLogs = new DataGridView();
            colData = new DataGridViewTextBoxColumn();
            colOperator = new DataGridViewTextBoxColumn();
            colDescription = new DataGridViewTextBoxColumn();
            pnlDetails = new Panel();
            txtDetails = new TextBox();
            lblDetailsOperator = new Label();
            lblDetailsDate = new Label();
            lblDetailsTitle = new Label();
            pnlHeader.SuspendLayout();
            pnlFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLogs).BeginInit();
            pnlDetails.SuspendLayout();
            SuspendLayout();
            // pnlHeader
            pnlHeader.BackColor = Color.FromArgb(21, 74, 124);
            pnlHeader.Controls.Add(lblCount);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1000, 88);
            pnlHeader.TabIndex = 0;
            // lblCount
            lblCount.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCount.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblCount.ForeColor = Color.White;
            lblCount.Location = new Point(741, 31);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(235, 26);
            lblCount.TabIndex = 2;
            lblCount.Text = "0 registros";
            lblCount.TextAlign = ContentAlignment.MiddleRight;
            // lblSubtitle
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = Color.FromArgb(218, 232, 245);
            lblSubtitle.Location = new Point(27, 54);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(294, 15);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Histórico de eventos e intervenções realizadas no sistema";
            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(22, 14);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(242, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Histórico de logs";
            // pnlFilters
            pnlFilters.BackColor = Color.White;
            pnlFilters.Controls.Add(btnClearFilters);
            pnlFilters.Controls.Add(cboPeriod);
            pnlFilters.Controls.Add(lblPeriod);
            pnlFilters.Controls.Add(cboOperator);
            pnlFilters.Controls.Add(lblOperator);
            pnlFilters.Controls.Add(txtSearch);
            pnlFilters.Controls.Add(lblSearch);
            pnlFilters.Dock = DockStyle.Top;
            pnlFilters.Location = new Point(0, 88);
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Size = new Size(1000, 75);
            pnlFilters.TabIndex = 1;
            // btnClearFilters
            btnClearFilters.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClearFilters.BackColor = Color.White;
            btnClearFilters.FlatAppearance.BorderColor = Color.FromArgb(185, 194, 204);
            btnClearFilters.FlatStyle = FlatStyle.Flat;
            btnClearFilters.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnClearFilters.ForeColor = Color.FromArgb(50, 61, 72);
            btnClearFilters.Location = new Point(852, 31);
            btnClearFilters.Name = "btnClearFilters";
            btnClearFilters.Size = new Size(124, 30);
            btnClearFilters.TabIndex = 6;
            btnClearFilters.Text = "LIMPAR FILTROS";
            btnClearFilters.UseVisualStyleBackColor = false;
            btnClearFilters.Click += btnClearFilters_Click;
            // cboPeriod
            cboPeriod.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cboPeriod.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPeriod.FormattingEnabled = true;
            cboPeriod.Location = new Point(674, 34);
            cboPeriod.Name = "cboPeriod";
            cboPeriod.Size = new Size(164, 23);
            cboPeriod.TabIndex = 5;
            cboPeriod.SelectedIndexChanged += FilterChanged;
            // lblPeriod
            lblPeriod.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPeriod.AutoSize = true;
            lblPeriod.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblPeriod.ForeColor = Color.FromArgb(70, 80, 90);
            lblPeriod.Location = new Point(674, 13);
            lblPeriod.Name = "lblPeriod";
            lblPeriod.Size = new Size(48, 15);
            lblPeriod.TabIndex = 4;
            lblPeriod.Text = "Período";
            // cboOperator
            cboOperator.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cboOperator.DropDownStyle = ComboBoxStyle.DropDownList;
            cboOperator.FormattingEnabled = true;
            cboOperator.Location = new Point(470, 34);
            cboOperator.Name = "cboOperator";
            cboOperator.Size = new Size(190, 23);
            cboOperator.TabIndex = 3;
            cboOperator.SelectedIndexChanged += FilterChanged;
            // lblOperator
            lblOperator.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblOperator.AutoSize = true;
            lblOperator.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblOperator.ForeColor = Color.FromArgb(70, 80, 90);
            lblOperator.Location = new Point(470, 13);
            lblOperator.Name = "lblOperator";
            lblOperator.Size = new Size(59, 15);
            lblOperator.TabIndex = 2;
            lblOperator.Text = "Operador";
            // txtSearch
            txtSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.Location = new Point(24, 34);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Pesquisar na descrição ou operador...";
            txtSearch.Size = new Size(430, 23);
            txtSearch.TabIndex = 1;
            txtSearch.TextChanged += FilterChanged;
            // lblSearch
            lblSearch.AutoSize = true;
            lblSearch.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblSearch.ForeColor = Color.FromArgb(70, 80, 90);
            lblSearch.Location = new Point(24, 13);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(55, 15);
            lblSearch.TabIndex = 0;
            lblSearch.Text = "Pesquisa";
            // dgvLogs
            dgvLogs.AllowUserToAddRows = false;
            dgvLogs.AllowUserToDeleteRows = false;
            dgvLogs.AllowUserToResizeRows = false;
            dgvLogs.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(247, 249, 252) };
            dgvLogs.BackgroundColor = Color.White;
            dgvLogs.BorderStyle = BorderStyle.None;
            dgvLogs.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvLogs.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvLogs.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft, BackColor = Color.FromArgb(235, 240, 246), Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(45, 58, 70), SelectionBackColor = Color.FromArgb(235, 240, 246), SelectionForeColor = Color.FromArgb(45, 58, 70) };
            dgvLogs.ColumnHeadersHeight = 40;
            dgvLogs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvLogs.Columns.AddRange(new DataGridViewColumn[] { colData, colOperator, colDescription });
            dgvLogs.DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.White, ForeColor = Color.FromArgb(38, 48, 58), Padding = new Padding(7, 0, 7, 0), SelectionBackColor = Color.FromArgb(215, 231, 247), SelectionForeColor = Color.FromArgb(25, 55, 82) };
            dgvLogs.Dock = DockStyle.Fill;
            dgvLogs.EnableHeadersVisualStyles = false;
            dgvLogs.GridColor = Color.FromArgb(225, 230, 235);
            dgvLogs.Location = new Point(0, 163);
            dgvLogs.MultiSelect = false;
            dgvLogs.Name = "dgvLogs";
            dgvLogs.ReadOnly = true;
            dgvLogs.RowHeadersVisible = false;
            dgvLogs.RowTemplate.Height = 36;
            dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLogs.Size = new Size(1000, 347);
            dgvLogs.TabIndex = 2;
            dgvLogs.SelectionChanged += dgvLogs_SelectionChanged;
            // columns
            colData.DataPropertyName = "Data";
            colData.DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" };
            colData.HeaderText = "DATA E HORA";
            colData.MinimumWidth = 155;
            colData.Name = "colData";
            colData.ReadOnly = true;
            colData.Width = 165;
            colOperator.DataPropertyName = "Operator";
            colOperator.HeaderText = "OPERADOR";
            colOperator.MinimumWidth = 150;
            colOperator.Name = "colOperator";
            colOperator.ReadOnly = true;
            colOperator.Width = 190;
            colDescription.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colDescription.DataPropertyName = "Description";
            colDescription.HeaderText = "DESCRIÇÃO";
            colDescription.MinimumWidth = 300;
            colDescription.Name = "colDescription";
            colDescription.ReadOnly = true;
            // pnlDetails
            pnlDetails.BackColor = Color.FromArgb(247, 249, 252);
            pnlDetails.Controls.Add(txtDetails);
            pnlDetails.Controls.Add(lblDetailsOperator);
            pnlDetails.Controls.Add(lblDetailsDate);
            pnlDetails.Controls.Add(lblDetailsTitle);
            pnlDetails.Dock = DockStyle.Bottom;
            pnlDetails.Location = new Point(0, 510);
            pnlDetails.Name = "pnlDetails";
            pnlDetails.Size = new Size(1000, 130);
            pnlDetails.TabIndex = 3;
            // txtDetails
            txtDetails.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtDetails.BackColor = Color.White;
            txtDetails.BorderStyle = BorderStyle.FixedSingle;
            txtDetails.Location = new Point(24, 65);
            txtDetails.Multiline = true;
            txtDetails.Name = "txtDetails";
            txtDetails.ReadOnly = true;
            txtDetails.ScrollBars = ScrollBars.Vertical;
            txtDetails.Size = new Size(952, 49);
            txtDetails.TabIndex = 3;
            // detail labels
            lblDetailsOperator.AutoSize = true;
            lblDetailsOperator.ForeColor = Color.FromArgb(80, 90, 100);
            lblDetailsOperator.Location = new Point(235, 40);
            lblDetailsOperator.Name = "lblDetailsOperator";
            lblDetailsOperator.Size = new Size(64, 15);
            lblDetailsOperator.TabIndex = 2;
            lblDetailsOperator.Text = "Operador: —";
            lblDetailsDate.AutoSize = true;
            lblDetailsDate.ForeColor = Color.FromArgb(80, 90, 100);
            lblDetailsDate.Location = new Point(24, 40);
            lblDetailsDate.Name = "lblDetailsDate";
            lblDetailsDate.Size = new Size(45, 15);
            lblDetailsDate.TabIndex = 1;
            lblDetailsDate.Text = "Data: —";
            lblDetailsTitle.AutoSize = true;
            lblDetailsTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblDetailsTitle.ForeColor = Color.FromArgb(45, 58, 70);
            lblDetailsTitle.Location = new Point(24, 13);
            lblDetailsTitle.Name = "lblDetailsTitle";
            lblDetailsTitle.Size = new Size(135, 19);
            lblDetailsTitle.TabIndex = 0;
            lblDetailsTitle.Text = "Detalhes do registro";
            // Logs
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1000, 640);
            Controls.Add(dgvLogs);
            Controls.Add(pnlDetails);
            Controls.Add(pnlFilters);
            Controls.Add(pnlHeader);
            Font = new Font("Segoe UI", 9F);
            MinimumSize = new Size(820, 560);
            Name = "Logs";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Histórico de Logs";
            Load += Logs_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlFilters.ResumeLayout(false);
            pnlFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLogs).EndInit();
            pnlDetails.ResumeLayout(false);
            pnlDetails.PerformLayout();
            ResumeLayout(false);
        }
    }
}
