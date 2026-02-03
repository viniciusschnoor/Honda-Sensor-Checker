namespace HondaSensorChecker
{
    partial class Users
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
            dgvUsers = new DataGridView();
            txtRe = new TextBox();
            label2 = new Label();
            txtZfId = new TextBox();
            label3 = new Label();
            txtNome = new TextBox();
            label4 = new Label();
            checkBoxAdmin = new CheckBox();
            btnSalvar = new Button();
            btnRemover = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvUsers).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI Semibold", 20.25F, FontStyle.Bold);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(123, 37);
            label1.TabIndex = 1;
            label1.Text = "Usuários";
            // 
            // dgvUsers
            // 
            dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvUsers.Location = new Point(12, 118);
            dgvUsers.Name = "dgvUsers";
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.Size = new Size(765, 291);
            dgvUsers.TabIndex = 2;
            dgvUsers.CellEndEdit += dgvUsers_CellEndEdit;
            // 
            // txtRe
            // 
            txtRe.Location = new Point(12, 89);
            txtRe.Name = "txtRe";
            txtRe.Size = new Size(123, 23);
            txtRe.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 71);
            label2.Name = "label2";
            label2.Size = new Size(20, 15);
            label2.TabIndex = 4;
            label2.Text = "RE";
            // 
            // txtZfId
            // 
            txtZfId.Location = new Point(141, 89);
            txtZfId.Name = "txtZfId";
            txtZfId.Size = new Size(123, 23);
            txtZfId.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(141, 71);
            label3.Name = "label3";
            label3.Size = new Size(103, 15);
            label3.TabIndex = 6;
            label3.Text = "ZF-ID (Z#######)";
            // 
            // txtNome
            // 
            txtNome.Location = new Point(270, 89);
            txtNome.Name = "txtNome";
            txtNome.Size = new Size(270, 23);
            txtNome.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(270, 71);
            label4.Name = "label4";
            label4.Size = new Size(40, 15);
            label4.TabIndex = 8;
            label4.Text = "Nome";
            // 
            // checkBoxAdmin
            // 
            checkBoxAdmin.AutoSize = true;
            checkBoxAdmin.Location = new Point(546, 91);
            checkBoxAdmin.Name = "checkBoxAdmin";
            checkBoxAdmin.Size = new Size(102, 19);
            checkBoxAdmin.TabIndex = 9;
            checkBoxAdmin.Text = "Administrador";
            checkBoxAdmin.UseVisualStyleBackColor = true;
            // 
            // btnSalvar
            // 
            btnSalvar.Location = new Point(654, 89);
            btnSalvar.Name = "btnSalvar";
            btnSalvar.Size = new Size(123, 23);
            btnSalvar.TabIndex = 10;
            btnSalvar.Text = "SALVAR";
            btnSalvar.UseVisualStyleBackColor = true;
            btnSalvar.Click += btnSalvar_Click;
            // 
            // btnRemover
            // 
            btnRemover.Location = new Point(12, 415);
            btnRemover.Name = "btnRemover";
            btnRemover.Size = new Size(123, 23);
            btnRemover.TabIndex = 11;
            btnRemover.Text = "REMOVER";
            btnRemover.UseVisualStyleBackColor = true;
            btnRemover.Click += btnRemover_Click;
            // 
            // Users
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(789, 450);
            Controls.Add(btnRemover);
            Controls.Add(btnSalvar);
            Controls.Add(checkBoxAdmin);
            Controls.Add(label4);
            Controls.Add(txtNome);
            Controls.Add(label3);
            Controls.Add(txtZfId);
            Controls.Add(label2);
            Controls.Add(txtRe);
            Controls.Add(dgvUsers);
            Controls.Add(label1);
            FormScreenCaptureMode = ScreenCaptureMode.HideWindow;
            MaximizeBox = false;
            MaximumSize = new Size(805, 489);
            MdiChildrenMinimizedAnchorBottom = false;
            MinimizeBox = false;
            MinimumSize = new Size(805, 489);
            Name = "Users";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Usuários";
            Load += Users_Load;
            ((System.ComponentModel.ISupportInitialize)dgvUsers).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private DataGridView dgvUsers;
        private TextBox txtRe;
        private Label label2;
        private TextBox txtZfId;
        private Label label3;
        private TextBox txtNome;
        private Label label4;
        private CheckBox checkBoxAdmin;
        private Button btnSalvar;
        private Button btnRemover;
    }
}