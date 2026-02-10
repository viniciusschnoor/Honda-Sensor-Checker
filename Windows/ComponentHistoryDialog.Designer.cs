using System.Drawing;
using System.Windows.Forms;

namespace HondaSensorChecker
{
    partial class ComponentHistoryDialog
    {
        private System.ComponentModel.IContainer components = null;

        private TableLayoutPanel layout;
        private TextBox txtSerial;
        private Button btnSearch;
        private ListView listView;
        private ColumnHeader colSerial;
        private ColumnHeader colDataHora;
        private ColumnHeader colWorkOrder;
        private ColumnHeader colSupplierBox;
        private ColumnHeader colZfBox;
        private ColumnHeader colStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            layout = new TableLayoutPanel();
            txtSerial = new TextBox();
            btnSearch = new Button();
            listView = new ListView();
            colSerial = new ColumnHeader();
            colDataHora = new ColumnHeader();
            colWorkOrder = new ColumnHeader();
            colSupplierBox = new ColumnHeader();
            colZfBox = new ColumnHeader();
            colStatus = new ColumnHeader();
            layout.SuspendLayout();
            SuspendLayout();
            // 
            // layout
            // 
            layout.ColumnCount = 2;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            layout.Controls.Add(txtSerial, 0, 0);
            layout.Controls.Add(btnSearch, 1, 0);
            layout.Controls.Add(listView, 0, 1);
            layout.Dock = DockStyle.Fill;
            layout.Location = new Point(0, 0);
            layout.Name = "layout";
            layout.RowCount = 2;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.Size = new Size(720, 360);
            layout.TabIndex = 0;
            // 
            // txtSerial
            // 
            txtSerial.Dock = DockStyle.Fill;
            txtSerial.Font = new Font("Segoe UI", 12F);
            txtSerial.Location = new Point(3, 3);
            txtSerial.Name = "txtSerial";
            txtSerial.PlaceholderText = "Digite o serial do sensor";
            txtSerial.Size = new Size(498, 29);
            txtSerial.TabIndex = 0;
            txtSerial.KeyPress += TxtSerial_KeyPress;
            // 
            // btnSearch
            // 
            btnSearch.Dock = DockStyle.Fill;
            btnSearch.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSearch.Location = new Point(507, 3);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(210, 34);
            btnSearch.TabIndex = 1;
            btnSearch.Text = "Pesquisar";
            btnSearch.Click += BtnSearch_Click;
            // 
            // listView
            // 
            listView.Columns.AddRange(new ColumnHeader[] { colSerial, colDataHora, colWorkOrder, colSupplierBox, colZfBox, colStatus });
            layout.SetColumnSpan(listView, 2);
            listView.Dock = DockStyle.Fill;
            listView.FullRowSelect = true;
            listView.Location = new Point(3, 43);
            listView.Name = "listView";
            listView.Size = new Size(714, 314);
            listView.TabIndex = 2;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            // 
            // colSerial
            // 
            colSerial.Text = "Serial";
            colSerial.Width = 120;
            // 
            // colDataHora
            // 
            colDataHora.Text = "Data/Hora";
            colDataHora.Width = 150;
            // 
            // colWorkOrder
            // 
            colWorkOrder.Text = "WorkOrder";
            colWorkOrder.Width = 120;
            // 
            // colSupplierBox
            // 
            colSupplierBox.Text = "SupplierBox";
            colSupplierBox.Width = 120;
            // 
            // colZfBox
            // 
            colZfBox.Text = "ZfBox";
            colZfBox.Width = 120;
            // 
            // colStatus
            // 
            colStatus.Text = "Status";
            colStatus.Width = 80;
            // 
            // ComponentHistoryDialog
            // 
            ClientSize = new Size(720, 360);
            Controls.Add(layout);
            Name = "ComponentHistoryDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Consultar componente";
            layout.ResumeLayout(false);
            layout.PerformLayout();
            ResumeLayout(false);
        }
    }
}