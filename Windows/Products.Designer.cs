namespace HondaSensorChecker
{
    partial class Products
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
            label1 = new Label();
            btnAdd = new Button();
            label2 = new Label();
            txtPrefix = new TextBox();
            label3 = new Label();
            txtPn = new TextBox();
            label4 = new Label();
            txtElsen = new TextBox();
            dgvSensors = new DataGridView();
            btnRemove = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvSensors).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            label1.Font = new Font("Segoe UI Semibold", 20.25F, FontStyle.Bold);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(995, 46);
            label1.TabIndex = 2;
            label1.Text = "Sensores";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnAdd
            // 
            btnAdd.Font = new Font("Segoe UI", 12F);
            btnAdd.Location = new Point(734, 58);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(133, 29);
            btnAdd.TabIndex = 3;
            btnAdd.Text = "ADICIONAR";
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += btnAdd_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 61);
            label2.Name = "label2";
            label2.Size = new Size(58, 21);
            label2.TabIndex = 4;
            label2.Text = "PREFIX";
            // 
            // txtPrefix
            // 
            txtPrefix.Font = new Font("Segoe UI", 12F);
            txtPrefix.Location = new Point(76, 58);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.Size = new Size(100, 29);
            txtPrefix.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F);
            label3.Location = new Point(182, 61);
            label3.Name = "label3";
            label3.Size = new Size(130, 21);
            label3.TabIndex = 6;
            label3.Text = "PARTNUMBER ZF";
            // 
            // txtPn
            // 
            txtPn.Font = new Font("Segoe UI", 12F);
            txtPn.Location = new Point(318, 58);
            txtPn.Name = "txtPn";
            txtPn.Size = new Size(142, 29);
            txtPn.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F);
            label4.Location = new Point(466, 61);
            label4.Name = "label4";
            label4.Size = new Size(114, 21);
            label4.TabIndex = 8;
            label4.Text = "ELSEN/ELMOD";
            // 
            // txtElsen
            // 
            txtElsen.Font = new Font("Segoe UI", 12F);
            txtElsen.Location = new Point(586, 58);
            txtElsen.Name = "txtElsen";
            txtElsen.Size = new Size(142, 29);
            txtElsen.TabIndex = 9;
            // 
            // dgvSensors
            // 
            dgvSensors.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvSensors.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvSensors.Location = new Point(12, 93);
            dgvSensors.Name = "dgvSensors";
            dgvSensors.Size = new Size(995, 345);
            dgvSensors.TabIndex = 10;
            dgvSensors.CellEndEdit += dgvSensors_CellEndEdit;
            // 
            // btnRemove
            // 
            btnRemove.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRemove.Font = new Font("Segoe UI", 12F);
            btnRemove.Location = new Point(874, 57);
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(133, 29);
            btnRemove.TabIndex = 11;
            btnRemove.Text = "REMOVER";
            btnRemove.UseVisualStyleBackColor = true;
            btnRemove.Click += btnRemove_Click;
            // 
            // Products
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1019, 450);
            Controls.Add(btnRemove);
            Controls.Add(dgvSensors);
            Controls.Add(txtElsen);
            Controls.Add(label4);
            Controls.Add(txtPn);
            Controls.Add(label3);
            Controls.Add(txtPrefix);
            Controls.Add(label2);
            Controls.Add(btnAdd);
            Controls.Add(label1);
            MinimizeBox = false;
            MinimumSize = new Size(1035, 489);
            Name = "Products";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Products";
            Load += Products_Load;
            ((System.ComponentModel.ISupportInitialize)dgvSensors).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button btnAdd;
        private Label label2;
        private TextBox txtPrefix;
        private Label label3;
        private TextBox txtPn;
        private Label label4;
        private TextBox txtElsen;
        private DataGridView dgvSensors;
        private Button btnRemove;
    }
}