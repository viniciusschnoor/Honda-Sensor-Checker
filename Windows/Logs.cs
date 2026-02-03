using HondaSensorChecker.Data.UnitOfWork;
using System.ComponentModel;

namespace HondaSensorChecker
{
    public partial class Logs : Form
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly BindingSource _bsLogs = new BindingSource();

        public Logs(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            InitializeComponent();
        }

        private void Logs_Load(object sender, EventArgs e)
        {
            var operators = _unitOfWork.Operators
                .GetAll()
                .ToDictionary(o => o.OperatorId, o => o.Name ?? o.ZfId ?? o.Re ?? string.Empty);

            var items = _unitOfWork.Logs
                .GetAll()
                .OrderByDescending(l => l.Data)
                .Select(l => new LogEntry
                {
                    Data = l.Data,
                    Operator = operators.TryGetValue(l.OperatorId, out var op) ? op : l.OperatorId.ToString(),
                    Description = l.Description
                })
                .ToList();

            _bsLogs.DataSource = new BindingList<LogEntry>(items);
            dgvLogs.AutoGenerateColumns = true;
            dgvLogs.DataSource = _bsLogs;
            dgvLogs.ReadOnly = true;
            dgvLogs.AllowUserToAddRows = false;
            dgvLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private sealed class LogEntry
        {
            public DateTime Data { get; set; }
            public string Operator { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
        }
    }
}
