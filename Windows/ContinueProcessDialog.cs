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
<<<<<<< HEAD
=======

            foreach (var option in options)
                _listBox.Items.Add(option);

            if (_listBox.Items.Count > 0)
                _listBox.SelectedIndex = 0;
>>>>>>> 4d73f24c0c9211149f55320542b8b65572e153e0
        }

        // ✅ Runtime
        public ContinueProcessDialog(List<ContinueProcessOption> options) : this()
        {
            SetOptions(options);
        }

<<<<<<< HEAD
        public void SetOptions(IEnumerable<ContinueProcessOption> options)
=======
        private void BtnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void BtnOk_Click(object sender, System.EventArgs e)
>>>>>>> 4d73f24c0c9211149f55320542b8b65572e153e0
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
