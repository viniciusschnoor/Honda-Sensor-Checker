using HondaSensorChecker.Data.UnitOfWork;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HondaSensorChecker
{
    public class ComponentHistoryDialog : Form
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TextBox _txtSerial;
        private readonly Button _btnSearch;
        private readonly ListView _listView;

        public ComponentHistoryDialog(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            Text = "Consultar componente";
            Size = new Size(720, 360);
            StartPosition = FormStartPosition.CenterParent;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            _txtSerial = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F),
                PlaceholderText = "Digite o serial do sensor"
            };
            _txtSerial.KeyPress += TxtSerial_KeyPress;

            _btnSearch = new Button
            {
                Dock = DockStyle.Fill,
                Text = "Pesquisar",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            _btnSearch.Click += BtnSearch_Click;

            _listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true
            };
            _listView.Columns.Add("Serial", 120);
            _listView.Columns.Add("Data/Hora", 150);
            _listView.Columns.Add("WorkOrder", 120);
            _listView.Columns.Add("SupplierBox", 120);
            _listView.Columns.Add("ZfBox", 120);
            _listView.Columns.Add("Status", 80);

            layout.Controls.Add(_txtSerial, 0, 0);
            layout.Controls.Add(_btnSearch, 1, 0);
            layout.Controls.Add(_listView, 0, 1);
            layout.SetColumnSpan(_listView, 2);

            Controls.Add(layout);
        }

        private void TxtSerial_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                BuscarHistorico();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            BuscarHistorico();
        }

        private void BuscarHistorico()
        {
            var serial = (_txtSerial.Text ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(serial))
                return;

            _listView.Items.Clear();

            var sensor = _unitOfWork.Sensors.Find(s => s.SerialNumber == serial).FirstOrDefault();
            if (sensor == null)
            {
                MessageBox.Show("Sensor não encontrado.", "Consultar componente",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var workOrder = _unitOfWork.SapWorkOrders.GetById(sensor.SapWorkOrderId);
            var supplierBox = _unitOfWork.SupplierBoxes.GetById(sensor.SupplierBoxId);
            var zfBox = _unitOfWork.ZfBoxes.GetById(sensor.ZfBoxId);

            var workOrderNumber = workOrder?.WorkOrderNumber ?? "N/D";
            var supplierNumber = supplierBox?.UniqueNumber ?? "N/D";
            var zfNumber = string.IsNullOrWhiteSpace(zfBox?.UniqueNumber) ? "(em andamento)" : zfBox.UniqueNumber;
            var status = sensor.InProgress ? "Em andamento" : "Finalizado";

            var item = new ListViewItem(new[]
            {
                sensor.SerialNumber,
                sensor.ScannedTime.ToString("dd/MM/yyyy HH:mm:ss"),
                workOrderNumber,
                supplierNumber,
                zfNumber,
                status
            });

            _listView.Items.Add(item);
        }
    }
}
