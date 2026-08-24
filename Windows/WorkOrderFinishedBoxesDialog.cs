using HondaSensorChecker.Data.UnitOfWork;

namespace HondaSensorChecker
{
    public partial class WorkOrderFinishedBoxesDialog : Form
    {
        private readonly IUnitOfWork? _unitOfWork;

        public WorkOrderFinishedBoxesDialog()
        {
            InitializeComponent();
            UiTheme.StyleForm(this);
            UiTheme.StylePrimaryButton(btnSearch);
            UiTheme.StylePrimaryButton(btnOpenBox);
        }

        public WorkOrderFinishedBoxesDialog(IUnitOfWork unitOfWork) : this()
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        private void BtnSearch_Click(object sender, EventArgs e) => SearchFinishedBoxes();

        private void TxtWorkOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                SearchFinishedBoxes();
            }
        }

        private void SearchFinishedBoxes()
        {
            if (_unitOfWork is null)
                return;

            var workOrderNumber = NormalizeWorkOrder(txtWorkOrder.Text);
            if (string.IsNullOrWhiteSpace(workOrderNumber))
            {
                MessageBox.Show("Informe a Work Order.", "Consultar caixas finalizadas",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtWorkOrder.Focus();
                return;
            }

            listBoxes.Items.Clear();
            btnOpenBox.Enabled = false;

            var workOrder = _unitOfWork.SapWorkOrders
                .Find(item => item.WorkOrderNumber == workOrderNumber)
                .FirstOrDefault();

            if (workOrder is null)
            {
                lblResult.Text = "Work Order não encontrada.";
                return;
            }

            var boxes = _unitOfWork.ZfBoxes
                .Find(box => box.SapWorkOrderId == workOrder.SapWorkOrderId && !box.InProgress)
                .OrderByDescending(box => box.ZfBoxId)
                .ToList();

            foreach (var box in boxes)
            {
                var sensorCount = _unitOfWork.Sensors.Find(sensor => sensor.ZfBoxId == box.ZfBoxId).Count();
                var product = _unitOfWork.Products.GetById(box.ProductId);
                var boxOperator = _unitOfWork.Operators.GetById(box.OperatorId);
                var operatorName = boxOperator?.Name ?? boxOperator?.Re ?? boxOperator?.ZfId ?? "N/D";

                var item = new ListViewItem(new[]
                {
                    FormatHu(box.UniqueNumber),
                    string.IsNullOrWhiteSpace(box.Batch) ? "N/D" : $"H{box.Batch}",
                    sensorCount.ToString(),
                    product?.EndPartNumber ?? "N/D",
                    operatorName
                })
                {
                    Tag = box.ZfBoxId
                };

                listBoxes.Items.Add(item);
            }

            lblResult.Text = boxes.Count == 0
                ? "Nenhuma caixa finalizada foi encontrada para esta Work Order."
                : $"{boxes.Count} caixa(s) finalizada(s) encontrada(s).";
        }

        private void ListBoxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOpenBox.Enabled = listBoxes.SelectedItems.Count == 1;
        }

        private void ListBoxes_DoubleClick(object sender, EventArgs e) => OpenSelectedBox();

        private void BtnOpenBox_Click(object sender, EventArgs e) => OpenSelectedBox();

        private void OpenSelectedBox()
        {
            if (_unitOfWork is null || listBoxes.SelectedItems.Count != 1)
                return;

            if (listBoxes.SelectedItems[0].Tag is not int zfBoxId)
                return;

            using var dialog = new FinishedBoxDetailsDialog(_unitOfWork, zfBoxId);
            dialog.ShowDialog(this);
        }

        private static string NormalizeWorkOrder(string? value)
        {
            var normalized = (value ?? string.Empty).Trim().ToUpperInvariant();
            return normalized.StartsWith('O') ? normalized[1..] : normalized;
        }

        internal static string FormatHu(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "N/D";

            var normalized = value.Trim().ToUpperInvariant();
            return normalized.StartsWith("1J") ? normalized : $"1J{normalized}";
        }
    }
}
