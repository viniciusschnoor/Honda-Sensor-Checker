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
        private readonly Configuration.AccSettings _accSettings;

        // Session/user context
        private readonly string _loggedWindowsUser = NormalizeUserName(Environment.UserName).ToUpperInvariant();
        private Operator? _currentOperator;

        // Current process context (work order -> supplier box -> sensors)
        private Product? _currentProduct;
        private SapWorkOrder? _currentWorkOrder;
        private SupplierBox? _currentSupplierBox;
        private ZfBox? _currentZfBox;

        private readonly List<Sensor> _scannedSensors = new();
        private Sensor? _pendingAccSensor;
        private int _sensorCounter = 0;
        private int _sensorLimit = 0;
        private int _runtimeSupplierBoxRemaining = 0;
        private bool _suppressQtyToSendChange = false;

        // SupplierBox change control
        private bool _forcingSupplierBoxChange = false;
        private int _previousSupplierBoxId = 0;
        private string _previousSupplierBoxUniqueNumber = string.Empty;
        private bool _allowSupplierBoxOverdraw = false;
        private bool _overdrawLogged = false;
        private int? _lockedBoxProductId;
        private string _lockedBoxPartNumber = string.Empty;
        private int? _currentAccPartTypeId;
        private string _currentAccPartDescription = string.Empty;
        private bool _accPartTypeDataInProgress;
        private bool _sensorOperationInProgress;
        private bool _criticalProcessBlock;
        private RetryTarget _retryTarget = RetryTarget.WorkOrder;

#if DEBUG
        private const string DebugAccBypassWorkOrder = "012345678912";
        private bool _debugAccBypassEnabled;
#endif

        private enum RetryTarget
        {
            WorkOrder,
            Sensor
        }

        public HSCMainForm(
            IUnitOfWork unitOfWork,
            IFinishedBoxFactory finishedBoxFactory,
            IServiceProvider serviceProvider,
            Configuration.AccSettings accSettings)
        {
            _unitOfWork = unitOfWork;
            _finishedBoxFactory = finishedBoxFactory;
            _serviceProvider = serviceProvider;
            _accSettings = accSettings;

            InitializeComponent();
        }

        private async void HSCMainForm_Load(object sender, EventArgs e)
        {
#if DEBUG
            lblDebugMode.Visible = true;
#endif

            Logging.ApplicationFileLogger.Information(
                "UI.MainFormLoading",
                "Main form is loading.",
                BuildApplicationLogContext());

            _currentOperator = _unitOfWork.Operators
                .GetAll()
                .FirstOrDefault(o =>
                    string.Equals(NormalizeUserName(o.ZfId).ToUpperInvariant(), _loggedWindowsUser,
                        StringComparison.OrdinalIgnoreCase));

            if (_currentOperator == null)
            {
                Logging.ApplicationFileLogger.Warning(
                    "Security.UnregisteredWindowsUser",
                    "The current Windows user is not registered as an operator.",
                    BuildApplicationLogContext());
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

            Logging.ApplicationFileLogger.Information(
                "Security.OperatorAuthenticated",
                "Operator identified from the current Windows user.",
                BuildApplicationLogContext());

            UpdateContinueProcessButton();
            await TryResumeActiveProcessOnStartupAsync();
        }

        private void CleanForm()
        {
            _currentProduct = null;
            _currentWorkOrder = null;
            _currentSupplierBox = null;
            _currentZfBox = null;
            _lockedBoxProductId = null;
            _lockedBoxPartNumber = string.Empty;
            _currentAccPartTypeId = null;
            _currentAccPartDescription = string.Empty;
            _accPartTypeDataInProgress = false;
            _sensorOperationInProgress = false;
            _criticalProcessBlock = false;
            _retryTarget = RetryTarget.WorkOrder;
#if DEBUG
            _debugAccBypassEnabled = false;
#endif

            _scannedSensors.Clear();
            _pendingAccSensor = null;
            _sensorCounter = 0;
            _sensorLimit = 0;
            _runtimeSupplierBoxRemaining = 0;

            _forcingSupplierBoxChange = false;
            _previousSupplierBoxId = 0;
            _previousSupplierBoxUniqueNumber = string.Empty;
            _allowSupplierBoxOverdraw = false;
            _overdrawLogged = false;
            _runtimeSupplierBoxRemaining = 0;

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
            btnRemoveSensor.Enabled = false;

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

            UpdateContinueProcessButton();
        }

        // ----------------------------
        // LOG HELPERS
        // ----------------------------

        private void AddLogSafe(
            string description,
            Logging.ApplicationLogLevel level = Logging.ApplicationLogLevel.Information,
            string eventName = "Process.Audit",
            Exception? exception = null)
        {
            Logging.ApplicationFileLogger.Write(
                level,
                eventName,
                description,
                exception,
                BuildApplicationLogContext());

            try
            {
                if (_currentOperator == null) return;

                if (!_unitOfWork.Logs.Add(new Log
                {
                    Data = DateTime.Now,
                    OperatorId = _currentOperator.OperatorId,
                    Description = description
                }, out var addError))
                {
                    Logging.ApplicationFileLogger.Error(
                        "Database.AuditLogAddFailed",
                        "Unable to add the business audit record to the database.",
                        context: MergeApplicationLogContext(new Dictionary<string, object?>
                        {
                            ["AuditDescription"] = description,
                            ["DatabaseError"] = addError
                        }));
                    return;
                }

                if (!_unitOfWork.Commit(out var commitError))
                {
                    Logging.ApplicationFileLogger.Error(
                        "Database.AuditLogCommitFailed",
                        "Unable to commit the business audit record to the database.",
                        context: MergeApplicationLogContext(new Dictionary<string, object?>
                        {
                            ["AuditDescription"] = description,
                            ["DatabaseError"] = commitError
                        }));
                }
            }
            catch (Exception ex)
            {
                Logging.ApplicationFileLogger.Error(
                    "Database.AuditLogUnexpectedFailure",
                    "Unexpected failure while writing the business audit record.",
                    ex,
                    MergeApplicationLogContext(new Dictionary<string, object?>
                    {
                        ["AuditDescription"] = description
                    }));
            }
        }

        private Dictionary<string, object?> BuildApplicationLogContext() => new()
        {
            ["LoggedWindowsUser"] = _loggedWindowsUser,
            ["OperatorId"] = _currentOperator?.OperatorId,
            ["OperatorZfId"] = _currentOperator?.ZfId,
            ["WorkOrderNumber"] = _currentWorkOrder?.WorkOrderNumber,
            ["ProductId"] = _currentProduct?.ProductId,
            ["ProductPrefix"] = _currentProduct?.Prefix,
            ["StartPartNumber"] = _currentProduct?.StartPartNumber,
            ["EndPartNumber"] = _currentProduct?.EndPartNumber,
            ["SupplierBoxId"] = _currentSupplierBox?.SupplierBoxId,
            ["SupplierBoxUniqueNumber"] = _currentSupplierBox?.UniqueNumber,
            ["ZfBoxId"] = _currentZfBox?.ZfBoxId,
            ["SensorCounter"] = _sensorCounter,
            ["SensorLimit"] = _sensorLimit,
            ["SupplierBoxRemaining"] = _runtimeSupplierBoxRemaining,
            ["AccPartTypeId"] = _currentAccPartTypeId,
            ["AccPartDescription"] = _currentAccPartDescription,
            ["AccEndpoint"] = $"{_accSettings.IpAddress}:{_accSettings.Port}",
            ["AccStation"] = _accSettings.Station,
            ["RetryTarget"] = _retryTarget.ToString()
        };

        private Dictionary<string, object?> MergeApplicationLogContext(
            IReadOnlyDictionary<string, object?> additionalContext)
        {
            var context = BuildApplicationLogContext();
            foreach (var item in additionalContext)
                context[item.Key] = item.Value;
            return context;
        }

        // ----------------------------
        // WORK ORDER
        // ----------------------------

        private void txtWorkOrderNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            var raw = (txtWorkOrderNumber.Text ?? string.Empty).Trim().ToUpperInvariant();

            if (!WorkOrderRules.TryNormalizeScannedLabel(raw, out var workOrderNumber))
            {
                ShowWarningAndReset(
                    $"CONFIRA O NÚMERO DA WORK-ORDER\n\nFORMATOS ACEITOS:\n{WorkOrderRules.ExpectedFormats}",
                    "SAP WORK ORDER");
                return;
            }

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
                UpdateContinueProcessButton();
                return;
            }

            // Work order does not exist; we will create it after reading the material number.
            _currentWorkOrder = new SapWorkOrder { WorkOrderNumber = workOrderNumber };

            txtWorkOrderNumber.Enabled = false;
            txtWorkOrderMaterialNumber.Enabled = true;
            txtWorkOrderMaterialNumber.Focus();
            UpdateContinueProcessButton();
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
            if (_suppressQtyToSendChange)
                return;

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
            Logging.ApplicationFileLogger.Warning(
                "Validation.SupplierBoxRejected",
                message,
                MergeApplicationLogContext(new Dictionary<string, object?>
                {
                    ["DialogTitle"] = title
                }));
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

                if (_lockedBoxProductId.HasValue &&
                    (supplierProduct.ProductId != _lockedBoxProductId.Value ||
                     !string.Equals(supplierProduct.StartPartNumber, _lockedBoxPartNumber,
                         StringComparison.OrdinalIgnoreCase)))
                {
                    ShowSupplierBoxWarningKeepFlow(
                        $"PARTNUMBER INVÁLIDO. ESPERADO: {_lockedBoxPartNumber}",
                        "TROCA DE SUPPLIER BOX");
                    return;
                }

                _currentSupplierBox = existingSupplierBox;
                _currentProduct = supplierProduct;
                _allowSupplierBoxOverdraw = false;
                _overdrawLogged = false;
                _runtimeSupplierBoxRemaining = existingSupplierBox.QtyRemaining;

                txtStartPartNumber.Text = $"P{supplierProduct.StartPartNumber}";
                txtQtySupplied.Text = $"Q{_runtimeSupplierBoxRemaining}";

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


            if (_lockedBoxProductId.HasValue &&
                (supplierProduct.ProductId != _lockedBoxProductId.Value ||
                 !string.Equals(supplierProduct.StartPartNumber, _lockedBoxPartNumber,
                     StringComparison.OrdinalIgnoreCase)))
            {
                ShowSupplierBoxWarningKeepFlow(
                    $"PARTNUMBER INVÁLIDO. ESPERADO: {_lockedBoxPartNumber}",
                    "TROCA DE SUPPLIER BOX");
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
                _runtimeSupplierBoxRemaining = _currentSupplierBox.QtyRemaining;
                txtQtySupplied.Text = $"Q{_runtimeSupplierBoxRemaining}";
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
            _runtimeSupplierBoxRemaining = sbDb.QtyRemaining;
            txtQtySupplied.Text = $"Q{_runtimeSupplierBoxRemaining}";

            if (!EnsureSupplierBoxHasAvailableStock(sbDb, reason: "zero_before_start"))
                return;

            if (_currentAccPartTypeId == null ||
                !_currentAccPartDescription.Contains(
                    _currentWorkOrder.WorkOrderNumber, StringComparison.OrdinalIgnoreCase))
            {
                ShowWorkOrderAccFailure("PARTTYPE DA WORK ORDER NÃO CARREGADO");
                return;
            }

            _lockedBoxProductId ??= _currentProduct.ProductId;
            if (string.IsNullOrWhiteSpace(_lockedBoxPartNumber))
                _lockedBoxPartNumber = _currentProduct.StartPartNumber;

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

            if (_currentZfBox == null)
            {
                var newZfBox = new ZfBox
                {
                    QtyToSend = _sensorLimit,
                    ProductId = _currentProduct.ProductId,
                    SapWorkOrderId = _currentWorkOrder.SapWorkOrderId,
                    OperatorId = _currentOperator!.OperatorId,
                    InProgress = true,
                    IsPaused = false,
                    CurrentSupplierBoxId = _currentSupplierBox.SupplierBoxId
                };

                if (!_unitOfWork.ZfBoxes.Add(newZfBox, out var addError))
                {
                    MessageBox.Show(addError, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!_unitOfWork.Commit(out var commitError))
                {
                    MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _currentZfBox = newZfBox;
            }
            else if (!_currentZfBox.InProgress)
            {
                MessageBox.Show("A caixa selecionada já foi finalizada.", "ZF BOX",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_currentZfBox.CurrentSupplierBoxId != _currentSupplierBox.SupplierBoxId ||
                _currentZfBox.IsPaused)
            {
                _currentZfBox.CurrentSupplierBoxId = _currentSupplierBox.SupplierBoxId;
                _currentZfBox.IsPaused = false;

                if (!_unitOfWork.ZfBoxes.Edit(_currentZfBox, out var stateError) ||
                    !_unitOfWork.Commit(out stateError))
                {
                    MessageBox.Show(stateError, "Erro ao salvar estado da caixa",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
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
            UpdateContinueProcessButton();
        }

        private async Task<bool> TryLoadAccPartTypeDataAsync(string workOrderNumber)
        {
            if (string.IsNullOrWhiteSpace(workOrderNumber))
                return false;

//#if DEBUG
//            if (string.Equals(workOrderNumber, DebugAccBypassWorkOrder, StringComparison.Ordinal))
//            {
//                _debugAccBypassEnabled = true;
//                _currentAccPartTypeId = 0;
//                _currentAccPartDescription = $"DEBUG ACC BYPASS - WORK ORDER {workOrderNumber}";

//                AddLogSafe(
//                    "ACC bypass enabled by the Debug test Work Order. " +
//                    $"WorkOrderNumber={workOrderNumber}. No ACC command will be sent for this process.",
//                    Logging.ApplicationLogLevel.Warning,
//                    "Debug.AccBypassEnabled");

//                lblCheckResult.BackColor = Color.FromArgb(255, 193, 7);
//                lblCheckResult.ForeColor = Color.Black;
//                lblCheckResult.Text = "MODO DEBUG\nBYPASS DO ACC ATIVO";
//                return true;
//            }

//            _debugAccBypassEnabled = false;
//#endif

            if (_currentAccPartTypeId.HasValue &&
                _currentAccPartDescription.Contains(workOrderNumber, StringComparison.OrdinalIgnoreCase))
                return true;

            if (string.IsNullOrWhiteSpace(_accSettings.IpAddress) ||
                _accSettings.Port is < 1 or > 65535 ||
                string.IsNullOrWhiteSpace(_accSettings.DllVersion) ||
                string.IsNullOrWhiteSpace(_accSettings.ProductType) ||
                string.IsNullOrWhiteSpace(_accSettings.Station))
            {
                ShowWorkOrderAccFailure("CONFIGURAÇÃO DO ACC INCOMPLETA");
                return false;
            }

            _accPartTypeDataInProgress = true;
            _currentAccPartTypeId = null;
            _currentAccPartDescription = string.Empty;
            txtWorkOrderMaterialNumber.Enabled = false;
            cbWorkOrderQtyToSend.Enabled = false;
            btnWorkOrderOk.Enabled = false;
            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.ForeColor = Color.Black;
            lblCheckResult.Text = "CONSULTANDO WORK ORDER NO ACC";

            try
            {
                var partTypeSetup = await Task.Run(() =>
                {
                    using var client = ZF.ACCComm.Client.ACCCommClient.Connect(
                        ZF.ACCComm.Utils.NetworkUtils.GetEndpoint(_accSettings.IpAddress, _accSettings.Port));
                    var matches = client.PartTypeList(
                            _accSettings.Station,
                            _accSettings.ProductType,
                            _accSettings.DllVersion)
                        .Where(item => item.PartDesc?.Contains(
                            workOrderNumber, StringComparison.OrdinalIgnoreCase) == true)
                        .ToList();

                    if (matches.Count == 0)
                        throw new InvalidOperationException(
                            $"Work Order '{workOrderNumber}' não encontrada no PartTypeList do ACC.");

                    if (matches.Count > 1)
                        throw new InvalidOperationException(
                            $"Work Order '{workOrderNumber}' possui {matches.Count} correspondências no ACC.");

                    var match = matches[0];

                    var response = client.PartTypeData(
                        _accSettings.Station,
                        _accSettings.ProductType,
                        _accSettings.DllVersion,
                        match.PartTypeID);

                    return (match.PartTypeID, Description: match.PartDesc?.Trim() ?? string.Empty,
                        Response: response);
                });

                _currentAccPartTypeId = partTypeSetup.PartTypeID;
                _currentAccPartDescription = partTypeSetup.Description;

                AddLogSafe(
                    "ACC Work Order PartTypeData completed. " +
                    $"WorkOrderNumber={workOrderNumber}, " +
                    $"PartTypeID={partTypeSetup.PartTypeID}, " +
                    $"PartDescription={partTypeSetup.Description}, " +
                    $"Station={_accSettings.Station}, " +
                    $"IntegerParameters={string.Join(',', partTypeSetup.Response.IntegerParameterList ?? [])}, " +
                    $"RealParameters={string.Join(',', partTypeSetup.Response.RealParameterList ?? [])}, " +
                    $"StringParameters={string.Join(',', partTypeSetup.Response.StringParameterList ?? [])}",
                    eventName: "ACC.WorkOrderPartTypeDataCompleted");
                return true;
            }
            catch (Exception ex)
            {
                _currentAccPartTypeId = null;
                _currentAccPartDescription = string.Empty;
                ShowWorkOrderAccFailure(ex.Message);
                AddLogSafe(
                    "ACC Work Order PartTypeData failed. " +
                    $"WorkOrderNumber={workOrderNumber}, Error={ex.Message}",
                    Logging.ApplicationLogLevel.Error,
                    "ACC.WorkOrderPartTypeDataFailed",
                    ex);
                return false;
            }
            finally
            {
                _accPartTypeDataInProgress = false;
            }
        }

        private void ShowWorkOrderAccFailure(string message)
        {
            _retryTarget = RetryTarget.WorkOrder;
            Logging.ApplicationFileLogger.Error(
                "ACC.WorkOrderFlowBlocked",
                message,
                context: BuildApplicationLogContext());
            lblCheckResult.BackColor = Color.Red;
            lblCheckResult.ForeColor = Color.White;
            lblCheckResult.Text = $"NOK ACC\n{message}";
            txtWorkOrderMaterialNumber.Enabled = false;
            cbWorkOrderQtyToSend.Enabled = false;
            btnWorkOrderOk.Enabled = false;
            txtComponentSerial.Enabled = false;
        }

        private void ShowAccFailure(string message)
        {
            _retryTarget = RetryTarget.Sensor;
            Logging.ApplicationFileLogger.Error(
                "ACC.SensorFlowBlocked",
                message,
                context: BuildApplicationLogContext());
            lblCheckResult.BackColor = Color.Red;
            lblCheckResult.ForeColor = Color.White;
            lblCheckResult.Text = $"NOK ACC\n{message}";
            txtComponentSerial.Enabled = false;
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
            _runtimeSupplierBoxRemaining = 0;
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

            if (_runtimeSupplierBoxRemaining <= 0)
            {
                if (!EnsureSupplierBoxHasAvailableStock(sbDb, reason: "zero_mid_process"))
                    return false;

                if (_runtimeSupplierBoxRemaining <= 0)
                    return true;
            }

            _runtimeSupplierBoxRemaining -= 1;
            txtQtySupplied.Text = $"Q{_runtimeSupplierBoxRemaining}";

            return true;
        }

        private void RestoreReservedSupplierBoxStock(int previousRemaining)
        {
            if (_currentSupplierBox == null)
                return;

            _runtimeSupplierBoxRemaining = previousRemaining;
            txtQtySupplied.Text = $"Q{_runtimeSupplierBoxRemaining}";
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

        private async void txtComponentSerial_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;

            if (_currentOperator == null || _currentWorkOrder == null || _currentProduct == null || _currentSupplierBox == null)
            {
                ShowWarningAndReset("PROCESS CONTEXT NOT FOUND", "SENSOR");
                return;
            }
            if (_currentZfBox == null)
            {
                ShowWarningAndReset("ZF BOX CONTEXT NOT FOUND", "SENSOR");
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

            // Avoid duplicates, preserving a specific audit message for scrap.
            var existingSensor = _unitOfWork.Sensors
                .Find(s => s.SerialNumber == serial)
                .FirstOrDefault();
            if (existingSensor != null)
            {
                if (existingSensor.IsScrap)
                {
                    var scrapOperator = string.IsNullOrWhiteSpace(existingSensor.ScrapOperatorName)
                        ? "NÃO IDENTIFICADO"
                        : existingSensor.ScrapOperatorName;
                    ShowSensorStatusNok(
                        $"{serial} FOI MARCADO COMO SCRAP PELO OPERADOR {scrapOperator}");
                }
                else
                {
                    ShowSensorStatusNok($"{serial} JÁ FOI LIDO EM OUTRA CAIXA OU NESTE PROCESSO");
                }

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
            var supplierRemainingBeforeReservation = _runtimeSupplierBoxRemaining;
            if (!TryDebitOneFromSupplierBoxOrRequestChange())
            {
                txtComponentSerial.Text = string.Empty;
                return;
            }

            _sensorOperationInProgress = true;
            UpdateContinueProcessButton();
            var loadResult = (Success: false, CycleId: (long?)null, UnitPartTypeId: (int?)null);
            try
            {
                if (_pendingAccSensor != null &&
                    !await CompleteSensorInAccAsync(_pendingAccSensor, approved: true))
                {
                    RestoreReservedSupplierBoxStock(supplierRemainingBeforeReservation);
                    return;
                }

                _pendingAccSensor = null;
                loadResult = await LoadSensorInAccAsync(serial);
            }
            finally
            {
                _sensorOperationInProgress = false;
                UpdateContinueProcessButton();
            }

            if (!loadResult.Success)
            {
                RestoreReservedSupplierBoxStock(supplierRemainingBeforeReservation);
                return;
            }

            // Create sensor object and persist immediately
            var sensor = new Sensor
            {
                SerialNumber = serial,
                ScannedTime = DateTime.Now,
                ProductId = scannedProduct.ProductId,
                OperatorId = _currentOperator.OperatorId,
                SupplierBoxId = _currentSupplierBox.SupplierBoxId, // pode mudar no meio do processo
                SapWorkOrderId = _currentWorkOrder.SapWorkOrderId,
                ZfBoxId = _currentZfBox.ZfBoxId,
                InProgress = true,
                AccState = SensorAccState.Loaded,
                AccPartTypeId = _currentAccPartTypeId,
                AccCycleId = loadResult.CycleId,
                AccUnitPartTypeId = loadResult.UnitPartTypeId
            };

            if (!_unitOfWork.Sensors.Add(sensor, out var addError))
            {
                await TryCompensateUnpersistedLoadAsync(sensor);
                RestoreReservedSupplierBoxStock(supplierRemainingBeforeReservation);
                MessageBox.Show(addError, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_unitOfWork.Commit(out var commitError))
            {
                await TryCompensateUnpersistedLoadAsync(sensor);
                RestoreReservedSupplierBoxStock(supplierRemainingBeforeReservation);
                MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _scannedSensors.Add(sensor);
            _pendingAccSensor = sensor;
            listBoxReadedSensors.Items.Insert(0, serial);
            listBoxReadedSensors.SelectedIndex = 0;
            UpdateScrapButtonState();

            _sensorCounter++;
            lblComponentQty.Text = $"{_sensorCounter:D3}/{_sensorLimit:D3}";

            if (_sensorCounter < _sensorLimit)
            {
                txtComponentSerial.Text = string.Empty;
                txtComponentSerial.Focus();
                return;
            }

            await ConfirmLastSensorAndFinalizeAsync();
        }

        private void OpenFinishedBoxDialog()
        {
            if (_currentWorkOrder == null || _currentProduct == null ||
                _currentZfBox == null || _currentOperator == null)
                return;

            var unresolvedSensors = _scannedSensors
                .Where(sensor =>
                    (!sensor.IsScrap && sensor.AccState != SensorAccState.UnloadedOk) ||
                    (sensor.IsScrap && sensor.AccState != SensorAccState.UnloadedNok))
                .Select(sensor => sensor.SerialNumber)
                .ToList();
            if (_sensorCounter != _sensorLimit || unresolvedSensors.Count > 0)
            {
                ShowSensorStatusNok(
                    unresolvedSensors.Count > 0
                        ? $"EXISTEM SENSORES PENDENTES NO ACC: {string.Join(", ", unresolvedSensors)}"
                        : $"QUANTIDADE BOA INCOMPLETA: {_sensorCounter}/{_sensorLimit}");
                return;
            }

            lblCheckResult.Enabled = false;
            btnForceChangeSupplierBox.Enabled = false;
            btnInterruptProcess.Enabled = false;

            var dialog = _finishedBoxFactory.Create(
                _currentWorkOrder,
                _currentProduct,
                _currentZfBox,
                _currentOperator.OperatorId);

            dialog.ShowDialog();
            CleanForm();
        }

        private async void btnRemoveSensor_Click(object sender, EventArgs e)
        {
            if (_sensorOperationInProgress || listBoxReadedSensors.SelectedItem == null)
                return;

            var serial = listBoxReadedSensors.SelectedItem.ToString();
            if (string.IsNullOrWhiteSpace(serial))
                return;

            if (listBoxReadedSensors.SelectedIndex != 0 ||
                _pendingAccSensor == null ||
                !string.Equals(_pendingAccSensor.SerialNumber, serial, StringComparison.OrdinalIgnoreCase) ||
                _pendingAccSensor.AccState != SensorAccState.Loaded)
            {
                MessageBox.Show(
                    "Somente o primeiro sensor da lista, que corresponde ao último sensor lido, pode ser marcado como scrap.",
                    "SCRAP BLOQUEADO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                UpdateScrapButtonState();
                return;
            }

            var confirm = MessageBox.Show(
                $"Confirma o scrap do sensor {serial}?\n\n" +
                "O ACC receberá Unload NOK e este serial nunca poderá ser lido novamente.",
                "CONFIRMAR SCRAP",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes)
                return;

            _sensorOperationInProgress = true;
            UpdateContinueProcessButton();
            try
            {
                if (!await CompleteSensorInAccAsync(
                        _pendingAccSensor,
                        approved: false,
                        scrapOperator: _currentOperator))
                    return;

                _sensorCounter = Math.Max(0, _sensorCounter - 1);
                lblComponentQty.Text = $"{_sensorCounter:D3}/{_sensorLimit:D3}";

                AddLogSafe(
                    "Sensor marked as scrap after ACC Unload NOK. " +
                    $"Serial={serial}, WorkOrderNumber={_currentWorkOrder?.WorkOrderNumber}, " +
                    $"ScrapOperatorId={_pendingAccSensor.ScrapOperatorId}, " +
                    $"ScrapOperatorName={_pendingAccSensor.ScrapOperatorName}, " +
                    $"SupplierBoxId={_pendingAccSensor.SupplierBoxId}, " +
                    $"CounterNow={_sensorCounter}/{_sensorLimit}",
                    Logging.ApplicationLogLevel.Warning,
                    "ACC.SensorScrapped");

                _pendingAccSensor = null;
                btnRemoveSensor.Enabled = false;
                lblCheckResult.BackColor = Color.FromArgb(255, 193, 7);
                lblCheckResult.ForeColor = Color.Black;
                lblCheckResult.Text = $"SCRAP REGISTRADO\n{serial}\nLEIA UM NOVO SENSOR";
                txtComponentSerial.Clear();
                txtComponentSerial.Enabled = true;
            }
            finally
            {
                _sensorOperationInProgress = false;
                UpdateContinueProcessButton();
            }

            txtComponentSerial.Focus();
        }

        private async void lblCheckResult_Click(object sender, EventArgs e)
        {
            if (_criticalProcessBlock)
            {
                MessageBox.Show(
                    "O estado do ACC e do banco precisa ser reconciliado pela Manutenção. Consulte o log antes de reiniciar o processo.",
                    "PROCESSO BLOQUEADO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            if (lblCheckResult.Text.StartsWith("NOK", StringComparison.OrdinalIgnoreCase))
            {
                if (_sensorCounter >= _sensorLimit && _pendingAccSensor != null)
                {
                    await ConfirmLastSensorAndFinalizeAsync();
                    return;
                }

                RetryCurrentStageAfterNok();
                return;
            }

            if (_sensorCounter >= _sensorLimit && _pendingAccSensor != null)
            {
                await ConfirmLastSensorAndFinalizeAsync();
                return;
            }

            // If disabled due to finalization flow, allow full reset
            if (!lblCheckResult.Enabled)
                CleanForm();
        }

        private void RetryCurrentStageAfterNok()
        {
            Logging.ApplicationFileLogger.Information(
                "UI.NokPanelClicked",
                "Operator requested a retry from the NOK result panel.",
                BuildApplicationLogContext());

            if (_retryTarget == RetryTarget.WorkOrder)
            {
                _currentAccPartTypeId = null;
                _currentAccPartDescription = string.Empty;
                txtComponentSerial.Enabled = false;
                txtWorkOrderMaterialNumber.Enabled = false;
                cbWorkOrderQtyToSend.Enabled = false;
                btnWorkOrderOk.Enabled = false;
                txtWorkOrderNumber.Enabled = true;
                txtWorkOrderNumber.SelectAll();
                lblCheckResult.BackColor = Color.Yellow;
                lblCheckResult.ForeColor = Color.Black;
                lblCheckResult.Text = "LEIA NOVAMENTE A WORK-ORDER";
                txtWorkOrderNumber.Focus();
                return;
            }

            var sensorContextIsValid =
                _currentOperator != null &&
                _currentWorkOrder != null &&
                _currentProduct != null &&
                _currentSupplierBox != null &&
                _currentZfBox != null &&
                _currentAccPartTypeId.HasValue;

            if (!sensorContextIsValid)
            {
                _retryTarget = RetryTarget.WorkOrder;
                ShowWorkOrderAccFailure("CONTEXTO DO PROCESSO INCOMPLETO");
                txtWorkOrderNumber.Enabled = true;
                txtWorkOrderNumber.SelectAll();
                txtWorkOrderNumber.Focus();
                return;
            }

            txtComponentSerial.Clear();
            txtComponentSerial.Enabled = true;
            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.ForeColor = Color.Black;
            lblCheckResult.Text = "POSICIONE O SENSOR E ESCANEIE O SERIAL";
            txtComponentSerial.Focus();
        }

        // ----------------------------
        // ADMIN FORMS
        // ----------------------------

        private void btnConsultComponent_Click(object sender, EventArgs e)
        {
            var dialog = new ComponentHistoryDialog(_unitOfWork);
            dialog.ShowDialog();
        }

        private void btnConsultFinishedBoxes_Click(object sender, EventArgs e)
        {
            using var dialog = new WorkOrderFinishedBoxesDialog(_unitOfWork);
            dialog.ShowDialog(this);
        }

        private void btnInterruptProcess_Click(object sender, EventArgs e)
        {
            if (_sensorOperationInProgress)
                return;

            if (_currentWorkOrder == null || _currentProduct == null || _currentZfBox == null || !_currentZfBox.InProgress)
            {
                MessageBox.Show(
                    "Não existe uma caixa em andamento para interromper.",
                    "INTERROMPER PROCESSO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                UpdateContinueProcessButton();
                return;
            }

            using var authorizationDialog = new AdminAuthorizationDialog();
            if (authorizationDialog.ShowDialog(this) != DialogResult.OK)
                return;

            var authorizingAdmin = _unitOfWork.Operators
                .GetAll()
                .FirstOrDefault(op =>
                    op.Admin &&
                    string.Equals(op.Re?.Trim(), authorizationDialog.AdminRe, StringComparison.OrdinalIgnoreCase));

            if (authorizingAdmin == null)
            {
                AddLogSafe(
                    "Process interruption authorization rejected. " +
                    $"EnteredAdminRe={authorizationDialog.AdminRe}, ZfBoxId={_currentZfBox.ZfBoxId}",
                    Logging.ApplicationLogLevel.Warning,
                    "Security.ProcessInterruptionAuthorizationRejected");
                MessageBox.Show(
                    "RE não encontrado ou usuário sem permissão de administrador.",
                    "AUTORIZAÇÃO NEGADA",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var firstConfirmation = MessageBox.Show(
                "Deseja interromper o processo atual e deixar esta caixa aguardando?\n\n" +
                $"Work Order: {_currentWorkOrder.WorkOrderNumber}\n" +
                $"Sensores lidos: {_sensorCounter}/{_sensorLimit}\n" +
                $"Administrador: {authorizingAdmin.Name}",
                "CONFIRMAR INTERRUPÇÃO",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (firstConfirmation != DialogResult.Yes)
                return;

            var finalConfirmation = MessageBox.Show(
                "ÚLTIMA CONFIRMAÇÃO\n\n" +
                "A caixa permanecerá em andamento e deverá ser retomada pelo botão CONTINUAR PROCESSO.\n\n" +
                "Confirma a interrupção agora?",
                "RECONFIRMAR INTERRUPÇÃO",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (finalConfirmation != DialogResult.Yes)
                return;

            var interruptedZfBoxId = _currentZfBox.ZfBoxId;
            var interruptedWorkOrder = _currentWorkOrder.WorkOrderNumber;
            var interruptedSensorCount = _sensorCounter;
            var interruptedSensorLimit = _sensorLimit;

            _currentZfBox.IsPaused = true;
            _currentZfBox.CurrentSupplierBoxId = _currentSupplierBox?.SupplierBoxId ??
                                                   _currentZfBox.CurrentSupplierBoxId;

            if (!_unitOfWork.ZfBoxes.Edit(_currentZfBox, out var pauseError) ||
                !_unitOfWork.Commit(out pauseError))
            {
                MessageBox.Show(
                    $"Não foi possível colocar a caixa em espera.\n\n{pauseError}",
                    "ERRO AO INTERROMPER PROCESSO",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            AddLogSafe(
                "Process interrupted and left waiting. " +
                $"ZfBoxId={interruptedZfBoxId}, WorkOrderNumber={interruptedWorkOrder}, " +
                $"Sensors={interruptedSensorCount}/{interruptedSensorLimit}, " +
                $"AuthorizingAdminId={authorizingAdmin.OperatorId}, AuthorizingAdminRe={authorizingAdmin.Re}, " +
                $"AuthorizingAdminName={authorizingAdmin.Name}",
                Logging.ApplicationLogLevel.Warning,
                "Process.InterruptedByAdministrator");

            CleanForm();
        }

        private async void btnContinueProcess_Click(object sender, EventArgs e)
        {
            if (_currentWorkOrder != null)
                return;

            var inProgressBoxes = _unitOfWork.ZfBoxes
                .Find(z => z.InProgress)
                .OrderByDescending(z => z.ZfBoxId)
                .ToList();

            if (inProgressBoxes.Count == 0)
            {
                MessageBox.Show("Nenhuma caixa em andamento encontrada.", "CONTINUAR PROCESSO",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var options = inProgressBoxes
                .Select(z =>
                {
                    var workOrderNumber = _unitOfWork.SapWorkOrders.GetById(z.SapWorkOrderId)?.WorkOrderNumber ?? z.SapWorkOrderId.ToString();
                    var product = _unitOfWork.Products.GetById(z.ProductId);
                    var productLabel = product == null ? "Produto desconhecido" : $"{product.EndPartNumber}";
                    var uniqueLabel = string.IsNullOrWhiteSpace(z.UniqueNumber) ? "(sem etiqueta)" : z.UniqueNumber;

                    return new ContinueProcessOption
                    {
                        ZfBox = z,
                        Display = $"WO {workOrderNumber} | {productLabel} | Qtd {z.QtyToSend} | ZfBox {uniqueLabel}"
                    };
                })
                .ToList();

            using var dialog = new ContinueProcessDialog(options);
            if (dialog.ShowDialog() != DialogResult.OK || dialog.SelectedZfBox == null)
                return;

            await ResumePersistedProcessAsync(dialog.SelectedZfBox, restoreSupplierBox: false);
        }

        private async Task TryResumeActiveProcessOnStartupAsync()
        {
            var activeBoxes = _unitOfWork.ZfBoxes
                .Find(z => z.InProgress && !z.IsPaused)
                .OrderByDescending(z => z.ZfBoxId)
                .ToList();

            if (activeBoxes.Count == 0)
                return;

            if (activeBoxes.Count > 1)
            {
                AddLogSafe(
                    "Multiple active boxes were found during startup. The most recent one will be restored. " +
                    $"ZfBoxIds={string.Join(',', activeBoxes.Select(z => z.ZfBoxId))}",
                    Logging.ApplicationLogLevel.Warning,
                    "Process.MultipleActiveBoxesFound");
            }

            var activeBox = activeBoxes[0];
            AddLogSafe(
                $"Automatically restoring active process. ZfBoxId={activeBox.ZfBoxId}",
                eventName: "Process.AutomaticResumeStarted");

            await ResumePersistedProcessAsync(activeBox, restoreSupplierBox: true);
        }

        private async Task<bool> ResumePersistedProcessAsync(
            ZfBox zfBox,
            bool restoreSupplierBox)
        {
            var selectedWorkOrder = _unitOfWork.SapWorkOrders.GetById(zfBox.SapWorkOrderId);
            if (selectedWorkOrder == null)
            {
                MessageBox.Show("Work Order da caixa não encontrada.", "CONTINUAR PROCESSO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            StartContinueProcess(zfBox);

            var pendingSensors = _scannedSensors
                .Where(sensor => sensor.AccState == SensorAccState.Loaded && !sensor.IsScrap)
                .ToList();
            if (pendingSensors.Count > 1)
            {
                var pendingSerials = string.Join(", ", pendingSensors.Select(sensor => sensor.SerialNumber));
                BlockProcessForMaintenance(
                    $"MAIS DE UM SENSOR POSSUI LOAD PENDENTE\n{pendingSerials}");
                AddLogSafe(
                    "Process resume blocked because multiple sensors have pending ACC Loads. " +
                    $"ZfBoxId={zfBox.ZfBoxId}, Serials={pendingSerials}",
                    Logging.ApplicationLogLevel.Critical,
                    "Process.MultiplePendingAccLoads");
                return false;
            }

            if (!await TryLoadAccPartTypeDataAsync(selectedWorkOrder.WorkOrderNumber))
                return false;

            if (zfBox.IsPaused)
            {
                zfBox.IsPaused = false;
                if (!_unitOfWork.ZfBoxes.Edit(zfBox, out var resumeError) ||
                    !_unitOfWork.Commit(out resumeError))
                {
                    MessageBox.Show(resumeError, "Erro ao retomar caixa",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            if (_sensorLimit > 0 && _sensorCounter >= _sensorLimit)
            {
                await ConfirmLastSensorAndFinalizeAsync();
                return true;
            }

            if (restoreSupplierBox && TryRestorePersistedSupplierBox(zfBox))
            {
                AddLogSafe(
                    "Active process restored automatically. " +
                    $"ZfBoxId={zfBox.ZfBoxId}, WorkOrderNumber={selectedWorkOrder.WorkOrderNumber}, " +
                    $"SupplierBoxId={_currentSupplierBox?.SupplierBoxId}, Sensors={_sensorCounter}/{_sensorLimit}",
                    eventName: "Process.AutomaticResumeCompleted");
            }
            else if (restoreSupplierBox)
            {
                AddLogSafe(
                    "The active process was restored, but its SupplierBox could not be recovered. " +
                    $"ZfBoxId={zfBox.ZfBoxId}. A logistic label scan is required.",
                    Logging.ApplicationLogLevel.Warning,
                    "Process.AutomaticResumeSupplierBoxRequired");
            }

            return true;
        }

        private bool TryRestorePersistedSupplierBox(ZfBox zfBox)
        {
            var supplierBoxId = zfBox.CurrentSupplierBoxId ??
                                _scannedSensors.FirstOrDefault()?.SupplierBoxId;
            if (!supplierBoxId.HasValue || _currentProduct == null)
                return false;

            var supplierBox = _unitOfWork.SupplierBoxes
                .Find(box => box.SupplierBoxId == supplierBoxId.Value)
                .FirstOrDefault();
            if (supplierBox == null || supplierBox.ProductId != _currentProduct.ProductId)
                return false;

            _currentSupplierBox = supplierBox;
            var sensorsFromCurrentSupplier = _scannedSensors.Count(
                sensor => sensor.SupplierBoxId == supplierBox.SupplierBoxId);
            _runtimeSupplierBoxRemaining = Math.Max(
                0,
                supplierBox.QtyRemaining - sensorsFromCurrentSupplier);

            txtLogisticUniqueNumber.Text = $"S{supplierBox.UniqueNumber}";
            txtStartPartNumber.Text = $"P{_currentProduct.StartPartNumber}";
            txtQtySupplied.Text = $"Q{_runtimeSupplierBoxRemaining}";
            txtLogisticUniqueNumber.Enabled = false;
            txtStartPartNumber.Enabled = false;
            txtQtySupplied.Enabled = false;
            btnLogisticLabelNok.Enabled = false;
            btnLogisticLabelOk.Enabled = false;

            txtComponentSerial.Enabled = true;
            listBoxReadedSensors.Enabled = true;
            btnForceChangeSupplierBox.Enabled = true;
            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.ForeColor = Color.Black;
            lblCheckResult.Text = "PROCESSO RESTAURADO AUTOMATICAMENTE\nESCANEIE O PRÓXIMO SENSOR";
            txtComponentSerial.Focus();
            UpdateContinueProcessButton();
            return true;
        }

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

        private async void txtWorkOrderNumber_Leave(object sender, EventArgs e)
        {
            btnWorkOrderNok.Enabled = true;

            var raw = (txtWorkOrderNumber.Text ?? string.Empty).Trim().ToUpperInvariant();
            if (_accPartTypeDataInProgress ||
                !WorkOrderRules.TryNormalizeScannedLabel(raw, out var workOrderNumber))
                return;

            if (!await TryLoadAccPartTypeDataAsync(workOrderNumber))
            {
                txtWorkOrderNumber.Enabled = true;
                txtWorkOrderNumber.SelectAll();
                txtWorkOrderNumber.Focus();
                return;
            }

            if (_currentProduct == null)
            {
                txtWorkOrderMaterialNumber.Enabled = true;
                txtWorkOrderMaterialNumber.Focus();
            }
            else
            {
                cbWorkOrderQtyToSend.Enabled = true;
                btnWorkOrderOk.Enabled = cbWorkOrderQtyToSend.SelectedItem != null;
                cbWorkOrderQtyToSend.Focus();
            }
        }

        private void StartContinueProcess(ZfBox zfBox)
        {
            var workOrder = _unitOfWork.SapWorkOrders.GetById(zfBox.SapWorkOrderId);
            var product = _unitOfWork.Products.GetById(zfBox.ProductId);

            if (workOrder == null || product == null)
            {
                MessageBox.Show("Dados da caixa não encontrados no banco.", "CONTINUAR PROCESSO",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _currentWorkOrder = workOrder;
            _currentProduct = product;
            _currentSupplierBox = null;
            _currentZfBox = zfBox;
            _sensorLimit = zfBox.QtyToSend;
            _lockedBoxProductId = product.ProductId;
            _lockedBoxPartNumber = product.StartPartNumber;

            _scannedSensors.Clear();
            listBoxReadedSensors.Items.Clear();

            var existingSensors = _unitOfWork.Sensors
                .Find(s => s.ZfBoxId == zfBox.ZfBoxId && s.InProgress)
                .OrderByDescending(s => s.ScannedTime)
                .ToList();

            _scannedSensors.AddRange(existingSensors);
            _pendingAccSensor = existingSensors
                .FirstOrDefault(sensor => sensor.AccState == SensorAccState.Loaded && !sensor.IsScrap);
            _sensorCounter = _scannedSensors.Count(sensor => !sensor.IsScrap);
            lblComponentQty.Text = $"{_sensorCounter:D3}/{_sensorLimit:D3}";

            foreach (var sensor in existingSensors)
                listBoxReadedSensors.Items.Add(sensor.SerialNumber);

            if (listBoxReadedSensors.Items.Count > 0)
                listBoxReadedSensors.SelectedIndex = 0;

            UpdateScrapButtonState();

            txtWorkOrderNumber.Text = $"O{workOrder.WorkOrderNumber}";
            txtWorkOrderMaterialNumber.Text = $"P{product.EndPartNumber}";
            _suppressQtyToSendChange = true;
            cbWorkOrderQtyToSend.Text = _sensorLimit.ToString();
            _suppressQtyToSendChange = false;

            txtWorkOrderNumber.Enabled = false;
            txtWorkOrderMaterialNumber.Enabled = false;
            cbWorkOrderQtyToSend.Enabled = false;
            btnWorkOrderNok.Enabled = false;
            btnWorkOrderOk.Enabled = false;

            txtLogisticUniqueNumber.Enabled = true;
            txtStartPartNumber.Enabled = false;
            txtQtySupplied.Enabled = false;
            btnLogisticLabelNok.Enabled = true;
            btnLogisticLabelOk.Enabled = false;

            txtComponentSerial.Enabled = false;
            listBoxReadedSensors.Enabled = true;
            btnForceChangeSupplierBox.Enabled = false;

            txtLogisticUniqueNumber.Clear();
            txtStartPartNumber.Clear();
            txtQtySupplied.Clear();
            txtComponentSerial.Clear();

            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.ForeColor = Color.Black;
            lblCheckResult.Text = "LEIA A ETIQUETA DA LOGÍSTICA PARA CONTINUAR";

            _forcingSupplierBoxChange = false;
            _previousSupplierBoxId = 0;
            _previousSupplierBoxUniqueNumber = string.Empty;
            _allowSupplierBoxOverdraw = false;
            _overdrawLogged = false;
            _runtimeSupplierBoxRemaining = 0;

            txtLogisticUniqueNumber.Focus();
            UpdateContinueProcessButton();
        }

        // ----------------------------
        // UI helpers
        // ----------------------------

        private void ShowWarningAndReset(string message, string title)
        {
            Logging.ApplicationFileLogger.Warning(
                "Validation.ProcessReset",
                message,
                MergeApplicationLogContext(new Dictionary<string, object?>
                {
                    ["DialogTitle"] = title
                }));
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            CleanForm();
        }

        private async Task<(bool Success, long? CycleId, int? UnitPartTypeId)> LoadSensorInAccAsync(
            string serial)
        {
#if DEBUG
            if (_debugAccBypassEnabled)
            {
                lblCheckResult.BackColor = Color.Green;
                lblCheckResult.ForeColor = Color.White;
                lblCheckResult.Text = $"LOAD OK - DEBUG\n{serial}";
                txtComponentSerial.Enabled = true;

                AddLogSafe(
                    "ACC sensor Load bypassed in Debug mode. " +
                    $"Serial={serial}, WorkOrderNumber={_currentWorkOrder?.WorkOrderNumber}",
                    Logging.ApplicationLogLevel.Warning,
                    "Debug.AccSensorLoadBypassed");
                return (true, null, null);
            }
#endif

            if (_currentAccPartTypeId == null)
            {
                ShowWorkOrderAccFailure("PARTTYPEID NÃO CARREGADO");
                return (false, null, null);
            }

            txtComponentSerial.Enabled = false;
            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.ForeColor = Color.Black;
            lblCheckResult.Text = $"EXECUTANDO LOAD NO ACC\n{serial}";

            try
            {
                var accResult = await Task.Run(() =>
                {
                    var serials = new[] { serial };
                    using var client = ZF.ACCComm.Client.ACCCommClient.Connect(
                        ZF.ACCComm.Utils.NetworkUtils.GetEndpoint(_accSettings.IpAddress, _accSettings.Port));

                    var load = client.Load(
                        _accSettings.Station,
                        _accSettings.ProductType,
                        _accSettings.DllVersion,
                        _currentAccPartTypeId.Value,
                        serials,
                        null);

                    return (load.CycleID, load.UnitPartTypeID, load.StatusBits, load.IsRework);
                });

                lblCheckResult.BackColor = Color.Green;
                lblCheckResult.ForeColor = Color.White;
                lblCheckResult.Text = $"LOAD OK\n{serial}";
                txtComponentSerial.Enabled = true;

                AddLogSafe(
                    "ACC sensor Load completed; Unload is pending. " +
                    $"Serial={serial}, PartTypeID={_currentAccPartTypeId}, " +
                    $"CycleID={accResult.CycleID}, StatusBits={accResult.StatusBits}, " +
                    $"UnitPartTypeID={accResult.UnitPartTypeID}, IsRework={accResult.IsRework}",
                    eventName: "ACC.SensorLoadCompleted");
                return (
                    true,
                    checked((long?)accResult.CycleID),
                    accResult.UnitPartTypeID);
            }
            catch (Exception ex)
            {
                ShowAccFailure(ex.Message);
                AddLogSafe(
                    "ACC sensor Load failed. " +
                    $"Serial={serial}, PartTypeID={_currentAccPartTypeId}, Error={ex.Message}",
                    Logging.ApplicationLogLevel.Error,
                    "ACC.SensorLoadFailed",
                    ex);
                return (false, null, null);
            }
        }

        private async Task<bool> CompleteSensorInAccAsync(
            Sensor sensor,
            bool approved,
            Operator? scrapOperator = null)
        {
            if (sensor.AccState != SensorAccState.Loaded)
                return true;

            var partTypeId = sensor.AccPartTypeId ?? _currentAccPartTypeId;
            if (!partTypeId.HasValue)
            {
                ShowAccFailure("PARTTYPEID DO SENSOR NÃO ENCONTRADO");
                return false;
            }

            txtComponentSerial.Enabled = false;
            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.ForeColor = Color.Black;
            lblCheckResult.Text = approved
                ? $"EXECUTANDO UNLOAD OK\n{sensor.SerialNumber}"
                : $"EXECUTANDO UNLOAD NOK\n{sensor.SerialNumber}";

            try
            {
                string otherInfo;
#if DEBUG
                if (_debugAccBypassEnabled)
                {
                    otherInfo = "DEBUG ACC BYPASS";
                }
                else
#endif
                {
                    otherInfo = await Task.Run(() =>
                    {
                        using var client = ZF.ACCComm.Client.ACCCommClient.Connect(
                            ZF.ACCComm.Utils.NetworkUtils.GetEndpoint(
                                _accSettings.IpAddress,
                                _accSettings.Port));

                        var unload = client.Unload(
                            station: _accSettings.Station,
                            product: _accSettings.ProductType,
                            version: _accSettings.DllVersion,
                            partTypeID: partTypeId.Value,
                            statusBits: approved ? 1u : 0u,
                            failureBits: approved ? 0u : 1u,
                            components: new[] { sensor.SerialNumber },
                            tagList: null);

                        return unload.OtherInfo ?? string.Empty;
                    });
                }

                sensor.AccState = approved
                    ? SensorAccState.UnloadedOk
                    : SensorAccState.UnloadedNok;
                sensor.AccUnloadTime = DateTime.Now;
                sensor.AccUnloadOtherInfo = otherInfo;

                if (!approved)
                {
                    sensor.IsScrap = true;
                    sensor.ScrappedTime = DateTime.Now;
                    sensor.ScrapOperatorId = scrapOperator?.OperatorId;
                    sensor.ScrapOperatorName = scrapOperator?.Name ??
                                               scrapOperator?.Re ??
                                               _loggedWindowsUser;
                }

                if (!_unitOfWork.Sensors.Edit(sensor, out var editError) ||
                    !_unitOfWork.Commit(out editError))
                {
                    BlockProcessForMaintenance(
                        $"UNLOAD ENVIADO, MAS O BANCO NÃO FOI ATUALIZADO\n{sensor.SerialNumber}");
                    MessageBox.Show(
                        "O Unload foi enviado ao ACC, mas não foi possível persistir o estado local.\n\n" +
                        editError,
                        "ERRO DE PERSISTÊNCIA APÓS UNLOAD",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    AddLogSafe(
                        "ACC Unload succeeded but local persistence failed. " +
                        $"Serial={sensor.SerialNumber}, Approved={approved}, Error={editError}",
                        Logging.ApplicationLogLevel.Critical,
                        "ACC.SensorUnloadPersistenceFailed");
                    return false;
                }

                AddLogSafe(
                    $"ACC sensor Unload {(approved ? "OK" : "NOK")} completed. " +
                    $"Serial={sensor.SerialNumber}, PartTypeID={partTypeId}, " +
                    $"StatusBits={(approved ? 1 : 0)}, FailureBits={(approved ? 0 : 1)}, " +
                    $"OtherInfo={otherInfo}",
                    approved
                        ? Logging.ApplicationLogLevel.Information
                        : Logging.ApplicationLogLevel.Warning,
                    approved ? "ACC.SensorUnloadOkCompleted" : "ACC.SensorUnloadNokCompleted");
                return true;
            }
            catch (Exception ex)
            {
                ShowAccFailure(ex.Message);
                AddLogSafe(
                    $"ACC sensor Unload {(approved ? "OK" : "NOK")} failed. " +
                    $"Serial={sensor.SerialNumber}, PartTypeID={partTypeId}, Error={ex.Message}",
                    Logging.ApplicationLogLevel.Error,
                    approved ? "ACC.SensorUnloadOkFailed" : "ACC.SensorUnloadNokFailed",
                    ex);
                return false;
            }
        }

        private async Task TryCompensateUnpersistedLoadAsync(Sensor sensor)
        {
            try
            {
#if DEBUG
                if (_debugAccBypassEnabled)
                    return;
#endif
                if (!sensor.AccPartTypeId.HasValue)
                    return;

                await Task.Run(() =>
                {
                    using var client = ZF.ACCComm.Client.ACCCommClient.Connect(
                        ZF.ACCComm.Utils.NetworkUtils.GetEndpoint(
                            _accSettings.IpAddress,
                            _accSettings.Port));

                    client.Unload(
                        station: _accSettings.Station,
                        product: _accSettings.ProductType,
                        version: _accSettings.DllVersion,
                        partTypeID: sensor.AccPartTypeId.Value,
                        statusBits: 0u,
                        failureBits: 1u,
                        components: new[] { sensor.SerialNumber },
                        tagList: null);
                });

                AddLogSafe(
                    "ACC Load was compensated with Unload NOK after the local sensor commit failed. " +
                    $"Serial={sensor.SerialNumber}, PartTypeID={sensor.AccPartTypeId}",
                    Logging.ApplicationLogLevel.Warning,
                    "ACC.SensorLoadCompensatedAfterDatabaseFailure");
            }
            catch (Exception ex)
            {
                BlockProcessForMaintenance(
                    $"LOAD SEM REGISTRO LOCAL E FALHA NA COMPENSAÇÃO\n{sensor.SerialNumber}");
                AddLogSafe(
                    "Unable to compensate an ACC Load after the local sensor commit failed. " +
                    $"Serial={sensor.SerialNumber}, Error={ex.Message}",
                    Logging.ApplicationLogLevel.Critical,
                    "ACC.SensorLoadCompensationFailed",
                    ex);
            }
        }

        private async Task ConfirmLastSensorAndFinalizeAsync()
        {
            if (_sensorCounter < _sensorLimit)
                return;

            if (_pendingAccSensor == null)
            {
                OpenFinishedBoxDialog();
                return;
            }

            txtComponentSerial.Enabled = false;
            listBoxReadedSensors.SelectedIndex = 0;
            UpdateScrapButtonState();

            var confirmation = MessageBox.Show(
                $"O último sensor lido foi {_pendingAccSensor.SerialNumber}.\n\n" +
                "Você garante que TODOS os sensores estão seguros dentro da caixa?\n\n" +
                "SIM: envia Unload OK do último sensor e continua para a etiqueta final.\n" +
                "NÃO: mantém o último sensor disponível para marcar como scrap.",
                "CONFIRMAR SENSORES NA CAIXA",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmation != DialogResult.Yes)
            {
                lblCheckResult.BackColor = Color.FromArgb(255, 193, 7);
                lblCheckResult.ForeColor = Color.Black;
                lblCheckResult.Text =
                    $"ÚLTIMO SENSOR PENDENTE\n{_pendingAccSensor.SerialNumber}\n" +
                    "MARQUE COMO SCRAP OU CLIQUE AQUI PARA CONFIRMAR";
                UpdateScrapButtonState();
                return;
            }

            _sensorOperationInProgress = true;
            UpdateContinueProcessButton();
            try
            {
                if (!await CompleteSensorInAccAsync(_pendingAccSensor, approved: true))
                    return;

                _pendingAccSensor = null;
                btnRemoveSensor.Enabled = false;
                OpenFinishedBoxDialog();
            }
            finally
            {
                _sensorOperationInProgress = false;
                UpdateContinueProcessButton();
            }
        }

        private void listBoxReadedSensors_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateScrapButtonState();
        }

        private void UpdateScrapButtonState()
        {
            btnRemoveSensor.Enabled =
                !_criticalProcessBlock &&
                !_sensorOperationInProgress &&
                listBoxReadedSensors.Enabled &&
                listBoxReadedSensors.SelectedIndex == 0 &&
                _pendingAccSensor?.AccState == SensorAccState.Loaded &&
                !string.IsNullOrWhiteSpace(listBoxReadedSensors.SelectedItem?.ToString()) &&
                string.Equals(
                    listBoxReadedSensors.SelectedItem?.ToString(),
                    _pendingAccSensor.SerialNumber,
                    StringComparison.OrdinalIgnoreCase);
        }

        private void ShowSensorStatusNok(string message)
        {
            _retryTarget = RetryTarget.Sensor;
            Logging.ApplicationFileLogger.Warning(
                "Validation.SensorRejected",
                message,
                BuildApplicationLogContext());
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
            if (_runtimeSupplierBoxRemaining > 0)
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

        private void UpdateContinueProcessButton()
        {
            btnContinueProcess.Enabled = _currentWorkOrder == null && txtWorkOrderNumber.Enabled;
            btnInterruptProcess.Enabled =
                !_criticalProcessBlock &&
                !_sensorOperationInProgress &&
                _currentWorkOrder != null &&
                _currentProduct != null &&
                _currentZfBox?.InProgress == true;
            UpdateScrapButtonState();
        }

        private void BlockProcessForMaintenance(string message)
        {
            _criticalProcessBlock = true;
            txtComponentSerial.Enabled = false;
            btnForceChangeSupplierBox.Enabled = false;
            btnRemoveSensor.Enabled = false;
            btnInterruptProcess.Enabled = false;
            lblCheckResult.Enabled = true;
            lblCheckResult.BackColor = Color.DarkRed;
            lblCheckResult.ForeColor = Color.White;
            lblCheckResult.Text = $"NOK CRÍTICO\n{message}\nACIONAR MANUTENÇÃO";
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
