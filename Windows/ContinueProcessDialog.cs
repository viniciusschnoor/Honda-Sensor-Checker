using HondaSensorChecker.Models;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HondaSensorChecker
{
    public partial class ContinueProcessDialog : Form
    {
        public ZfBox? SelectedZfBox { get; private set; }

        public ContinueProcessDialog(List<ContinueProcessOption> options)
        {
            InitializeComponent();

            foreach (var option in options)
                _listBox.Items.Add(option);

            if (_listBox.Items.Count > 0)
                _listBox.SelectedIndex = 0;
        }

        private void ListBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            _btnOk.Enabled = _listBox.SelectedItem != null;
        }

        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
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
