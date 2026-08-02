using System.Drawing;
using System.Windows.Forms;

namespace HondaSensorChecker
{
    partial class ContinueProcessDialog
    {
        private System.ComponentModel.IContainer components = null;

        private ListBox listBox;
        private Panel buttonPanel;
        private Button btnCancel;
        private Button btnOk;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            this.listBox = new ListBox();
            this.buttonPanel = new Panel();
            this.btnCancel = new Button();
            this.btnOk = new Button();

            // Form
            this.Text = "Continuar processo anterior";
            this.ClientSize = new Size(640, 360);
            this.StartPosition = FormStartPosition.CenterParent;

            // listBox
            this.listBox.Dock = DockStyle.Fill;
            this.listBox.DisplayMember = "Display";
            this.listBox.SelectedIndexChanged += new System.EventHandler(this.ListBox_SelectedIndexChanged);

            // buttonPanel
            this.buttonPanel.Dock = DockStyle.Bottom;
            this.buttonPanel.Height = 40;

            // btnCancel
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.Dock = DockStyle.Right;
            this.btnCancel.Width = 100;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);

            // btnOk
            this.btnOk.Text = "Continuar";
            this.btnOk.Dock = DockStyle.Right;
            this.btnOk.Width = 100;
            this.btnOk.Enabled = false;
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);

            // Add buttons (ordem importa com DockStyle.Right)
            this.buttonPanel.Controls.Add(this.btnCancel);
            this.buttonPanel.Controls.Add(this.btnOk);

            // Add controls
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.buttonPanel);
        }
    }
}
