using HondaSensorChecker.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HondaSensorChecker
{
    public class ContinueProcessDialog : Form
    {
        private readonly ListBox _listBox;
        private readonly Button _btnOk;

        public ZfBox? SelectedZfBox { get; private set; }

        public ContinueProcessDialog(List<ContinueProcessOption> options)
        {
            Text = "Continuar processo anterior";
            Size = new Size(640, 360);
            StartPosition = FormStartPosition.CenterParent;

            _listBox = new ListBox
            {
                Dock = DockStyle.Fill,
                DisplayMember = nameof(ContinueProcessOption.Display)
            };
            _listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;

            foreach (var option in options)
                _listBox.Items.Add(option);

            var btnCancel = new Button
            {
                Text = "Cancelar",
                Dock = DockStyle.Right,
                Width = 100
            };
            btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;

            _btnOk = new Button
            {
                Text = "Continuar",
                Dock = DockStyle.Right,
                Width = 100,
                Enabled = false
            };
            _btnOk.Click += BtnOk_Click;

            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40
            };
            buttonPanel.Controls.Add(btnCancel);
            buttonPanel.Controls.Add(_btnOk);

            Controls.Add(_listBox);
            Controls.Add(buttonPanel);

            if (_listBox.Items.Count > 0)
                _listBox.SelectedIndex = 0;
        }

        private void ListBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            _btnOk.Enabled = _listBox.SelectedItem != null;
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
        {
            if (_listBox.SelectedItem is ContinueProcessOption option)
            {
                SelectedZfBox = option.ZfBox;
                DialogResult = DialogResult.OK;
            }
        }
    }

    public class ContinueProcessOption
    {
        public string Display { get; set; } = string.Empty;
        public ZfBox ZfBox { get; set; } = new ZfBox();
    }
}
