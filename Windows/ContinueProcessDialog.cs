using HondaSensorChecker.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HondaSensorChecker
{
    public partial class ContinueProcessDialog : Form
    {
        public ZfBox? SelectedZfBox { get; private set; }

        // ✅ Necessário para o Designer
        public ContinueProcessDialog()
        {
            InitializeComponent();
        }

        // ✅ Runtime
        public ContinueProcessDialog(List<ContinueProcessOption> options) : this()
        {
            SetOptions(options);
        }

        public void SetOptions(IEnumerable<ContinueProcessOption> options)
        {
            listBox.Items.Clear();

            if (options != null)
            {
                foreach (var option in options)
                    listBox.Items.Add(option);
            }

            // mantém mesma UX: seleciona o primeiro se existir
            if (listBox.Items.Count > 0)
                listBox.SelectedIndex = 0;
            else
                btnOk.Enabled = false;
        }

        private void ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = listBox.SelectedItem != null;
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (listBox.SelectedItem is ContinueProcessOption option)
            {
                SelectedZfBox = option.ZfBox;
                DialogResult = DialogResult.OK;
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }

    public class ContinueProcessOption
    {
        public string Display { get; set; } = string.Empty;
        public ZfBox ZfBox { get; set; } = new ZfBox();
    }
}
