<<<<<<< HEAD
﻿using System.Drawing;
using System.Windows.Forms;

=======
>>>>>>> 4d73f24c0c9211149f55320542b8b65572e153e0
namespace HondaSensorChecker
{
    partial class ContinueProcessDialog
    {
<<<<<<< HEAD
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
=======
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonPanel = new Panel();
            _btnCancel = new Button();
            _btnOk = new Button();
            _listBox = new ListBox();
            buttonPanel.SuspendLayout();
            SuspendLayout();
            // 
            // buttonPanel
            // 
            buttonPanel.Controls.Add(_btnCancel);
            buttonPanel.Controls.Add(_btnOk);
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Location = new Point(0, 320);
            buttonPanel.Name = "buttonPanel";
            buttonPanel.Size = new Size(640, 40);
            buttonPanel.TabIndex = 1;
            // 
            // _btnCancel
            // 
            _btnCancel.Dock = DockStyle.Right;
            _btnCancel.Location = new Point(440, 0);
            _btnCancel.Name = "_btnCancel";
            _btnCancel.Size = new Size(100, 40);
            _btnCancel.TabIndex = 1;
            _btnCancel.Text = "Cancelar";
            _btnCancel.UseVisualStyleBackColor = true;
            _btnCancel.Click += BtnCancel_Click;
            // 
            // _btnOk
            // 
            _btnOk.Dock = DockStyle.Right;
            _btnOk.Enabled = false;
            _btnOk.Location = new Point(540, 0);
            _btnOk.Name = "_btnOk";
            _btnOk.Size = new Size(100, 40);
            _btnOk.TabIndex = 0;
            _btnOk.Text = "Continuar";
            _btnOk.UseVisualStyleBackColor = true;
            _btnOk.Click += BtnOk_Click;
            // 
            // _listBox
            // 
            _listBox.DisplayMember = "Display";
            _listBox.Dock = DockStyle.Fill;
            _listBox.FormattingEnabled = true;
            _listBox.ItemHeight = 15;
            _listBox.Location = new Point(0, 0);
            _listBox.Name = "_listBox";
            _listBox.Size = new Size(640, 320);
            _listBox.TabIndex = 0;
            _listBox.SelectedIndexChanged += ListBox_SelectedIndexChanged;
            // 
            // ContinueProcessDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(640, 360);
            Controls.Add(_listBox);
            Controls.Add(buttonPanel);
            Name = "ContinueProcessDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Continuar processo anterior";
            buttonPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel buttonPanel;
        private Button _btnCancel;
        private Button _btnOk;
        private ListBox _listBox;
>>>>>>> 4d73f24c0c9211149f55320542b8b65572e153e0
    }
}
