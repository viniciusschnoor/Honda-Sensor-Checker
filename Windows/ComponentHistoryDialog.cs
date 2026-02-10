using HondaSensorChecker.Data.UnitOfWork;
using System;
using System.Linq;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HondaSensorChecker
{
    public partial class ComponentHistoryDialog : Form
    {
<<<<<<< HEAD
        private readonly IUnitOfWork? _unitOfWork;
=======
        private readonly IUnitOfWork _unitOfWork;
>>>>>>> 4d73f24c0c9211149f55320542b8b65572e153e0

        // ✅ Necessário para o Designer
        public ComponentHistoryDialog()
        {
            InitializeComponent();
<<<<<<< HEAD
        }

        // ✅ Usado em runtime
        public ComponentHistoryDialog(IUnitOfWork unitOfWork) : this()
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
=======
            _unitOfWork = unitOfWork;
>>>>>>> 4d73f24c0c9211149f55320542b8b65572e153e0
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
            if (_unitOfWork is null)
            {
                // No designer (ou se alguém abrir sem DI) não quebra.
                MessageBox.Show(
                    "UnitOfWork não foi informado. Abra este diálogo passando IUnitOfWork.",
                    "Consultar componente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var serial = (txtSerial.Text ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(serial))
                return;

            listView.Items.Clear();

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

            listView.Items.Add(item);
        }
    }
}