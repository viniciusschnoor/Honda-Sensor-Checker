using HondaSensorChecker.Data.UnitOfWork;
using System;
using System.Linq;
using System.Windows.Forms;

namespace HondaSensorChecker
{
    public partial class ComponentHistoryDialog : Form
    {
        private readonly IUnitOfWork _unitOfWork;

        public ComponentHistoryDialog(IUnitOfWork unitOfWork)
        {
            InitializeComponent();
            _unitOfWork = unitOfWork;
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
