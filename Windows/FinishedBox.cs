using HondaSensorChecker.Data.UnitOfWork;
using HondaSensorChecker.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HondaSensorChecker
{
    public partial class FinishedBox : Form
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _operatorId;

        // IDs only (avoid cross-context tracked entities)
        private readonly int _productId;
        private readonly int _workOrderId;
        private readonly int _zfBoxId;

        private readonly ZfBox _zfBox;

        public FinishedBox(
            IUnitOfWork unitOfWork,
            int operatorId,
            SapWorkOrder sapWorkOrder,
            Product product,
            ZfBox zfBox)
        {
            InitializeComponent();

            _unitOfWork = unitOfWork;
            _operatorId = operatorId;

            _workOrderId = sapWorkOrder?.SapWorkOrderId ?? 0;
            _productId = product?.ProductId ?? 0;
            _zfBoxId = zfBox?.ZfBoxId ?? 0;
            _zfBox = zfBox ?? new ZfBox();
        }

        private void txtUniqueNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
                return;

            var text = (txtUniqueNumber.Text ?? string.Empty).Trim().ToUpper();

            // Expected: "1J" + 10 chars
            if (text.Length == 12 && text.StartsWith("1J"))
            {
                _zfBox.UniqueNumber = text.Substring(2, 10);

                txtUniqueNumber.Enabled = false;
                txtMaterialNumber.Enabled = true;
                txtMaterialNumber.Focus();
                return;
            }

            MessageBox.Show("LEITURA INCORRETA", "ETIQUETA FINAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtUniqueNumber.Clear();
            txtUniqueNumber.Focus();
        }

        private void txtMaterialNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
                return;

            var text = (txtMaterialNumber.Text ?? string.Empty).Trim().ToUpper();

            // Expected: 'P' + 10 or 11 chars
            if ((text.Length == 11 || text.Length == 12) && text[0] == 'P')
            {
                var endPn = text.Substring(1);

                var productDb = _unitOfWork.Products.GetById(_productId);
                if (productDb == null)
                {
                    MessageBox.Show("Produto não encontrado no banco.", "ETIQUETA FINAL",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ResetToUniqueNumber(includeWorkOrder: true);
                    return;
                }

                if (string.Equals(productDb.EndPartNumber, endPn, StringComparison.OrdinalIgnoreCase))
                {
                    txtMaterialNumber.Enabled = false;
                    txtWorkOrder.Enabled = true;
                    txtWorkOrder.Focus();
                    return;
                }

                MessageBox.Show(
                    "PARTNUMBER DA ETIQUE FINAL É INCOMPATÍVEL COM O PARTNUMBER DOS SENSORES. ACIONAR FACILITADOR",
                    "ETIQUETA FINAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                ResetToUniqueNumber();
                return;
            }

            MessageBox.Show("LEITURA INCORRETA", "ETIQUETA FINAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ResetToUniqueNumber();
        }

        private void txtWorkOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
                return;

            var text = (txtWorkOrder.Text ?? string.Empty).Trim().ToUpper();

            // Expected: 'O' + 12 chars
            if (text.Length == 13 && text[0] == 'O')
            {
                var wo = text.Substring(1, 12);

                var woDb = _unitOfWork.SapWorkOrders.GetById(_workOrderId);
                if (woDb == null)
                {
                    MessageBox.Show("WorkOrder não encontrada no banco.", "ETIQUETA FINAL",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ResetToUniqueNumber(includeWorkOrder: true);
                    return;
                }

                if (string.Equals(woDb.WorkOrderNumber, wo, StringComparison.OrdinalIgnoreCase))
                {
                    txtWorkOrder.Enabled = false;
                    txtBatch.Enabled = true;
                    txtBatch.Focus();
                    return;
                }

                MessageBox.Show(
                    "WORKORDER DA ETIQUE FINAL É INCOMPATÍVEL COM O WORKORDER LIDO NO INÍCIO DO PROCESSO. ACIONAR FACILITADOR",
                    "ETIQUETA FINAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                ResetToUniqueNumber(includeWorkOrder: true);
                return;
            }

            MessageBox.Show("LEITURA INCORRETA", "ETIQUETA FINAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtWorkOrder.Clear();
            txtWorkOrder.Focus();
        }

        private void txtBatch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter)
                return;

            var text = (txtBatch.Text ?? string.Empty).Trim().ToUpper();

            // Expected: 'H' + 10 chars
            if (text.Length == 11 && text[0] == 'H')
            {
                _zfBox.Batch = text.Substring(1, 10);

                if (!PersistFinishedBox())
                    return;

                Close();
                return;
            }

            MessageBox.Show("LEITURA INCORRETA", "ETIQUETA FINAL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtBatch.Clear();
            txtBatch.Focus();
        }

        private bool PersistFinishedBox()
        {
            if (_workOrderId <= 0 || _productId <= 0)
            {
                MessageBox.Show("Contexto inválido (IDs).", "ETIQUETA FINAL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(_zfBox.UniqueNumber) || string.IsNullOrWhiteSpace(_zfBox.Batch))
            {
                MessageBox.Show("Dados incompletos para salvar a caixa.", "ETIQUETA FINAL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (_zfBoxId <= 0)
            {
                MessageBox.Show("Caixa em andamento não encontrada.", "ETIQUETA FINAL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var zfBoxDb = _unitOfWork.ZfBoxes.GetById(_zfBoxId);
            if (zfBoxDb == null)
            {
                MessageBox.Show("Caixa não encontrada no banco.", "ETIQUETA FINAL",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var sensorsDb = _unitOfWork.Sensors
                .Find(s => s.ZfBoxId == _zfBoxId && s.InProgress)
                .ToList();

            if (sensorsDb.Count == 0)
            {
                MessageBox.Show("Nenhum sensor em andamento para finalizar.", "ETIQUETA FINAL",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            zfBoxDb.UniqueNumber = _zfBox.UniqueNumber;
            zfBoxDb.Batch = _zfBox.Batch;
            zfBoxDb.InProgress = false;

            if (!_unitOfWork.ZfBoxes.Edit(zfBoxDb, out var error))
            {
                MessageBox.Show(error, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var supplierBoxCounts = new Dictionary<int, int>();

            foreach (var sensor in sensorsDb)
            {
                sensor.InProgress = false;

                if (!_unitOfWork.Sensors.Edit(sensor, out error))
                {
                    MessageBox.Show(error, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (!supplierBoxCounts.ContainsKey(sensor.SupplierBoxId))
                    supplierBoxCounts[sensor.SupplierBoxId] = 0;

                supplierBoxCounts[sensor.SupplierBoxId] += 1;
            }

            foreach (var kvp in supplierBoxCounts)
            {
                var sbDb = _unitOfWork.SupplierBoxes.GetById(kvp.Key);
                if (sbDb == null)
                    continue;

                sbDb.QtyRemaining = Math.Max(0, sbDb.QtyRemaining - kvp.Value);

                if (!_unitOfWork.SupplierBoxes.Edit(sbDb, out error))
                {
                    MessageBox.Show(error, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            var supplierBoxesUsed = supplierBoxCounts
                .Keys
                .Select(id => _unitOfWork.SupplierBoxes.GetById(id)?.UniqueNumber ?? id.ToString())
                .ToList();

            var workOrderNumber = _unitOfWork.SapWorkOrders.GetById(_workOrderId)?.WorkOrderNumber ?? _workOrderId.ToString();

            _unitOfWork.Logs.Add(new Log
            {
                Data = DateTime.Now,
                OperatorId = _operatorId,
                Description =
                    "Finished box created. " +
                    $"WorkOrderNumber={workOrderNumber}, " +
                    $"UniqueNumber={_zfBox.UniqueNumber}, " +
                    $"Batch={_zfBox.Batch}, " +
                    $"Sensors={sensorsDb.Count}, " +
                    $"SupplierBoxesUsed={string.Join(",", supplierBoxesUsed)}"
            }, out _);

            if (!_unitOfWork.Commit(out error))
            {
                MessageBox.Show(error, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void ResetToUniqueNumber(bool includeWorkOrder = false)
        {
            txtUniqueNumber.Clear();
            txtMaterialNumber.Clear();
            if (includeWorkOrder)
                txtWorkOrder.Clear();

            txtUniqueNumber.Enabled = true;
            txtMaterialNumber.Enabled = false;
            txtWorkOrder.Enabled = false;
            txtBatch.Enabled = false;

            txtUniqueNumber.Focus();
        }
    }
}
