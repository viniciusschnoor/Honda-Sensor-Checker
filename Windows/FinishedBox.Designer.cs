namespace HondaSensorChecker
{
    partial class FinishedBox
    {
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
            txtUniqueNumber = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            txtMaterialNumber = new TextBox();
            label4 = new Label();
            txtWorkOrder = new TextBox();
            label5 = new Label();
            txtBatch = new TextBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // txtUniqueNumber
            // 
            txtUniqueNumber.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtUniqueNumber.Font = new Font("Segoe UI", 18F);
            txtUniqueNumber.Location = new Point(12, 147);
            txtUniqueNumber.Name = "txtUniqueNumber";
            txtUniqueNumber.Size = new Size(576, 39);
            txtUniqueNumber.TabIndex = 1;
            txtUniqueNumber.TextAlign = HorizontalAlignment.Center;
            txtUniqueNumber.KeyPress += txtUniqueNumber_KeyPress;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(576, 75);
            label1.TabIndex = 2;
            label1.Text = "CAIXA FINALIZADA";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label2.ForeColor = Color.White;
            label2.Location = new Point(12, 123);
            label2.Name = "label2";
            label2.Size = new Size(122, 21);
            label2.TabIndex = 3;
            label2.Text = "Número Único";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label3.ForeColor = Color.White;
            label3.Location = new Point(12, 209);
            label3.Name = "label3";
            label3.Size = new Size(141, 21);
            label3.TabIndex = 5;
            label3.Text = "Número Material";
            // 
            // txtMaterialNumber
            // 
            txtMaterialNumber.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtMaterialNumber.Enabled = false;
            txtMaterialNumber.Font = new Font("Segoe UI", 18F);
            txtMaterialNumber.Location = new Point(12, 233);
            txtMaterialNumber.Name = "txtMaterialNumber";
            txtMaterialNumber.Size = new Size(576, 39);
            txtMaterialNumber.TabIndex = 4;
            txtMaterialNumber.TextAlign = HorizontalAlignment.Center;
            txtMaterialNumber.KeyPress += txtMaterialNumber_KeyPress;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label4.ForeColor = Color.White;
            label4.Location = new Point(12, 295);
            label4.Name = "label4";
            label4.Size = new Size(152, 21);
            label4.TabIndex = 7;
            label4.Text = "Número da Ordem";
            // 
            // txtWorkOrder
            // 
            txtWorkOrder.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtWorkOrder.Enabled = false;
            txtWorkOrder.Font = new Font("Segoe UI", 18F);
            txtWorkOrder.Location = new Point(12, 319);
            txtWorkOrder.Name = "txtWorkOrder";
            txtWorkOrder.Size = new Size(576, 39);
            txtWorkOrder.TabIndex = 6;
            txtWorkOrder.TextAlign = HorizontalAlignment.Center;
            txtWorkOrder.KeyPress += txtWorkOrder_KeyPress;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            label5.ForeColor = Color.White;
            label5.Location = new Point(12, 381);
            label5.Name = "label5";
            label5.Size = new Size(43, 21);
            label5.TabIndex = 8;
            label5.Text = "Lote";
            // 
            // txtBatch
            // 
            txtBatch.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtBatch.Enabled = false;
            txtBatch.Font = new Font("Segoe UI", 18F);
            txtBatch.Location = new Point(12, 405);
            txtBatch.Name = "txtBatch";
            txtBatch.Size = new Size(576, 39);
            txtBatch.TabIndex = 9;
            txtBatch.TextAlign = HorizontalAlignment.Center;
            txtBatch.KeyPress += txtBatch_KeyPress;
            // 
            // button1
            // 
            button1.Enabled = false;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button1.ForeColor = Color.Green;
            button1.Location = new Point(12, 470);
            button1.Name = "button1";
            button1.Size = new Size(576, 51);
            button1.TabIndex = 10;
            button1.Text = "FINALIZAR";
            button1.UseVisualStyleBackColor = true;
            // 
            // FinishedBox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            BackColor = Color.Green;
            ClientSize = new Size(600, 600);
            Controls.Add(button1);
            Controls.Add(txtBatch);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(txtWorkOrder);
            Controls.Add(label3);
            Controls.Add(txtMaterialNumber);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtUniqueNumber);
            FormBorderStyle = FormBorderStyle.None;
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            Name = "FinishedBox";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FinishedBox";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtUniqueNumber;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox txtMaterialNumber;
        private Label label4;
        private TextBox txtWorkOrder;
        private Label label5;
        private TextBox txtBatch;
        private Button button1;
    }
}