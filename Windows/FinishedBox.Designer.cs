namespace HondaSensorChecker
{
    partial class FinishedBox
    {
        private System.ComponentModel.IContainer components = null;
        private Panel pnlHeader, pnlContent;
        private TextBox txtUniqueNumber, txtMaterialNumber, txtWorkOrder, txtBatch;
        private Label label1, lblSubtitle, label2, label3, label4, label5;
        private Button button1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            label1 = new Label();
            lblSubtitle = new Label();
            pnlContent = new Panel();
            label2 = new Label();
            txtUniqueNumber = new TextBox();
            label3 = new Label();
            txtMaterialNumber = new TextBox();
            label4 = new Label();
            txtWorkOrder = new TextBox();
            label5 = new Label();
            txtBatch = new TextBox();
            button1 = new Button();
            pnlHeader.SuspendLayout();
            pnlContent.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(25, 126, 75);
            pnlHeader.Controls.Add(label1);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Margin = new Padding(3, 4, 3, 4);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(686, 140);
            pnlHeader.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(30, 20);
            label1.Name = "label1";
            label1.Size = new Size(304, 54);
            label1.TabIndex = 0;
            label1.Text = "Caixa finalizada";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = Color.FromArgb(220, 244, 231);
            lblSubtitle.Location = new Point(35, 85);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(429, 20);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Confira a etiqueta final lendo os campos na sequência indicada";
            // 
            // pnlContent
            // 
            pnlContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlContent.BackColor = Color.White;
            pnlContent.Controls.Add(label2);
            pnlContent.Controls.Add(txtUniqueNumber);
            pnlContent.Controls.Add(label3);
            pnlContent.Controls.Add(txtMaterialNumber);
            pnlContent.Controls.Add(label4);
            pnlContent.Controls.Add(txtWorkOrder);
            pnlContent.Controls.Add(label5);
            pnlContent.Controls.Add(txtBatch);
            pnlContent.Controls.Add(button1);
            pnlContent.Location = new Point(27, 169);
            pnlContent.Margin = new Padding(3, 4, 3, 4);
            pnlContent.Name = "pnlContent";
            pnlContent.Size = new Size(631, 593);
            pnlContent.TabIndex = 0;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(65, 78, 70);
            label2.Location = new Point(27, 27);
            label2.Name = "label2";
            label2.Size = new Size(158, 20);
            label2.TabIndex = 0;
            label2.Text = "1  NÚMERO ÚNICO";
            // 
            // txtUniqueNumber
            // 
            txtUniqueNumber.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtUniqueNumber.BorderStyle = BorderStyle.FixedSingle;
            txtUniqueNumber.Font = new Font("Segoe UI", 16F);
            txtUniqueNumber.Location = new Point(27, 60);
            txtUniqueNumber.Margin = new Padding(3, 4, 3, 4);
            txtUniqueNumber.Name = "txtUniqueNumber";
            txtUniqueNumber.Size = new Size(576, 43);
            txtUniqueNumber.TabIndex = 1;
            txtUniqueNumber.TextAlign = HorizontalAlignment.Center;
            txtUniqueNumber.KeyPress += txtUniqueNumber_KeyPress;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(65, 78, 70);
            label3.Location = new Point(27, 131);
            label3.Name = "label3";
            label3.Size = new Size(181, 20);
            label3.TabIndex = 2;
            label3.Text = "2  NÚMERO MATERIAL";
            // 
            // txtMaterialNumber
            // 
            txtMaterialNumber.Enabled = false;
            txtMaterialNumber.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtMaterialNumber.BorderStyle = BorderStyle.FixedSingle;
            txtMaterialNumber.Font = new Font("Segoe UI", 16F);
            txtMaterialNumber.Location = new Point(27, 164);
            txtMaterialNumber.Margin = new Padding(3, 4, 3, 4);
            txtMaterialNumber.Name = "txtMaterialNumber";
            txtMaterialNumber.Size = new Size(576, 43);
            txtMaterialNumber.TabIndex = 3;
            txtMaterialNumber.TextAlign = HorizontalAlignment.Center;
            txtMaterialNumber.KeyPress += txtMaterialNumber_KeyPress;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label4.ForeColor = Color.FromArgb(65, 78, 70);
            label4.Location = new Point(27, 235);
            label4.Name = "label4";
            label4.Size = new Size(174, 20);
            label4.TabIndex = 4;
            label4.Text = "3  NÚMERO DA ORDEM";
            // 
            // txtWorkOrder
            // 
            txtWorkOrder.Enabled = false;
            txtWorkOrder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtWorkOrder.BorderStyle = BorderStyle.FixedSingle;
            txtWorkOrder.Font = new Font("Segoe UI", 16F);
            txtWorkOrder.Location = new Point(27, 268);
            txtWorkOrder.Margin = new Padding(3, 4, 3, 4);
            txtWorkOrder.Name = "txtWorkOrder";
            txtWorkOrder.Size = new Size(576, 43);
            txtWorkOrder.TabIndex = 5;
            txtWorkOrder.TextAlign = HorizontalAlignment.Center;
            txtWorkOrder.KeyPress += txtWorkOrder_KeyPress;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            label5.ForeColor = Color.FromArgb(65, 78, 70);
            label5.Location = new Point(27, 339);
            label5.Name = "label5";
            label5.Size = new Size(67, 20);
            label5.TabIndex = 6;
            label5.Text = "4  LOTE";
            // 
            // txtBatch
            // 
            txtBatch.Enabled = false;
            txtBatch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtBatch.BorderStyle = BorderStyle.FixedSingle;
            txtBatch.Font = new Font("Segoe UI", 16F);
            txtBatch.Location = new Point(27, 372);
            txtBatch.Margin = new Padding(3, 4, 3, 4);
            txtBatch.Name = "txtBatch";
            txtBatch.Size = new Size(576, 43);
            txtBatch.TabIndex = 7;
            txtBatch.TextAlign = HorizontalAlignment.Center;
            txtBatch.KeyPress += txtBatch_KeyPress;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            button1.Enabled = false;
            button1.Location = new Point(27, 501);
            button1.Margin = new Padding(3, 4, 3, 4);
            button1.Name = "button1";
            button1.Size = new Size(576, 59);
            button1.TabIndex = 8;
            button1.Text = "FINALIZAÇÃO AUTOMÁTICA APÓS A ÚLTIMA LEITURA";
            // 
            // FinishedBox
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(237, 242, 239);
            ClientSize = new Size(686, 800);
            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            MinimizeBox = false;
            Name = "FinishedBox";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Finalizar caixa";
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            ResumeLayout(false);
        }

    }
}
