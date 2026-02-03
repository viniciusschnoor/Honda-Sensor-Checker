using HondaSensorChecker.Data.UnitOfWork;
using HondaSensorChecker.Models;
using System.ComponentModel;
using System.Linq;

namespace HondaSensorChecker
{
    public partial class Products : Form
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _operatorId;

        private readonly BindingSource _bsProducts = new BindingSource();
        private bool _refreshingGrid = false;

        public Products(IUnitOfWork unitOfWork, int operatorId)
        {
            _unitOfWork = unitOfWork;
            _operatorId = operatorId;
            InitializeComponent();
        }

        private BindingList<Product> LoadProducts()
        {
            var list = _unitOfWork.Products.GetAll().ToList();
            return new BindingList<Product>(list);
        }

        private void Products_Load(object sender, EventArgs e)
        {
            _bsProducts.DataSource = LoadProducts();
            dgvSensors.AutoGenerateColumns = true;
            dgvSensors.DataSource = _bsProducts;

            dgvSensors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSensors.MultiSelect = false;
            dgvSensors.AllowUserToAddRows = false;

            dgvSensors.Columns[0].HeaderText = "ID";
            dgvSensors.Columns[1].HeaderText = "PREFIX";
            dgvSensors.Columns[2].HeaderText = "ZF PARTNUMBER";
            dgvSensors.Columns[3].HeaderText = "ELSEN/ELMOD";

            dgvSensors.Columns[0].ReadOnly = true;
        }

        private void RefreshProductsGridSafe()
        {
            if (_refreshingGrid) return;
            _refreshingGrid = true;

            BeginInvoke(new Action(() =>
            {
                try
                {
                    dgvSensors.EndEdit();
                    dgvSensors.CommitEdit(DataGridViewDataErrorContexts.Commit);

                    _bsProducts.DataSource = LoadProducts();
                    _bsProducts.ResetBindings(false);
                }
                finally
                {
                    _refreshingGrid = false;
                }
            }));
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPrefix.Text) ||
                string.IsNullOrWhiteSpace(txtPn.Text) ||
                string.IsNullOrWhiteSpace(txtElsen.Text))
            {
                MessageBox.Show("FILL ALL REQUIRED FIELDS", "PRODUCT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearInputs();
                return;
            }

            var product = new Product
            {
                Prefix = txtPrefix.Text.Trim().ToUpper(),
                StartPartNumber = txtPn.Text.Trim().ToUpper(),
                EndPartNumber = txtElsen.Text.Trim().ToUpper()
            };

            if (!IsValidEndPartNumber(product.EndPartNumber))
            {
                MessageBox.Show("END PARTNUMBER INVALID (use ELSEN#####, ELSENA##### or ELMOD#####)", "PRODUCT",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearInputs();
                return;
            }

            var hasStart = _unitOfWork.Products.Find(p => p.StartPartNumber == product.StartPartNumber).Any();
            var hasEnd = _unitOfWork.Products.Find(p => p.EndPartNumber == product.EndPartNumber).Any();

            if (hasStart || hasEnd)
            {
                MessageBox.Show("PARTNUMBER ALREADY EXISTS", "PRODUCT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearInputs();
                return;
            }

            if (!_unitOfWork.Products.Add(product, out var addError))
            {
                MessageBox.Show(addError, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_unitOfWork.Commit(out var commitError))
            {
                MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TryAddLogSafe($"Product created. ProductId={product.ProductId}, Prefix={product.Prefix}, StartPN={product.StartPartNumber}, EndPN={product.EndPartNumber}");

            RefreshProductsGridSafe();
            ClearInputs();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvSensors.SelectedRows.Count == 0)
                return;

            var id = Convert.ToInt32(dgvSensors.SelectedRows[0].Cells[0].Value);

            if (HasProductDependencies(id, out var dependencyMessage))
            {
                MessageBox.Show(
                    dependencyMessage,
                    "Delete blocked",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!_unitOfWork.Products.Remove(id, out var removeError))
            {
                MessageBox.Show(removeError, "Delete error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_unitOfWork.Commit(out var commitError))
            {
                if (IsFkViolation(commitError))
                {
                    MessageBox.Show(
                        "Não é possível remover este produto porque existem registros vinculados.\n" +
                        "Verifique: sensores, caixas (supplier/zf), work orders ou logs.",
                        "Delete blocked",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TryAddLogSafe($"Product deleted. ProductId={id}");

            RefreshProductsGridSafe();
        }

        private void dgvSensors_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_refreshingGrid) return;
            if (e.RowIndex < 0) return;

            dgvSensors.EndEdit();
            dgvSensors.CommitEdit(DataGridViewDataErrorContexts.Commit);

            if (dgvSensors.Rows[e.RowIndex].DataBoundItem is not Product product)
                return;

            product.Prefix = (product.Prefix ?? string.Empty).Trim().ToUpper();
            product.StartPartNumber = (product.StartPartNumber ?? string.Empty).Trim().ToUpper();
            product.EndPartNumber = (product.EndPartNumber ?? string.Empty).Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(product.Prefix) ||
                string.IsNullOrWhiteSpace(product.StartPartNumber) ||
                string.IsNullOrWhiteSpace(product.EndPartNumber))
            {
                MessageBox.Show("FILL ALL REQUIRED FIELDS", "PRODUCT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshProductsGridSafe();
                return;
            }

            if (!IsValidEndPartNumber(product.EndPartNumber))
            {
                MessageBox.Show("END PARTNUMBER INVALID (use ELSEN#####, ELSENA##### or ELMOD#####)", "PRODUCT",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshProductsGridSafe();
                return;
            }

            var duplicatePrefix = _unitOfWork.Products.Find(p => p.Prefix == product.Prefix && p.ProductId != product.ProductId).Any();
            if (duplicatePrefix)
            {
                MessageBox.Show("PREFIX ALREADY EXISTS", "PRODUCT", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshProductsGridSafe();
                return;
            }

            if (!_unitOfWork.Products.Edit(product, out var editError))
            {
                MessageBox.Show(editError, "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RefreshProductsGridSafe();
                return;
            }

            if (!_unitOfWork.Commit(out var commitError))
            {
                if (IsFkViolation(commitError))
                {
                    MessageBox.Show(
                        "Não foi possível salvar a alteração.\n" +
                        "Este produto já está em uso e alguma mudança gerou conflito de integridade (FK).\n" +
                        "Dica: se o produto já foi usado, evite alterar PartNumbers/Prefix.",
                        "Update blocked",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                RefreshProductsGridSafe();
                return;
            }

            TryAddLogSafe($"Product updated. ProductId={product.ProductId}");

            RefreshProductsGridSafe();
        }

        private void TryAddLogSafe(string description)
        {
            try
            {
                if (_operatorId <= 0) return;

                var operatorExists = _unitOfWork.Operators.Find(o => o.OperatorId == _operatorId).Any();
                if (!operatorExists) return;

                _unitOfWork.Logs.Add(new Log
                {
                    Data = DateTime.Now,
                    OperatorId = _operatorId,
                    Description = description
                }, out _);

                _unitOfWork.Commit(out _); // se falhar, ignora
            }
            catch
            {
                // ignora log
            }
        }

        private bool HasProductDependencies(int productId, out string message)
        {
            message = string.Empty;

            var sensors = _unitOfWork.Sensors.Find(s => s.ProductId == productId).Count();
            var supplierBoxes = _unitOfWork.SupplierBoxes.Find(sb => sb.ProductId == productId).Count();
            var zfBoxes = _unitOfWork.ZfBoxes.Find(zb => zb.ProductId == productId).Count();
            var workOrders = _unitOfWork.SapWorkOrders.Find(wo => wo.ProductId == productId).Count();

            var total = sensors + supplierBoxes + zfBoxes + workOrders;
            if (total == 0)
                return false;

            message =
                "Não é possível remover este produto porque existem registros vinculados.\n" +
                $"Sensores: {sensors}\n" +
                $"Caixas supplier: {supplierBoxes}\n" +
                $"Caixas ZF: {zfBoxes}\n" +
                $"Work orders: {workOrders}\n" +
                "Remova os vínculos antes de excluir o produto.";
            return true;
        }

        private bool IsFkViolation(string error)
        {
            if (string.IsNullOrWhiteSpace(error)) return false;

            return error.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) ||
                   error.Contains("FOREIGN_KEY_VIOLATION", StringComparison.OrdinalIgnoreCase) ||
                   error.Contains("FOREIGN KEY constraint failed", StringComparison.OrdinalIgnoreCase);
        }

        private bool IsValidEndPartNumber(string endPartNumber)
        {
            if (string.IsNullOrWhiteSpace(endPartNumber))
                return false;

            if (endPartNumber.StartsWith("ELMOD", StringComparison.OrdinalIgnoreCase))
            {
                return endPartNumber.Length == 10 && endPartNumber.Skip(5).All(char.IsDigit);
            }

            return endPartNumber.StartsWith("ELSEN", StringComparison.OrdinalIgnoreCase) &&
                   ((endPartNumber.Length == 10 && endPartNumber.Skip(5).All(char.IsDigit)) ||
                    (endPartNumber.Length == 11 && endPartNumber[5] == 'A' && endPartNumber.Skip(6).All(char.IsDigit)));
        }

        private void ClearInputs()
        {
            txtPrefix.Text = string.Empty;
            txtPn.Text = string.Empty;
            txtElsen.Text = string.Empty;
            txtPrefix.Focus();
        }
    }
}
