namespace HondaSensorChecker
{
    partial class ComponentHistoryDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            layout = new TableLayoutPanel();
            _txtSerial = new TextBox();
            _btnSearch = new Button();
            _listView = new ListView();
            columnSerial = new ColumnHeader();
            columnDate = new ColumnHeader();
            columnWorkOrder = new ColumnHeader();
            columnSupplier = new ColumnHeader();
            columnZfBox = new ColumnHeader();
            columnStatus = new ColumnHeader();
            layout.SuspendLayout();
            SuspendLayout();
            // 
            // layout
            // 
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            layout.Controls.Add(_txtSerial, 0, 0);
            layout.Controls.Add(_btnSearch, 1, 0);
            layout.Controls.Add(_listView, 0, 1);
            layout.Dock = DockStyle.Fill;
            layout.Location = new Point(0, 0);
            layout.Name = "layout";
            layout.RowCount = 2;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.SetColumnSpan(_listView, 2);
            layout.Size = new Size(720, 360);
            layout.TabIndex = 0;
            // 
            // _txtSerial
            // 
            _txtSerial.Dock = DockStyle.Fill;
            _txtSerial.Font = new Font("Segoe UI", 12F);
            _txtSerial.Location = new Point(3, 3);
            _txtSerial.Name = "_txtSerial";
            _txtSerial.PlaceholderText = "Digite o serial do sensor";
            _txtSerial.Size = new Size(498, 29);
            _txtSerial.TabIndex = 0;
            _txtSerial.KeyPress += TxtSerial_KeyPress;
            // 
            // _btnSearch
            // 
            _btnSearch.Dock = DockStyle.Fill;
            _btnSearch.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _btnSearch.Location = new Point(507, 3);
            _btnSearch.Name = "_btnSearch";
            _btnSearch.Size = new Size(210, 34);
            _btnSearch.TabIndex = 1;
            _btnSearch.Text = "Pesquisar";
            _btnSearch.UseVisualStyleBackColor = true;
            _btnSearch.Click += BtnSearch_Click;
            // 
            // _listView
            // 
            _listView.Columns.AddRange(new ColumnHeader[] { columnSerial, columnDate, columnWorkOrder, columnSupplier, columnZfBox, columnStatus });
            _listView.Dock = DockStyle.Fill;
            _listView.FullRowSelect = true;
            _listView.Location = new Point(3, 43);
            _listView.Name = "_listView";
            _listView.Size = new Size(714, 314);
            _listView.TabIndex = 2;
            _listView.UseCompatibleStateImageBehavior = false;
            _listView.View = View.Details;
            // 
            // columnSerial
            // 
            columnSerial.Text = "Serial";
            columnSerial.Width = 120;
            // 
            // columnDate
            // 
            columnDate.Text = "Data/Hora";
            columnDate.Width = 150;
            // 
            // columnWorkOrder
            // 
            columnWorkOrder.Text = "WorkOrder";
            columnWorkOrder.Width = 120;
            // 
            // columnSupplier
            // 
            columnSupplier.Text = "SupplierBox";
            columnSupplier.Width = 120;
            // 
            // columnZfBox
            // 
            columnZfBox.Text = "ZfBox";
            columnZfBox.Width = 120;
            // 
            // columnStatus
            // 
            columnStatus.Text = "Status";
            columnStatus.Width = 80;
            // 
            // ComponentHistoryDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(720, 360);
            Controls.Add(layout);
            Name = "ComponentHistoryDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Consultar componente";
            layout.ResumeLayout(false);
            layout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel layout;
        private TextBox _txtSerial;
        private Button _btnSearch;
        private ListView _listView;
        private ColumnHeader columnSerial;
        private ColumnHeader columnDate;
        private ColumnHeader columnWorkOrder;
        private ColumnHeader columnSupplier;
        private ColumnHeader columnZfBox;
        private ColumnHeader columnStatus;
    }
}
