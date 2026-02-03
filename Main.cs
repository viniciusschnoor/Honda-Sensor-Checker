using HondaSensorChecker.Data.UnitOfWork;
using HondaSensorChecker.Models;
using Microsoft.Extensions.DependencyInjection;

namespace HondaSensorChecker
{
    public partial class HSCMainForm : Form
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFinishedBoxFactory _finishedBoxFactory;
        private readonly IServiceProvider _serviceProvider;

        // Session/user context
        private readonly string _loggedWindowsUser = NormalizeUserName(Environment.UserName).ToUpperInvariant();
        private Operator? _currentOperator;

        // Current process context (work order -> supplier box -> sensors)
        private Product? _currentProduct;
        private SapWorkOrder? _currentWorkOrder;
        private SupplierBox? _currentSupplierBox;

        private readonly List<Sensor> _scannedSensors = new();
        private int _sensorCounter = 0;
        private int _sensorLimit = 0;

        // SupplierBox change control
        private bool _forcingSupplierBoxChange = false;
        private int _previousSupplierBoxId = 0;
        private string _previousSupplierBoxUniqueNumber = string.Empty;
        private bool _allowSupplierBoxOverdraw = false;
        private bool _overdrawLogged = false;

        public HSCMainForm(
            IUnitOfWork unitOfWork,
            IFinishedBoxFactory finishedBoxFactory,
            IServiceProvider serviceProvider)
        {
            _unitOfWork = unitOfWork;
            _finishedBoxFactory = finishedBoxFactory;
            _serviceProvider = serviceProvider;

            InitializeComponent();
        }

        private void HSCMainForm_Load(object sender, EventArgs e)
        {
            _currentOperator = _unitOfWork.Operators
                .GetAll()
                .FirstOrDefault(o =>
                    string.Equals(NormalizeUserName(o.ZfId).ToUpperInvariant(), _loggedWindowsUser,
                        StringComparison.OrdinalIgnoreCase));

            if (_currentOperator == null)
            {
                lblCheckResult.BackColor = Color.Red;
                lblCheckResult.ForeColor = Color.White;
                lblCheckResult.Text = "USUÁRIO NÃO REGISTRADO";
                return;
            }

            if (_currentOperator.Admin)
            {
                btnNewUser.Visible = true;
                btnNewProduct.Visible = true;
                btnLogs.Visible = true;
            }

            txtWorkOrderNumber.Enabled = true;
            txtWorkOrderNumber.Focus();
        }

        private void CleanForm()
        {
            _currentProduct = null;
            _currentWorkOrder = null;
            _currentSupplierBox = null;

            _scannedSensors.Clear();
            _sensorCounter = 0;
            _sensorLimit = 0;

            _forcingSupplierBoxChange = false;
            _previousSupplierBoxId = 0;
            _previousSupplierBoxUniqueNumber = string.Empty;
            _allowSupplierBoxOverdraw = false;
            _overdrawLogged = false;

            lblComponentQty.Text = "000/000";

            txtWorkOrderNumber.Enabled = true;
            txtWorkOrderMaterialNumber.Enabled = false;
            cbWorkOrderQtyToSend.Enabled = false;
            btnWorkOrderNok.Enabled = true;
            btnWorkOrderOk.Enabled = false;

            txtLogisticUniqueNumber.Enabled = false;
            txtStartPartNumber.Enabled = false;
            txtQtySupplied.Enabled = false;
            btnLogisticLabelNok.Enabled = false;
            btnLogisticLabelOk.Enabled = false;

            txtComponentSerial.Enabled = false;
            listBoxReadedSensors.Enabled = false;

            btnForceChangeSupplierBox.Enabled = false;

            txtWorkOrderNumber.Text = string.Empty;
            txtWorkOrderMaterialNumber.Text = string.Empty;
            cbWorkOrderQtyToSend.Text = string.Empty;

            txtLogisticUniqueNumber.Text = string.Empty;
            txtStartPartNumber.Text = string.Empty;
            txtQtySupplied.Text = string.Empty;

            txtComponentSerial.Text = string.Empty;
            listBoxReadedSensors.Items.Clear();

            lblCheckResult.Enabled = true;
            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.ForeColor = Color.Black;
            lblCheckResult.Text = "LEIA A WORK-ORDER";

            txtWorkOrderNumber.Focus();
        }

        // ----------------------------
        // LOG HELPERS
        // ----------------------------

        private void AddLogSafe(string description)
        {
            try
            {
                if (_currentOperator == null) return;

                _unitOfWork.Logs.Add(new Log
                {
                    Data = DateTime.Now,
                    OperatorId = _currentOperator.OperatorId,
                    Description = description
                }, out _);

                _unitOfWork.Commit(out _); // se falhar, ignora
            }
            catch
            {
                // ignora
            }
        }

        // ----------------------------
        // WORK ORDER
        // ----------------------------

        private void txtWorkOrderNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            var raw = (txtWorkOrderNumber.Text ?? string.Empty).Trim().ToUpper();

            if (raw.Length != 13 || raw[0] != 'O')
            {
                ShowWarningAndReset("CONFIRA O NÚMERO DA WORK-ORDER", "SAP WORK ORDER");
                return;
            }

            var workOrderNumber = raw.Substring(1, 12);

            var existingWorkOrder = _unitOfWork.SapWorkOrders
                .Find(wo => wo.WorkOrderNumber == workOrderNumber)
                .FirstOrDefault();

            if (existingWorkOrder != null)
            {
                _currentWorkOrder = existingWorkOrder;

                var product = _unitOfWork.Products
                    .Find(p => p.ProductId == existingWorkOrder.ProductId)
                    .FirstOrDefault();

                if (product == null)
                {
                    ShowWarningAndReset("PRODUTO NÃO ENCONTRADO PARA ESTA WORK-ORDER", "SAP WORK ORDER");
                    return;
                }

                _currentProduct = product;

                txtWorkOrderMaterialNumber.Text = $"P{product.EndPartNumber}";
                txtWorkOrderNumber.Enabled = false;

                cbWorkOrderQtyToSend.Enabled = true;
                cbWorkOrderQtyToSend.Focus();
                return;
            }

            // Work order does not exist; we will create it after reading the material number.
            _currentWorkOrder = new SapWorkOrder { WorkOrderNumber = workOrderNumber };

            txtWorkOrderNumber.Enabled = false;
            txtWorkOrderMaterialNumber.Enabled = true;
            txtWorkOrderMaterialNumber.Focus();
        }

        private void txtWorkOrderMaterialNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (_currentWorkOrder == null)
            {
                ShowWarningAndReset("WORK ORDER CONTEXT NOT FOUND", "SAP PARTNUMBER");
                return;
            }

            var raw = (txtWorkOrderMaterialNumber.Text ?? string.Empty).Trim().ToUpper();

            if ((raw.Length != 11 && raw.Length != 12) || raw[0] != 'P')
            {
                ShowWarningAndReset("CONFIRA O PARTNUMBER", "SAP PARTNUMBER");
                return;
            }

            var endPartNumber = raw.Substring(1);

            var product = _unitOfWork.Products
                .Find(p => p.EndPartNumber == endPartNumber)
                .FirstOrDefault();

            if (product == null)
            {
                ShowWarningAndReset("PARTNUMBER NÃO REGISTRADO", "SAP PARTNUMBER");
                return;
            }

            // If the work order was newly created, persist it now and commit to get the ID.
            if (_currentWorkOrder.SapWorkOrderId == 0)
            {
                _currentWorkOrder.ProductId = product.ProductId;

                if (!_unitOfWork.SapWorkOrders.Add(_currentWorkOrder, out var addError))
                {
                    MessageBox.Show(addError, "ERRO NO BANCO DE DADOS", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _unitOfWork.Logs.Add(new Log
                {
                    Data = DateTime.Now,
                    OperatorId = _currentOperator!.OperatorId,
                    Description =
                        "WorkOrder created. " +
                        $"WorkOrderNumber={_currentWorkOrder.WorkOrderNumber}, " +
                        $"EndPartNumber={product.EndPartNumber}, " +
                        $"Prefix={product.Prefix}"
                }, out _);

                if (!_unitOfWork.Commit(out var commitError))
                {
                    MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            _currentProduct = product;

            txtWorkOrderMaterialNumber.Enabled = false;
            cbWorkOrderQtyToSend.Enabled = true;
            cbWorkOrderQtyToSend.Focus();
        }

        private void cbWorkOrderQtyToSend_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbWorkOrderQtyToSend.SelectedItem == null) return;

            _sensorLimit = Convert.ToInt32(cbWorkOrderQtyToSend.SelectedItem);
            _sensorCounter = 0;

            lblComponentQty.Text = $"{_sensorCounter:D3}/{_sensorLimit:D3}";
            btnWorkOrderOk.Enabled = true;
        }

        private void btnWorkOrderNok_Click(object sender, EventArgs e)
        {
            CleanForm();
        }

        private void btnWorkOrderOk_Click(object sender, EventArgs e)
        {
            if (_currentWorkOrder == null || _currentProduct == null || _sensorLimit <= 0)
            {
                ShowWarningAndReset("INVALID WORK ORDER CONTEXT", "SAP WORK ORDER");
                return;
            }

            txtWorkOrderNumber.Enabled = false;
            txtWorkOrderMaterialNumber.Enabled = false;
            cbWorkOrderQtyToSend.Enabled = false;
            btnWorkOrderNok.Enabled = false;
            btnWorkOrderOk.Enabled = false;

            txtLogisticUniqueNumber.Enabled = true;
            btnLogisticLabelNok.Enabled = true;
            btnLogisticLabelOk.Enabled = false;

            txtLogisticUniqueNumber.Focus();
        }

        // ----------------------------
        // SUPPLIER BOX (LOGISTIC LABEL)
        // ----------------------------

        private void ShowSupplierBoxWarningKeepFlow(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Se estiver no meio do processo trocando supplier box, NÃO resetar tudo.
            if (_forcingSupplierBoxChange)
            {
                txtLogisticUniqueNumber.Enabled = true;
                txtStartPartNumber.Enabled = false;
                txtQtySupplied.Enabled = false;
                btnLogisticLabelOk.Enabled = false;

                txtLogisticUniqueNumber.Clear();
                txtStartPartNumber.Clear();
                txtQtySupplied.Clear();

                txtLogisticUniqueNumber.Focus();
                return;
            }

            CleanForm();
        }

        private void txtLogisticUniqueNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (_currentWorkOrder == null || _currentProduct == null)
            {
                ShowSupplierBoxWarningKeepFlow("WORK ORDER CONTEXT NOT FOUND", "LOGISTIC LABEL");
                return;
            }

            var raw = (txtLogisticUniqueNumber.Text ?? string.Empty).Trim().ToUpper();

            if (raw.Length != 11 || raw[0] != 'S')
            {
                ShowSupplierBoxWarningKeepFlow("CONFIRA O NÚMERO ÚNICO", "LOGISTIC LABEL");
                return;
            }

            var uniqueNumber = raw.Substring(1, 10);

            var existingSupplierBox = _unitOfWork.SupplierBoxes
                .Find(sb => sb.UniqueNumber == uniqueNumber)
                .FirstOrDefault();

            if (existingSupplierBox != null)
            {
                var supplierProduct = _unitOfWork.Products
                    .Find(p => p.ProductId == existingSupplierBox.ProductId)
                    .FirstOrDefault();

                if (supplierProduct == null)
                {
                    ShowSupplierBoxWarningKeepFlow("PRODUTO NÃO ENCONTRADO PARA ESTA CAIXA", "LOGISTIC LABEL");
                    return;
                }

                // Validate supplier box product matches work order product (by ProductId)
                if (supplierProduct.ProductId != _currentWorkOrder.ProductId)
                {
                    ShowSupplierBoxWarningKeepFlow("PARTNUMBER DESTA CAIXA NÃO COINCIDE COM O PARTNUMBER DA WORK-ORDER", "OPERATION ERROR");
                    return;
                }

                _currentSupplierBox = existingSupplierBox;
                _currentProduct = supplierProduct;
                _allowSupplierBoxOverdraw = false;
                _overdrawLogged = false;

                txtStartPartNumber.Text = $"P{supplierProduct.StartPartNumber}";
                txtQtySupplied.Text = $"Q{existingSupplierBox.QtyRemaining}";

                txtLogisticUniqueNumber.Enabled = false;

                // Se SB existe, pode liberar OK direto
                btnLogisticLabelOk.Enabled = true;
                btnLogisticLabelOk.Focus();
                return;
            }

            // SupplierBox does not exist; we will create it after reading StartPartNumber and Qty.
            _currentSupplierBox = new SupplierBox { UniqueNumber = uniqueNumber };
            _allowSupplierBoxOverdraw = false;
            _overdrawLogged = false;

            txtLogisticUniqueNumber.Enabled = false;
            txtStartPartNumber.Enabled = true;
            txtStartPartNumber.Focus();
        }

        private void txtStartPartNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (_currentWorkOrder == null || _currentSupplierBox == null)
            {
                ShowSupplierBoxWarningKeepFlow("WORK ORDER CONTEXT NOT FOUND", "LOGISTIC LABEL");
                return;
            }

            var raw = (txtStartPartNumber.Text ?? string.Empty).Trim().ToUpper();

            if (raw.Length != 9 || raw[0] != 'P')
            {
                ShowSupplierBoxWarningKeepFlow("CONFIRA O PARTNUMBER", "LOGISTIC LABEL");
                return;
            }

            var startPartNumber = raw.Substring(1, 8);

            var supplierProduct = _unitOfWork.Products
                .Find(p => p.StartPartNumber == startPartNumber)
                .FirstOrDefault();

            if (supplierProduct == null)
            {
                ShowSupplierBoxWarningKeepFlow("PARTNUMBER NÃO REGISTRADO", "LOGISTIC LABEL");
                return;
            }

            // Must match the work order product (same ProductId)
            if (supplierProduct.ProductId != _currentWorkOrder.ProductId)
            {
                ShowSupplierBoxWarningKeepFlow("PARTNUMBER DESTA CAIXA NÃO COINCIDE COM O PARTNUMBER DA WORK-ORDER", "LOGISTIC LABEL");
                return;
            }

            _currentSupplierBox.ProductId = supplierProduct.ProductId;
            _currentProduct = supplierProduct;

            txtStartPartNumber.Enabled = false;
            txtQtySupplied.Enabled = true;
            txtQtySupplied.Focus();
        }

        private void txtQtySupplied_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (_currentSupplierBox == null)
            {
                ShowSupplierBoxWarningKeepFlow("CAIXA NÃO ENCONTRADA", "LOGISTIC LABEL");
                return;
            }

            var raw = (txtQtySupplied.Text ?? string.Empty).Trim().ToUpper();

            if (raw.Length != 4 || raw[0] != 'Q')
            {
                ShowSupplierBoxWarningKeepFlow("CHEQUE O CÓDIGO DA QUANTIDADE (FORMATO DEVE SER Q###)", "LOGISTIC LABEL");
                return;
            }

            if (!int.TryParse(raw.Substring(1), out var qtySupplied) || qtySupplied <= 0)
            {
                ShowSupplierBoxWarningKeepFlow("QUANTIDADE INVÁLIDA", "LOGISTIC LABEL");
                return;
            }

            // Persist supplier box if it is newly created and commit to get SupplierBoxId.
            if (_currentSupplierBox.SupplierBoxId == 0)
            {
                _currentSupplierBox.QtySupplied = qtySupplied;
                _currentSupplierBox.QtyRemaining = qtySupplied;

                if (!_unitOfWork.SupplierBoxes.Add(_currentSupplierBox, out var addError))
                {
                    MessageBox.Show(addError, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _unitOfWork.Logs.Add(new Log
                {
                    Data = DateTime.Now,
                    OperatorId = _currentOperator!.OperatorId,
                    Description =
                        "SupplierBox created. " +
                        $"UniqueNumber={_currentSupplierBox.UniqueNumber}, " +
                        $"StartPartNumber={_currentProduct.StartPartNumber}, " +
                        $"EndPartNumber={_currentProduct.EndPartNumber}, " +
                        $"QtySupplied={_currentSupplierBox.QtySupplied}"
                }, out _);

                if (!_unitOfWork.Commit(out var commitError))
                {
                    MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // atualiza UI
                txtQtySupplied.Text = $"Q{_currentSupplierBox.QtyRemaining}";
            }

            txtQtySupplied.Enabled = false;
            btnLogisticLabelOk.Enabled = true;
            btnLogisticLabelOk.Focus();
        }

        private void btnLogisticLabelNok_Click(object sender, EventArgs e)
        {
            // Se estiver apenas trocando SupplierBox, não limpar tudo
            if (_forcingSupplierBoxChange)
            {
                ForceSupplierBoxChange("LEIA A ETIQUETA DE UMA OUTRA CAIXA COM SALDO.", changeType: "cancel_or_retry");
                return;
            }

            CleanForm();
        }

        private void btnLogisticLabelOk_Click(object sender, EventArgs e)
        {
            if (_currentWorkOrder == null || _currentProduct == null || _currentSupplierBox == null)
            {
                ShowSupplierBoxWarningKeepFlow("INVALID LOGISTIC LABEL CONTEXT", "LOGISTIC LABEL");
                return;
            }

            // Pré-check de saldo ANTES de iniciar o scan
            var sbDb = _unitOfWork.SupplierBoxes.GetById(_currentSupplierBox.SupplierBoxId);
            if (sbDb == null)
            {
                MessageBox.Show("CAIXA NÃO ENCONTRADA NO BANCO DE DADOS", "SUPPLIER BOX",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (_forcingSupplierBoxChange) ForceSupplierBoxChange("LEIA UMA NOVA CAIXA", changeType: "db_missing");
                else CleanForm();
                return;
            }

            // Atualiza memória/UI
            _currentSupplierBox.QtyRemaining = sbDb.QtyRemaining;
            txtQtySupplied.Text = $"Q{sbDb.QtyRemaining}";

            if (!EnsureSupplierBoxHasAvailableStock(sbDb, reason: "zero_before_start"))
                return;

            // Se houver saldo suficiente, não faz nada e deixa continuar.
            // Se não houver saldo suficiente, avisa e deixa continuar.
            if (_sensorLimit > 0 && sbDb.QtyRemaining < _sensorLimit)
            {
                MessageBox.Show(
                    $"ATENÇÃO: saldo insuficiente para a quantidade selecionada.\n" +
                    $"SALDO ATUAL: {sbDb.QtyRemaining}\n" +
                    $"QUANTIDADE A SER ENVIADA: {_sensorLimit}\n\n" +
                    $"VOCÊ PODERÁ CONTINUAR MAS, CASO A QUANTIA ZERE NO MEIO DO PROCESSO, SERÁ NECESSÁRIO ESCANEAR OUTRA CAIXA",
                    "SUPPLIER BOX",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            // Log de mudança de SupplierBox no meio do processo (quando estava forçando troca)
            if (_forcingSupplierBoxChange)
            {
                AddLogSafe(
                    "SupplierBox changed mid-process. " +
                    $"OldSupplierBox={_previousSupplierBoxUniqueNumber}, " +
                    $"NewSupplierBox={_currentSupplierBox.UniqueNumber}, " +
                    $"WorkOrderNumber={_currentWorkOrder.WorkOrderNumber}, " +
                    $"AlreadyScanned={_sensorCounter}/{_sensorLimit}");

                _forcingSupplierBoxChange = false;
                _previousSupplierBoxId = 0;
                _previousSupplierBoxUniqueNumber = string.Empty;
                _allowSupplierBoxOverdraw = false;
                _overdrawLogged = false;
            }

            // Volta ao fluxo normal: trava logistic label e libera scan
            txtLogisticUniqueNumber.Enabled = false;
            txtStartPartNumber.Enabled = false;
            txtQtySupplied.Enabled = false;
            btnLogisticLabelNok.Enabled = false;
            btnLogisticLabelOk.Enabled = false;

            txtComponentSerial.Enabled = true;
            listBoxReadedSensors.Enabled = true;

            // Agora pode forçar troca (estoque físico diferente do sistema)
            btnForceChangeSupplierBox.Enabled = true;

            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.ForeColor = Color.Black;
            lblCheckResult.Text = "POSICIONE O SENSOR E ESCANEIE O SERIAL";

            txtComponentSerial.Focus();
        }

        // changeType é só para ajudar no log (automático vs manual etc)
        private void ForceSupplierBoxChange(string message, string changeType)
        {
            if (_currentWorkOrder == null || _currentProduct == null)
                return;

            _forcingSupplierBoxChange = true;
            _previousSupplierBoxId = _currentSupplierBox?.SupplierBoxId ?? 0;
            _previousSupplierBoxUniqueNumber = _currentSupplierBox?.UniqueNumber ?? string.Empty;

            AddLogSafe(
                "SupplierBox change requested. " +
                $"Type={changeType}, " +
                $"OldSupplierBox={_previousSupplierBoxUniqueNumber}, " +
                $"WorkOrderNumber={_currentWorkOrder.WorkOrderNumber}, " +
                $"AlreadyScanned={_sensorCounter}/{_sensorLimit}");

            MessageBox.Show(message, "SUPPLIER BOX", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // Pausa leitura de sensor
            txtComponentSerial.Enabled = false;

            // Reabilita apenas fluxo do logistic label
            txtLogisticUniqueNumber.Enabled = true;
            txtStartPartNumber.Enabled = false;
            txtQtySupplied.Enabled = false;

            btnLogisticLabelNok.Enabled = true;
            btnLogisticLabelOk.Enabled = false;

            txtLogisticUniqueNumber.Clear();
            txtStartPartNumber.Clear();
            txtQtySupplied.Clear();

            // bloquear botão enquanto está em modo de troca
            btnForceChangeSupplierBox.Enabled = false;

            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.ForeColor = Color.Black;
            lblCheckResult.Text = "LEIA UMA NOVA CAIXA DO FORNECEDOR";

            txtLogisticUniqueNumber.Focus();
        }

        // Zera saldo da SB atual se operador escolheu SIM
        private void ZeroCurrentSupplierBoxRemaining(string reason)
        {
            if (_currentSupplierBox == null) return;

            var sbDb = _unitOfWork.SupplierBoxes.GetById(_currentSupplierBox.SupplierBoxId);
            if (sbDb == null) return;

            if (sbDb.QtyRemaining <= 0)
            {
                AddLogSafe(
                    "SupplierBox zero request ignored (already zero). " +
                    $"SupplierBox={sbDb.UniqueNumber}, " +
                    $"WorkOrderNumber={_currentWorkOrder?.WorkOrderNumber}, " +
                    $"Reason={reason}");
                return;
            }

            sbDb.QtyRemaining = 0;

            if (!_unitOfWork.SupplierBoxes.Edit(sbDb, out var editError))
            {
                MessageBox.Show(editError, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_unitOfWork.Commit(out var commitError))
            {
                MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _currentSupplierBox.QtyRemaining = 0;
            txtQtySupplied.Text = "Q0";

            AddLogSafe(
                "SupplierBox stock set to ZERO by operator. " +
                $"SupplierBox={sbDb.UniqueNumber}, " +
                $"WorkOrderNumber={_currentWorkOrder?.WorkOrderNumber}, " +
                $"Reason={reason}");
        }

        // Debita 1 unidade por scan; se zerou -> força troca
        private bool TryDebitOneFromSupplierBoxOrRequestChange()
        {
            if (_currentSupplierBox == null)
                return false;

            var sbDb = _unitOfWork.SupplierBoxes.GetById(_currentSupplierBox.SupplierBoxId);
            if (sbDb == null)
            {
                MessageBox.Show("SupplierBox não encontrada no banco.", "SUPPLIER BOX",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (sbDb.QtyRemaining <= 0)
            {
                if (!EnsureSupplierBoxHasAvailableStock(sbDb, reason: "zero_mid_process"))
                    return false;

                if (sbDb.QtyRemaining <= 0)
                    return true;
            }

            sbDb.QtyRemaining -= 1;

            if (!_unitOfWork.SupplierBoxes.Edit(sbDb, out var editError))
            {
                MessageBox.Show(editError, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!_unitOfWork.Commit(out var commitError))
            {
                MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            _currentSupplierBox.QtyRemaining = sbDb.QtyRemaining;
            txtQtySupplied.Text = $"Q{sbDb.QtyRemaining}";

            return true;
        }

        // ----------------------------
        // FORÇAR TROCA (NOVO BOTÃO)
        // ----------------------------

        private void btnForceChangeSupplierBox_Click(object sender, EventArgs e)
        {
            // Só faz sentido se já está no modo de scan (tem SB atual e WO)
            if (_currentWorkOrder == null || _currentProduct == null || _currentSupplierBox == null)
                return;

            // 1) confirmar troca
            var confirm = MessageBox.Show(
                "Deseja realmente trocar a Supplier Box?\n\nUse esta opção quando a caixa ficou vazia, mas no sistema ainda há saldo disponível",
                "TROCAR SUPPLIER BOX",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                AddLogSafe(
                    "SupplierBox manual change canceled by operator. " +
                    $"CurrentSupplierBox={_currentSupplierBox.UniqueNumber}, " +
                    $"WorkOrderNumber={_currentWorkOrder.WorkOrderNumber}");
                txtComponentSerial.Focus();
                return;
            }

            AddLogSafe(
                "SupplierBox manual change CONFIRMED by operator. " +
                $"CurrentSupplierBox={_currentSupplierBox.UniqueNumber}, " +
                $"WorkOrderNumber={_currentWorkOrder.WorkOrderNumber}");

            // 2) perguntar se quer zerar saldo no sistema
            var zero = MessageBox.Show(
                "Deseja zerar o estoque restante da caixa atual no sistema?\n\n" +
                "Selecione SIM se fisicamente a caixa acabou (não há mais componentes na caixa).",
                "ZERAR ESTOQUE DA CAIXA ATUAL?",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (zero == DialogResult.Yes)
            {
                ZeroCurrentSupplierBoxRemaining(reason: "manual_change_physical_empty");
            }
            else
            {
                AddLogSafe(
                    "SupplierBox manual change: operator chose NOT to zero stock. " +
                    $"SupplierBox={_currentSupplierBox.UniqueNumber}");
            }

            // 3) entrar em modo de troca
            ForceSupplierBoxChange("Leia a NOVA SupplierBox.", changeType: "manual_button");
        }

        // ----------------------------
        // SENSOR SCANNING
        // ----------------------------

        private void txtComponentSerial_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (_currentOperator == null || _currentWorkOrder == null || _currentProduct == null || _currentSupplierBox == null)
            {
                ShowWarningAndReset("PROCESS CONTEXT NOT FOUND", "SENSOR");
                return;
            }

            if (_sensorCounter >= _sensorLimit)
            {
                ShowSensorStatusNok("CAIXA A SER EXPEDIDA JÁ COMPLETA");
                txtComponentSerial.Text = string.Empty;
                return;
            }

            var serial = (txtComponentSerial.Text ?? string.Empty).Trim().ToUpper();
            if (serial.Length != 9)
            {
                ShowSensorStatusNok("CONFIRA O SERIAL NUMBER DO SENSOR");
                return;
            }

            // Determine product by prefix (first 4 chars)
            var prefix = serial.Substring(0, 4);
            var scannedProduct = _unitOfWork.Products.Find(p => p.Prefix == prefix).FirstOrDefault();
            if (scannedProduct == null)
            {
                ShowSensorStatusNok($"{prefix} NÃO REGISTRADO PARA NENHUM PARTNUMBER");
                return;
            }

            // Avoid duplicates: database and current list
            if (_unitOfWork.Sensors.Find(s => s.SerialNumber == serial).Any())
            {
                ShowSensorStatusNok($"{serial} JÁ FOI EXPEDIDO EM OUTRA CAIXA");
                return;
            }

            if (_scannedSensors.Any(s => s.SerialNumber == serial) || listBoxReadedSensors.Items.Contains(serial))
            {
                ShowSensorStatusNok($"{serial} JÁ FOI LIDO NESTA MESMA CAIXA");
                return;
            }

            // Validate scanned sensor matches expected product
            if (scannedProduct.ProductId != _currentProduct.ProductId)
            {
                ShowSensorStatusNok($"ESPERADO: {_currentProduct.Prefix}  LIDO: {prefix}");
                return;
            }

            // Debita 1 unidade AGORA (reserva). Se zerou, força troca.
            if (!TryDebitOneFromSupplierBoxOrRequestChange())
            {
                txtComponentSerial.Text = string.Empty;
                return;
            }

            // Create sensor object in memory (persisted later in FinishedBox)
            var sensor = new Sensor
            {
                SerialNumber = serial,
                ScannedTime = DateTime.Now,
                ProductId = scannedProduct.ProductId,
                OperatorId = _currentOperator.OperatorId,
                SupplierBoxId = _currentSupplierBox.SupplierBoxId, // pode mudar no meio do processo
                SapWorkOrderId = _currentWorkOrder.SapWorkOrderId
            };

            _scannedSensors.Add(sensor);
            listBoxReadedSensors.Items.Insert(0, serial);

            _sensorCounter++;
            lblComponentQty.Text = $"{_sensorCounter:D3}/{_sensorLimit:D3}";

            if (_sensorCounter < _sensorLimit)
            {
                ShowSensorStatusOk(serial);
                txtComponentSerial.Text = string.Empty;
                txtComponentSerial.Focus();
                return;
            }

            // Box is full; open finalization dialog
            lblCheckResult.Enabled = false;

            // Snapshot
            var sensorsSnapshot = _scannedSensors.ToList();

            // bloquear botão pois vai finalizar
            btnForceChangeSupplierBox.Enabled = false;

            var dialog = _finishedBoxFactory.Create(
                _currentWorkOrder,
                _currentSupplierBox,
                _currentProduct,
                _sensorCounter,
                sensorsSnapshot,
                _currentOperator.OperatorId);

            dialog.ShowDialog();
            CleanForm();
        }

        private void btnRemoveSensor_Click(object sender, EventArgs e)
        {
            if (listBoxReadedSensors.SelectedItem == null)
                return;

            var serial = listBoxReadedSensors.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(serial))
                return;

            // Remove da lista em memória
            var removed = _scannedSensors.RemoveAll(s => s.SerialNumber == serial) > 0;

            // Remove da UI
            listBoxReadedSensors.Items.Remove(serial);

            if (removed)
            {
                _sensorCounter = Math.Max(0, _sensorCounter - 1);
                lblComponentQty.Text = $"{_sensorCounter:D3}/{_sensorLimit:D3}";

                RestoreSupplierBoxStockForRemovedSensor();

                // Log de scrap
                AddLogSafe(
                    $"Sensor removed (SCRAP). Serial={serial}, " +
                    $"WorkOrderNumber={_currentWorkOrder?.WorkOrderNumber}, " +
                    $"SupplierBox={_currentSupplierBox?.UniqueNumber}, " +
                    $"CounterNow={_sensorCounter}/{_sensorLimit}");
            }

            txtComponentSerial.Focus();
        }

        private void lblCheckResult_Click(object sender, EventArgs e)
        {
            // When NOK, allow retry by enabling the scan input again
            if (lblCheckResult.Text.StartsWith("NOK", StringComparison.OrdinalIgnoreCase))
            {
                txtComponentSerial.Text = string.Empty;
                txtComponentSerial.Enabled = true;
                txtComponentSerial.Focus();
                return;
            }

            // If disabled due to finalization flow, allow full reset
            if (!lblCheckResult.Enabled)
                CleanForm();
        }

        // ----------------------------
        // ADMIN FORMS
        // ----------------------------

        private void btnNewUser_Click(object sender, EventArgs e)
        {
            using var scope = _serviceProvider.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var dialog = new Users(uow, _currentOperator!.OperatorId);
            dialog.ShowDialog();
        }

        private void btnNewProduct_Click(object sender, EventArgs e)
        {
            using var scope = _serviceProvider.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var dialog = new Products(uow, _currentOperator!.OperatorId);
            dialog.ShowDialog();
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {
            using var scope = _serviceProvider.CreateScope();

            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var dialog = new Logs(uow);
            dialog.ShowDialog();
        }

        private void txtWorkOrderNumber_Leave(object sender, EventArgs e)
        {
            btnWorkOrderNok.Enabled = true;
        }

        // ----------------------------
        // UI helpers
        // ----------------------------

        private void ShowWarningAndReset(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            CleanForm();
        }

        private void ShowSensorStatusOk(string serial)
        {
            lblCheckResult.BackColor = Color.Green;
            lblCheckResult.ForeColor = Color.White;
            lblCheckResult.Text = $"OK\n{serial}";
            txtComponentSerial.Enabled = true;
        }

        private void ShowSensorStatusNok(string message)
        {
            lblCheckResult.BackColor = Color.Red;
            lblCheckResult.ForeColor = Color.White;
            lblCheckResult.Text = $"NOK\n{message}";
            txtComponentSerial.Enabled = false;
            txtComponentSerial.Focus();
        }

        private void txtWorkOrderMaterialNumber_Enter(object sender, EventArgs e)
        {
            lblCheckResult.Text = "LEIA O PARTNUMBER DA WORK-ORDER";
            btnWorkOrderNok.Enabled = true;
        }

        private void cbWorkOrderQtyToSend_Enter(object sender, EventArgs e)
        {
            lblCheckResult.Text = "SELECIONE A QUANTIDADE A EMPACOTAR";
        }

        private void txtLogisticUniqueNumber_Enter(object sender, EventArgs e)
        {
            lblCheckResult.Text = "LEIA O NÚMERO ÚNICO NA ETIQUETA DA LOGÍSTICA";
        }

        private void txtStartPartNumber_Enter(object sender, EventArgs e)
        {
            lblCheckResult.Text = "LEIA O PARTNUMBER NA ETIQUETA DA LOGÍSTICA";
        }

        private void txtQtySupplied_Enter(object sender, EventArgs e)
        {
            lblCheckResult.Text = "LEIA A QUANTIDADE TOTAL NA ETIQUETA DA LOGÍSTICA";
        }

        private void txtComponentSerial_Enter(object sender, EventArgs e)
        {
            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.ForeColor = Color.Black;
            lblCheckResult.Text = "POSICIONE O SENSOR E ESCANEIE O SERIAL";
        }

        private bool EnsureSupplierBoxHasAvailableStock(SupplierBox sbDb, string reason)
        {
            if (sbDb.QtyRemaining > 0)
                return true;

            if (_allowSupplierBoxOverdraw)
                return true;

            var confirm = MessageBox.Show(
                "Saldo da caixa está zerado no sistema, mas ainda contém sensores na caixa física?\n\n" +
                "Clique SIM para continuar usando esta mesma caixa.\n" +
                "Clique NÃO para abrir outra caixa.",
                "SUPPLIER BOX",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                ForceSupplierBoxChange("Leia a etiqueta da nova caixa.", changeType: reason);
                return false;
            }

            _allowSupplierBoxOverdraw = true;

            if (!_overdrawLogged)
            {
                AddLogSafe(
                    "SupplierBox overdraw enabled. " +
                    $"SupplierBox={sbDb.UniqueNumber}, " +
                    $"WorkOrderNumber={_currentWorkOrder?.WorkOrderNumber}, " +
                    $"Reason={reason}");
                _overdrawLogged = true;
            }

            return true;
        }

        private void RestoreSupplierBoxStockForRemovedSensor()
        {
            if (_currentSupplierBox == null)
                return;

            var sbDb = _unitOfWork.SupplierBoxes.GetById(_currentSupplierBox.SupplierBoxId);
            if (sbDb == null)
                return;

            sbDb.QtyRemaining += 1;
            if (sbDb.QtySupplied > 0 && sbDb.QtyRemaining > sbDb.QtySupplied)
                sbDb.QtyRemaining = sbDb.QtySupplied;

            if (!_unitOfWork.SupplierBoxes.Edit(sbDb, out _))
                return;

            if (!_unitOfWork.Commit(out _))
                return;

            _currentSupplierBox.QtyRemaining = sbDb.QtyRemaining;
            txtQtySupplied.Text = $"Q{sbDb.QtyRemaining}";

            AddLogSafe(
                "SupplierBox stock restored after scrap removal. " +
                $"SupplierBox={sbDb.UniqueNumber}, " +
                $"WorkOrderNumber={_currentWorkOrder?.WorkOrderNumber}, " +
                $"QtyRemaining={sbDb.QtyRemaining}");
        }

        private static string NormalizeUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return string.Empty;

            var withoutDomain = userName.Contains('\\')
                ? userName.Split('\\', 2)[1]
                : userName;

            return withoutDomain.Contains('@')
                ? withoutDomain.Split('@', 2)[0]
                : withoutDomain;
        }
    }
}
