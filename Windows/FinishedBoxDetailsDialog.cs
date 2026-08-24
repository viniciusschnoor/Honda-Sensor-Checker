using HondaSensorChecker.Data.UnitOfWork;

namespace HondaSensorChecker
{
    public partial class FinishedBoxDetailsDialog : Form
    {
        private readonly IUnitOfWork? _unitOfWork;
        private readonly int _zfBoxId;

        public FinishedBoxDetailsDialog()
        {
            InitializeComponent();
            UiTheme.StyleForm(this);
        }

        public FinishedBoxDetailsDialog(IUnitOfWork unitOfWork, int zfBoxId) : this()
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _zfBoxId = zfBoxId;
            LoadBoxDetails();
        }

        private void LoadBoxDetails()
        {
            if (_unitOfWork is null || _zfBoxId <= 0)
                return;

            var box = _unitOfWork.ZfBoxes.GetById(_zfBoxId);
            if (box is null)
            {
                MessageBox.Show("Caixa não encontrada.", "Componentes da caixa",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                return;
            }

            var workOrder = _unitOfWork.SapWorkOrders.GetById(box.SapWorkOrderId);
            var sensors = _unitOfWork.Sensors
                .Find(sensor => sensor.ZfBoxId == _zfBoxId)
                .OrderBy(sensor => sensor.ScannedTime)
                .ToList();

            lblHuValue.Text = WorkOrderFinishedBoxesDialog.FormatHu(box.UniqueNumber);
            lblWorkOrderValue.Text = workOrder is null ? "N/D" : $"O{workOrder.WorkOrderNumber}";
            lblBatchValue.Text = string.IsNullOrWhiteSpace(box.Batch) ? "N/D" : $"H{box.Batch}";
            lblQuantityValue.Text = sensors.Count.ToString();

            listSensors.Items.Clear();
            foreach (var sensor in sensors)
            {
                var scanOperator = _unitOfWork.Operators.GetById(sensor.OperatorId);
                var supplierBox = _unitOfWork.SupplierBoxes.GetById(sensor.SupplierBoxId);
                var operatorName = scanOperator?.Name ?? scanOperator?.Re ?? scanOperator?.ZfId ?? "N/D";
                var supplierNumber = supplierBox?.UniqueNumber ?? "N/D";

                listSensors.Items.Add(new ListViewItem(new[]
                {
                    sensor.SerialNumber,
                    sensor.ScannedTime.ToString("dd/MM/yyyy HH:mm:ss"),
                    operatorName,
                    supplierNumber,
                    sensor.InProgress ? "Em andamento" : "Finalizado"
                }));
            }
        }
    }
}
