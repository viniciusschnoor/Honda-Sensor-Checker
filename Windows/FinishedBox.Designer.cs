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
            pnlHeader = new Panel(); pnlContent = new Panel(); txtUniqueNumber = new TextBox(); txtMaterialNumber = new TextBox();
            txtWorkOrder = new TextBox(); txtBatch = new TextBox(); label1 = new Label(); lblSubtitle = new Label(); label2 = new Label();
            label3 = new Label(); label4 = new Label(); label5 = new Label(); button1 = new Button();
            pnlHeader.SuspendLayout(); pnlContent.SuspendLayout(); SuspendLayout();

            pnlHeader.BackColor = Color.FromArgb(25, 126, 75); pnlHeader.Dock = DockStyle.Top; pnlHeader.Height = 105; pnlHeader.Controls.AddRange(new Control[] { label1, lblSubtitle });
            label1.AutoSize = true; label1.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold); label1.ForeColor = Color.White; label1.Location = new Point(26, 15); label1.Text = "Caixa finalizada";
            lblSubtitle.AutoSize = true; lblSubtitle.ForeColor = Color.FromArgb(220, 244, 231); lblSubtitle.Location = new Point(31, 64); lblSubtitle.Text = "Confira a etiqueta final lendo os campos na sequência indicada";

            pnlContent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; pnlContent.BackColor = Color.White;
            pnlContent.Location = new Point(24, 127); pnlContent.Size = new Size(552, 445); pnlContent.Controls.AddRange(new Control[] { label2, txtUniqueNumber, label3, txtMaterialNumber, label4, txtWorkOrder, label5, txtBatch, button1 });
            ConfigureLabel(label2, "1   NÚMERO ÚNICO", 24, 20); ConfigureInput(txtUniqueNumber, 24, 45); txtUniqueNumber.KeyPress += txtUniqueNumber_KeyPress;
            ConfigureLabel(label3, "2   NÚMERO MATERIAL", 24, 104); ConfigureInput(txtMaterialNumber, 24, 129); txtMaterialNumber.Enabled = false; txtMaterialNumber.KeyPress += txtMaterialNumber_KeyPress;
            ConfigureLabel(label4, "3   NÚMERO DA ORDEM", 24, 188); ConfigureInput(txtWorkOrder, 24, 213); txtWorkOrder.Enabled = false; txtWorkOrder.KeyPress += txtWorkOrder_KeyPress;
            ConfigureLabel(label5, "4   LOTE", 24, 272); ConfigureInput(txtBatch, 24, 297); txtBatch.Enabled = false; txtBatch.KeyPress += txtBatch_KeyPress;
            button1.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom; button1.Enabled = false; button1.Location = new Point(24, 376); button1.Size = new Size(504, 44);
            button1.Text = "FINALIZAÇÃO AUTOMÁTICA APÓS A ÚLTIMA LEITURA";

            AutoScaleDimensions = new SizeF(7F, 15F); AutoScaleMode = AutoScaleMode.Font; BackColor = Color.FromArgb(237, 242, 239); ClientSize = new Size(600, 600);
            Controls.Add(pnlContent); Controls.Add(pnlHeader); FormBorderStyle = FormBorderStyle.None; MinimizeBox = false; Name = "FinishedBox";
            StartPosition = FormStartPosition.CenterScreen; Text = "Finalizar caixa";
            pnlHeader.ResumeLayout(false); pnlHeader.PerformLayout(); pnlContent.ResumeLayout(false); pnlContent.PerformLayout(); ResumeLayout(false);
        }

        private static void ConfigureLabel(Label label, string text, int x, int y)
        {
            label.AutoSize = true; label.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold); label.ForeColor = Color.FromArgb(65, 78, 70);
            label.Location = new Point(x, y); label.Text = text;
        }

        private static void ConfigureInput(TextBox input, int x, int y)
        {
            input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right; input.BorderStyle = BorderStyle.FixedSingle;
            input.Font = new Font("Segoe UI", 16F); input.Location = new Point(x, y); input.Size = new Size(504, 36); input.TextAlign = HorizontalAlignment.Center;
        }
    }
}
