using HondaSensorChecker.Data.UnitOfWork;
using System.ComponentModel;

namespace HondaSensorChecker
{
    public partial class Logs : Form
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly BindingSource _bsLogs = new BindingSource();
        private List<LogEntry> _allLogs = new();
        private bool _isInitializing;

        public Logs(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            InitializeComponent();
        }

        private void Logs_Load(object sender, EventArgs e)
        {
            _isInitializing = true;

            var operators = _unitOfWork.Operators
                .GetAll()
                .ToDictionary(o => o.OperatorId, o => o.Name ?? o.ZfId ?? o.Re ?? string.Empty);

            _allLogs = _unitOfWork.Logs
                .GetAll()
                .OrderByDescending(l => l.Data)
                .Select(l => new LogEntry
                {
                    Data = l.Data,
                    Operator = operators.TryGetValue(l.OperatorId, out var op) ? op : l.OperatorId.ToString(),
                    Description = l.Description ?? string.Empty
                })
                .ToList();

            cboOperator.Items.Add("Todos os operadores");
            cboOperator.Items.AddRange(_allLogs
                .Select(l => l.Operator)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .OrderBy(o => o)
                .Cast<object>()
                .ToArray());

            cboPeriod.Items.AddRange(new object[]
            {
                "Todo o período",
                "Hoje",
                "Últimos 7 dias",
                "Últimos 30 dias"
            });

            cboOperator.SelectedIndex = 0;
            cboPeriod.SelectedIndex = 0;
            dgvLogs.AutoGenerateColumns = false;
            dgvLogs.DataSource = _bsLogs;
            _isInitializing = false;
            ApplyFilters();
        }

        private void FilterChanged(object? sender, EventArgs e)
        {
            if (!_isInitializing)
                ApplyFilters();
        }

        private void ApplyFilters()
        {
            var search = txtSearch.Text.Trim();
            var selectedOperator = cboOperator.SelectedItem?.ToString();
            var minimumDate = cboPeriod.SelectedIndex switch
            {
                1 => DateTime.Today,
                2 => DateTime.Today.AddDays(-6),
                3 => DateTime.Today.AddDays(-29),
                _ => DateTime.MinValue
            };

            var filtered = _allLogs
                .Where(log => log.Data >= minimumDate)
                .Where(log => selectedOperator == "Todos os operadores" ||
                              string.Equals(log.Operator, selectedOperator, StringComparison.CurrentCultureIgnoreCase))
                .Where(log => string.IsNullOrWhiteSpace(search) ||
                              log.Operator.Contains(search, StringComparison.CurrentCultureIgnoreCase) ||
                              log.Description.Contains(search, StringComparison.CurrentCultureIgnoreCase))
                .ToList();

            _bsLogs.DataSource = new BindingList<LogEntry>(filtered);
            lblCount.Text = filtered.Count == 1 ? "1 registro" : $"{filtered.Count:N0} registros";

            if (filtered.Count == 0)
                ClearDetails();
        }

        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            _isInitializing = true;
            txtSearch.Clear();
            cboOperator.SelectedIndex = 0;
            cboPeriod.SelectedIndex = 0;
            _isInitializing = false;
            ApplyFilters();
            txtSearch.Focus();
        }

        private void dgvLogs_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLogs.CurrentRow?.DataBoundItem is not LogEntry selected)
            {
                ClearDetails();
                return;
            }

            lblDetailsDate.Text = $"Data: {selected.Data:dd/MM/yyyy HH:mm:ss}";
            lblDetailsOperator.Text = $"Operador: {selected.Operator}";
            txtDetails.Text = selected.Description;
        }

        private void ClearDetails()
        {
            lblDetailsDate.Text = "Data: —";
            lblDetailsOperator.Text = "Operador: —";
            txtDetails.Clear();
        }

        private sealed class LogEntry
        {
            public DateTime Data { get; set; }
            public string Operator { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
    }
}
