using HondaSensorChecker.Data.UnitOfWork;
using HondaSensorChecker.Models;
using System.ComponentModel;

namespace HondaSensorChecker
{
    public partial class Users : Form
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly int _operatorId; // logged operator id (who is performing the change)

        // ✅ BindingSource para evitar reentrância do DataGridView
        private readonly BindingSource _bsUsers = new BindingSource();

        // Evita refresh concorrente
        private bool _refreshingGrid = false;

        public Users(IUnitOfWork unitOfWork, int operatorId)
        {
            _unitOfWork = unitOfWork;
            _operatorId = operatorId;
            InitializeComponent();
        }

        private BindingList<Operator> LoadOperators()
        {
            // BindingList ajuda o DataGridView a trabalhar melhor com edição
            var list = _unitOfWork.Operators.GetAll().ToList();
            return new BindingList<Operator>(list);
        }

        private void Users_Load(object sender, EventArgs e)
        {
            _bsUsers.DataSource = LoadOperators();
            dgvUsers.AutoGenerateColumns = true;
            dgvUsers.DataSource = _bsUsers;

            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.MultiSelect = false;
            dgvUsers.AllowUserToAddRows = false;

            dgvUsers.Columns[0].HeaderText = "ID";
            dgvUsers.Columns[1].HeaderText = "RE";
            dgvUsers.Columns[2].HeaderText = "ZF-ID";
            dgvUsers.Columns[3].HeaderText = "NAME";
            dgvUsers.Columns[4].HeaderText = "ADMIN";

            dgvUsers.Columns[0].ReadOnly = true;
        }

        private void RefreshUsersGridSafe()
        {
            if (_refreshingGrid) return;
            _refreshingGrid = true;

            // ✅ Não rebinda o grid durante evento interno: joga pra fila do UI
            BeginInvoke(new Action(() =>
            {
                try
                {
                    // Finaliza edição antes de trocar fonte
                    dgvUsers.EndEdit();
                    dgvUsers.CommitEdit(DataGridViewDataErrorContexts.Commit);

                    _bsUsers.DataSource = LoadOperators();
                    _bsUsers.ResetBindings(false);
                }
                finally
                {
                    _refreshingGrid = false;
                }
            }));
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtRe.Text) ||
                string.IsNullOrWhiteSpace(txtZfId.Text) ||
                string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("FILL ALL REQUIRED FIELDS", "USER", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearInputs();
                return;
            }

            var newOperator = new Operator
            {
                Re = txtRe.Text.Trim().ToUpper(),
                ZfId = txtZfId.Text.Trim().ToUpper(),
                Name = txtNome.Text.Trim().ToUpper(),
                Admin = checkBoxAdmin.Checked
            };

            var hasSameRe = _unitOfWork.Operators.Find(o => o.Re == newOperator.Re).Any();
            var hasSameZfId = _unitOfWork.Operators.Find(o => o.ZfId == newOperator.ZfId).Any();

            if (hasSameRe || hasSameZfId)
            {
                MessageBox.Show("USER ALREADY EXISTS (RE or ZF-ID)", "USER", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ClearInputs();
                return;
            }

            if (!_unitOfWork.Operators.Add(newOperator, out var addError))
            {
                MessageBox.Show(addError, "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!_unitOfWork.Commit(out var commitError))
            {
                MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TryAddLogSafe($"Operator created. NewOperatorId={newOperator.OperatorId}, Re={newOperator.Re}, ZfId={newOperator.ZfId}, Admin={newOperator.Admin}");

            RefreshUsersGridSafe();
            ClearInputs();
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count == 0)
                return;

            var id = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells[0].Value);

            if (id == _operatorId)
            {
                MessageBox.Show("You cannot delete the currently logged user.", "USER", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_unitOfWork.Operators.Remove(id, out var removeError))
            {
                MessageBox.Show(removeError, "Delete error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!_unitOfWork.Commit(out var commitError))
            {
                if (IsFkViolation(commitError))
                {
                    MessageBox.Show(
                        "Não é possível remover este usuário porque existem registros vinculados.\n" +
                        "Exemplo: sensores, caixas ou logs.",
                        "Delete blocked",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            TryAddLogSafe($"Operator deleted. OperatorId={id}");

            RefreshUsersGridSafe();
        }

        private void dgvUsers_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_refreshingGrid) return;
            if (e.RowIndex < 0) return;

            // Finaliza a edição antes de qualquer coisa
            dgvUsers.EndEdit();
            dgvUsers.CommitEdit(DataGridViewDataErrorContexts.Commit);

            if (dgvUsers.Rows[e.RowIndex].DataBoundItem is not Operator op)
                return;

            op.Re = (op.Re ?? string.Empty).Trim().ToUpper();
            op.ZfId = (op.ZfId ?? string.Empty).Trim().ToUpper();
            op.Name = (op.Name ?? string.Empty).Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(op.Re) ||
                string.IsNullOrWhiteSpace(op.ZfId) ||
                string.IsNullOrWhiteSpace(op.Name))
            {
                MessageBox.Show("FILL ALL REQUIRED FIELDS", "USER", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshUsersGridSafe();
                return;
            }

            var duplicateRe = _unitOfWork.Operators.Find(o => o.Re == op.Re && o.OperatorId != op.OperatorId).Any();
            var duplicateZf = _unitOfWork.Operators.Find(o => o.ZfId == op.ZfId && o.OperatorId != op.OperatorId).Any();
            if (duplicateRe || duplicateZf)
            {
                MessageBox.Show("USER ALREADY EXISTS (RE or ZF-ID)", "USER", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshUsersGridSafe();
                return;
            }

            if (!_unitOfWork.Operators.Edit(op, out var editError))
            {
                MessageBox.Show(editError, "Update error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RefreshUsersGridSafe();
                return;
            }

            if (!_unitOfWork.Commit(out var commitError))
            {
                if (IsFkViolation(commitError))
                {
                    MessageBox.Show(
                        "Não foi possível salvar a alteração.\n" +
                        "Este usuário já possui vínculos no sistema e alguma mudança gerou conflito de integridade (FK).",
                        "Update blocked",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(commitError, "Commit error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                RefreshUsersGridSafe();
                return;
            }

            TryAddLogSafe($"Operator updated. OperatorId={op.OperatorId}");

            RefreshUsersGridSafe();
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

        private bool IsFkViolation(string error)
        {
            if (string.IsNullOrWhiteSpace(error)) return false;

            return error.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase) ||
                   error.Contains("FOREIGN_KEY_VIOLATION", StringComparison.OrdinalIgnoreCase) ||
                   error.Contains("FOREIGN KEY constraint failed", StringComparison.OrdinalIgnoreCase);
        }

        private void ClearInputs()
        {
            txtRe.Text = string.Empty;
            txtZfId.Text = string.Empty;
            txtNome.Text = string.Empty;
            checkBoxAdmin.Checked = false;
            txtRe.Focus();
        }
    }
}
