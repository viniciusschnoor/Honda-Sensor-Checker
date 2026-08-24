namespace HondaSensorChecker
{
    partial class FinishedBoxDetailsDialog
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutRoot;
        private Panel pnlHeader;
        private TableLayoutPanel layoutSummary;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblHu;
        private Label lblHuValue;
        private Label lblWorkOrder;
        private Label lblWorkOrderValue;
        private Label lblBatch;
        private Label lblBatchValue;
        private Label lblQuantity;
        private Label lblQuantityValue;
        private ListView listSensors;
        private ColumnHeader colSerial;
        private ColumnHeader colScannedTime;
        private ColumnHeader colOperator;
        private ColumnHeader colSupplierBox;
        private ColumnHeader colStatus;

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
            layoutSummary = new TableLayoutPanel();
            lblHu = new Label();
            lblHuValue = new Label();
            lblWorkOrder = new Label();
            lblWorkOrderValue = new Label();
            lblBatch = new Label();
            lblBatchValue = new Label();
            lblQuantity = new Label();
            lblQuantityValue = new Label();
            listSensors = new ListView();
            colSerial = new ColumnHeader();
            colScannedTime = new ColumnHeader();
            colOperator = new ColumnHeader();
            colSupplierBox = new ColumnHeader();
            colStatus = new ColumnHeader();
            layoutRoot.SuspendLayout();
            pnlHeader.SuspendLayout();
            layoutSummary.SuspendLayout();
            SuspendLayout();
            // layoutRoot
            layoutRoot.ColumnCount = 1;
            layoutRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutRoot.Controls.Add(pnlHeader, 0, 0);
            layoutRoot.Controls.Add(layoutSummary, 0, 1);
            layoutRoot.Controls.Add(listSensors, 0, 2);
            layoutRoot.Dock = DockStyle.Fill;
            layoutRoot.Location = new Point(0, 0);
            layoutRoot.Margin = new Padding(0);
            layoutRoot.Name = "layoutRoot";
            layoutRoot.RowCount = 3;
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 88F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Absolute, 92F));
            layoutRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutRoot.Size = new Size(960, 540);
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
            lblTitle.Text = "Componentes da caixa";
            // lblSubtitle
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = Color.FromArgb(218, 232, 245);
            lblSubtitle.Location = new Point(27, 55);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Text = "Rastreabilidade completa dos sensores da HU finalizada";
            // layoutSummary
            layoutSummary.BackColor = Color.White;
            layoutSummary.ColumnCount = 4;
            layoutSummary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layoutSummary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layoutSummary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layoutSummary.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            layoutSummary.Controls.Add(lblHu, 0, 0);
            layoutSummary.Controls.Add(lblWorkOrder, 1, 0);
            layoutSummary.Controls.Add(lblBatch, 2, 0);
            layoutSummary.Controls.Add(lblQuantity, 3, 0);
            layoutSummary.Controls.Add(lblHuValue, 0, 1);
            layoutSummary.Controls.Add(lblWorkOrderValue, 1, 1);
            layoutSummary.Controls.Add(lblBatchValue, 2, 1);
            layoutSummary.Controls.Add(lblQuantityValue, 3, 1);
            layoutSummary.Dock = DockStyle.Fill;
            layoutSummary.Margin = new Padding(0);
            layoutSummary.Padding = new Padding(24, 12, 24, 12);
            layoutSummary.RowCount = 2;
            layoutSummary.RowStyles.Add(new RowStyle(SizeType.Absolute, 25F));
            layoutSummary.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            // summary labels
            lblHu.AutoSize = true;
            lblHu.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblHu.ForeColor = Color.FromArgb(80, 90, 100);
            lblHu.Text = "NÚMERO ÚNICO (HU)";
            lblHuValue.AutoSize = true;
            lblHuValue.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblHuValue.Text = "N/D";
            lblWorkOrder.AutoSize = true;
            lblWorkOrder.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblWorkOrder.ForeColor = Color.FromArgb(80, 90, 100);
            lblWorkOrder.Text = "WORK ORDER";
            lblWorkOrderValue.AutoSize = true;
            lblWorkOrderValue.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblWorkOrderValue.Text = "N/D";
            lblBatch.AutoSize = true;
            lblBatch.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblBatch.ForeColor = Color.FromArgb(80, 90, 100);
            lblBatch.Text = "LOTE";
            lblBatchValue.AutoSize = true;
            lblBatchValue.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblBatchValue.Text = "N/D";
            lblQuantity.AutoSize = true;
            lblQuantity.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblQuantity.ForeColor = Color.FromArgb(80, 90, 100);
            lblQuantity.Text = "COMPONENTES";
            lblQuantityValue.AutoSize = true;
            lblQuantityValue.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            lblQuantityValue.Text = "0";
            // listSensors
            listSensors.BackColor = Color.White;
            listSensors.BorderStyle = BorderStyle.None;
            listSensors.Columns.AddRange(new ColumnHeader[] { colSerial, colScannedTime, colOperator, colSupplierBox, colStatus });
            listSensors.Dock = DockStyle.Fill;
            listSensors.Font = new Font("Segoe UI", 10F);
            listSensors.FullRowSelect = true;
            listSensors.GridLines = true;
            listSensors.HideSelection = false;
            listSensors.Location = new Point(0, 180);
            listSensors.Margin = new Padding(0);
            listSensors.Name = "listSensors";
            listSensors.UseCompatibleStateImageBehavior = false;
            listSensors.View = View.Details;
            // columns
            colSerial.Text = "SERIAL DO SENSOR";
            colSerial.Width = 190;
            colScannedTime.Text = "DATA E HORA";
            colScannedTime.Width = 175;
            colOperator.Text = "USUÁRIO DO SCAN";
            colOperator.Width = 230;
            colSupplierBox.Text = "SUPPLIER BOX";
            colSupplierBox.Width = 190;
            colStatus.Text = "STATUS";
            colStatus.Width = 130;
            // FinishedBoxDetailsDialog
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(960, 540);
            Controls.Add(layoutRoot);
            MinimumSize = new Size(820, 460);
            Name = "FinishedBoxDetailsDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Componentes da caixa finalizada";
            layoutRoot.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            layoutSummary.ResumeLayout(false);
            layoutSummary.PerformLayout();
            ResumeLayout(false);
        }
    }
}
