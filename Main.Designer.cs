namespace HondaSensorChecker
{
    partial class HSCMainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HSCMainForm));
            imgClientLogo = new PictureBox();
            imgZfLogo = new PictureBox();
            lblAppTitle = new Label();
            panel1 = new Panel();
            btnNewProduct = new Button();
            btnLogs = new Button();
            btnNewUser = new Button();
            lblDebugMode = new Label();
            lblComponentQty = new Label();
            gbSapWorkOrderInfo = new GroupBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            btnWorkOrderNok = new Button();
            txtWorkOrderNumber = new TextBox();
            label2 = new Label();
            label1 = new Label();
            txtWorkOrderMaterialNumber = new TextBox();
            cbWorkOrderQtyToSend = new ComboBox();
            label3 = new Label();
            btnWorkOrderOk = new Button();
            gbZfLabel = new GroupBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            btnLogisticLabelNok = new Button();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            txtQtySupplied = new TextBox();
            txtStartPartNumber = new TextBox();
            txtLogisticUniqueNumber = new TextBox();
            btnLogisticLabelOk = new Button();
            gbZfSensorChecker = new GroupBox();
            btnConsultComponent = new Button();
            btnInterruptProcess = new Button();
            btnContinueProcess = new Button();
            btnForceChangeSupplierBox = new Button();
            btnRemoveSensor = new Button();
            listBoxReadedSensors = new ListBox();
            label7 = new Label();
            lblCheckResult = new Label();
            txtComponentSerial = new TextBox();
            ((System.ComponentModel.ISupportInitialize)imgClientLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)imgZfLogo).BeginInit();
            panel1.SuspendLayout();
            gbSapWorkOrderInfo.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            gbZfLabel.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            gbZfSensorChecker.SuspendLayout();
            SuspendLayout();
            // 
            // imgClientLogo
            // 
            imgClientLogo.Image = Properties.Resources.honda_seeklogo;
            imgClientLogo.Location = new Point(67, 4);
            imgClientLogo.Margin = new Padding(3, 4, 3, 4);
            imgClientLogo.Name = "imgClientLogo";
            imgClientLogo.Size = new Size(57, 67);
            imgClientLogo.SizeMode = PictureBoxSizeMode.Zoom;
            imgClientLogo.TabIndex = 0;
            imgClientLogo.TabStop = false;
            // 
            // imgZfLogo
            // 
            imgZfLogo.Image = Properties.Resources.zf_logo_blue;
            imgZfLogo.Location = new Point(3, 4);
            imgZfLogo.Margin = new Padding(3, 4, 3, 4);
            imgZfLogo.Name = "imgZfLogo";
            imgZfLogo.Size = new Size(57, 67);
            imgZfLogo.SizeMode = PictureBoxSizeMode.Zoom;
            imgZfLogo.TabIndex = 1;
            imgZfLogo.TabStop = false;
            // 
            // lblAppTitle
            // 
            lblAppTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblAppTitle.AutoSize = true;
            lblAppTitle.Font = new Font("Segoe UI Semibold", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAppTitle.Location = new Point(130, 14);
            lblAppTitle.Name = "lblAppTitle";
            lblAppTitle.Size = new Size(257, 46);
            lblAppTitle.TabIndex = 2;
            lblAppTitle.Text = "Sensor Checker";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(btnNewProduct);
            panel1.Controls.Add(btnLogs);
            panel1.Controls.Add(btnNewUser);
            panel1.Controls.Add(lblDebugMode);
            panel1.Controls.Add(lblComponentQty);
            panel1.Controls.Add(imgZfLogo);
            panel1.Controls.Add(lblAppTitle);
            panel1.Controls.Add(imgClientLogo);
            panel1.Location = new Point(14, 16);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(1047, 76);
            panel1.TabIndex = 3;
            // 
            // btnNewProduct
            // 
            btnNewProduct.Location = new Point(393, 41);
            btnNewProduct.Margin = new Padding(3, 4, 3, 4);
            btnNewProduct.Name = "btnNewProduct";
            btnNewProduct.Size = new Size(196, 31);
            btnNewProduct.TabIndex = 5;
            btnNewProduct.Text = "+ PRODUTO";
            btnNewProduct.UseVisualStyleBackColor = true;
            btnNewProduct.Visible = false;
            btnNewProduct.Click += btnNewProduct_Click;
            // 
            // btnLogs
            // 
            btnLogs.Location = new Point(494, 4);
            btnLogs.Margin = new Padding(3, 4, 3, 4);
            btnLogs.Name = "btnLogs";
            btnLogs.Size = new Size(95, 31);
            btnLogs.TabIndex = 6;
            btnLogs.Text = "LOGS";
            btnLogs.UseVisualStyleBackColor = true;
            btnLogs.Visible = false;
            btnLogs.Click += btnLogs_Click;
            // 
            // btnNewUser
            // 
            btnNewUser.Location = new Point(393, 4);
            btnNewUser.Margin = new Padding(3, 4, 3, 4);
            btnNewUser.Name = "btnNewUser";
            btnNewUser.Size = new Size(95, 31);
            btnNewUser.TabIndex = 4;
            btnNewUser.Text = "+ USUÁRIO";
            btnNewUser.UseVisualStyleBackColor = true;
            btnNewUser.Visible = false;
            btnNewUser.Click += btnNewUser_Click;
            // 
            // lblDebugMode
            // 
            lblDebugMode.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblDebugMode.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDebugMode.ForeColor = Color.Red;
            lblDebugMode.Location = new Point(667, 4);
            lblDebugMode.Name = "lblDebugMode";
            lblDebugMode.Size = new Size(141, 67);
            lblDebugMode.TabIndex = 7;
            lblDebugMode.Text = "DEBUG";
            lblDebugMode.TextAlign = ContentAlignment.MiddleRight;
            lblDebugMode.Visible = false;
            // 
            // lblComponentQty
            // 
            lblComponentQty.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            lblComponentQty.Font = new Font("Segoe UI Semibold", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblComponentQty.Location = new Point(811, 4);
            lblComponentQty.Name = "lblComponentQty";
            lblComponentQty.Size = new Size(233, 67);
            lblComponentQty.TabIndex = 3;
            lblComponentQty.Text = "000/000";
            lblComponentQty.TextAlign = ContentAlignment.MiddleRight;
            // 
            // gbSapWorkOrderInfo
            // 
            gbSapWorkOrderInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbSapWorkOrderInfo.Controls.Add(tableLayoutPanel1);
            gbSapWorkOrderInfo.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gbSapWorkOrderInfo.Location = new Point(14, 100);
            gbSapWorkOrderInfo.Margin = new Padding(3, 4, 3, 4);
            gbSapWorkOrderInfo.Name = "gbSapWorkOrderInfo";
            gbSapWorkOrderInfo.Padding = new Padding(3, 4, 3, 4);
            gbSapWorkOrderInfo.Size = new Size(1047, 129);
            gbSapWorkOrderInfo.TabIndex = 4;
            gbSapWorkOrderInfo.TabStop = false;
            gbSapWorkOrderInfo.Text = "SAP - WORK ORDER";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 5;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            tableLayoutPanel1.Controls.Add(btnWorkOrderNok, 3, 1);
            tableLayoutPanel1.Controls.Add(txtWorkOrderNumber, 0, 1);
            tableLayoutPanel1.Controls.Add(label2, 1, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(txtWorkOrderMaterialNumber, 1, 1);
            tableLayoutPanel1.Controls.Add(cbWorkOrderQtyToSend, 2, 1);
            tableLayoutPanel1.Controls.Add(label3, 2, 0);
            tableLayoutPanel1.Controls.Add(btnWorkOrderOk, 4, 1);
            tableLayoutPanel1.Location = new Point(7, 29);
            tableLayoutPanel1.Margin = new Padding(3, 4, 3, 4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(1033, 92);
            tableLayoutPanel1.TabIndex = 6;
            // 
            // btnWorkOrderNok
            // 
            btnWorkOrderNok.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnWorkOrderNok.Enabled = false;
            btnWorkOrderNok.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnWorkOrderNok.ForeColor = Color.FromArgb(192, 0, 0);
            btnWorkOrderNok.Location = new Point(931, 31);
            btnWorkOrderNok.Margin = new Padding(3, 4, 3, 4);
            btnWorkOrderNok.Name = "btnWorkOrderNok";
            btnWorkOrderNok.Size = new Size(45, 53);
            btnWorkOrderNok.TabIndex = 7;
            btnWorkOrderNok.Text = "✕";
            btnWorkOrderNok.UseVisualStyleBackColor = true;
            btnWorkOrderNok.Click += btnWorkOrderNok_Click;
            // 
            // txtWorkOrderNumber
            // 
            txtWorkOrderNumber.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtWorkOrderNumber.Enabled = false;
            txtWorkOrderNumber.Font = new Font("Segoe UI", 18F);
            txtWorkOrderNumber.Location = new Point(3, 31);
            txtWorkOrderNumber.Margin = new Padding(3, 4, 3, 4);
            txtWorkOrderNumber.Name = "txtWorkOrderNumber";
            txtWorkOrderNumber.Size = new Size(355, 47);
            txtWorkOrderNumber.TabIndex = 0;
            txtWorkOrderNumber.TextAlign = HorizontalAlignment.Center;
            txtWorkOrderNumber.KeyPress += txtWorkOrderNumber_KeyPress;
            txtWorkOrderNumber.Leave += txtWorkOrderNumber_Leave;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new Point(364, 7);
            label2.Name = "label2";
            label2.Size = new Size(131, 20);
            label2.TabIndex = 3;
            label2.Text = "PartNumber Final";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(3, 7);
            label1.Name = "label1";
            label1.Size = new Size(98, 20);
            label1.TabIndex = 2;
            label1.Text = "Nº da ordem";
            // 
            // txtWorkOrderMaterialNumber
            // 
            txtWorkOrderMaterialNumber.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtWorkOrderMaterialNumber.Enabled = false;
            txtWorkOrderMaterialNumber.Font = new Font("Segoe UI", 18F);
            txtWorkOrderMaterialNumber.Location = new Point(364, 31);
            txtWorkOrderMaterialNumber.Margin = new Padding(3, 4, 3, 4);
            txtWorkOrderMaterialNumber.Name = "txtWorkOrderMaterialNumber";
            txtWorkOrderMaterialNumber.Size = new Size(355, 47);
            txtWorkOrderMaterialNumber.TabIndex = 1;
            txtWorkOrderMaterialNumber.TextAlign = HorizontalAlignment.Center;
            txtWorkOrderMaterialNumber.Enter += txtWorkOrderMaterialNumber_Enter;
            txtWorkOrderMaterialNumber.KeyPress += txtWorkOrderMaterialNumber_KeyPress;
            // 
            // cbWorkOrderQtyToSend
            // 
            cbWorkOrderQtyToSend.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cbWorkOrderQtyToSend.Enabled = false;
            cbWorkOrderQtyToSend.Font = new Font("Segoe UI", 18F);
            cbWorkOrderQtyToSend.FormattingEnabled = true;
            cbWorkOrderQtyToSend.Items.AddRange(new object[] { "3", "10", "60", "420" });
            cbWorkOrderQtyToSend.Location = new Point(725, 31);
            cbWorkOrderQtyToSend.Margin = new Padding(3, 4, 3, 4);
            cbWorkOrderQtyToSend.Name = "cbWorkOrderQtyToSend";
            cbWorkOrderQtyToSend.Size = new Size(200, 49);
            cbWorkOrderQtyToSend.TabIndex = 4;
            cbWorkOrderQtyToSend.SelectedValueChanged += cbWorkOrderQtyToSend_SelectedValueChanged;
            cbWorkOrderQtyToSend.Enter += cbWorkOrderQtyToSend_Enter;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.AutoSize = true;
            label3.Location = new Point(725, 7);
            label3.Name = "label3";
            label3.Size = new Size(148, 20);
            label3.TabIndex = 5;
            label3.Text = "Quantidade a Enviar";
            // 
            // btnWorkOrderOk
            // 
            btnWorkOrderOk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnWorkOrderOk.Enabled = false;
            btnWorkOrderOk.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnWorkOrderOk.ForeColor = Color.FromArgb(0, 192, 0);
            btnWorkOrderOk.Location = new Point(982, 31);
            btnWorkOrderOk.Margin = new Padding(3, 4, 3, 4);
            btnWorkOrderOk.Name = "btnWorkOrderOk";
            btnWorkOrderOk.Size = new Size(48, 53);
            btnWorkOrderOk.TabIndex = 5;
            btnWorkOrderOk.Text = "✓";
            btnWorkOrderOk.UseVisualStyleBackColor = true;
            btnWorkOrderOk.Click += btnWorkOrderOk_Click;
            // 
            // gbZfLabel
            // 
            gbZfLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            gbZfLabel.Controls.Add(tableLayoutPanel2);
            gbZfLabel.Font = new Font("Segoe UI Semibold", 9F);
            gbZfLabel.Location = new Point(14, 237);
            gbZfLabel.Margin = new Padding(3, 4, 3, 4);
            gbZfLabel.Name = "gbZfLabel";
            gbZfLabel.Padding = new Padding(3, 4, 3, 4);
            gbZfLabel.Size = new Size(1047, 125);
            gbZfLabel.TabIndex = 5;
            gbZfLabel.TabStop = false;
            gbZfLabel.Text = "ZF - LOGISTIC LABEL";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 5;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            tableLayoutPanel2.Controls.Add(btnLogisticLabelNok, 3, 1);
            tableLayoutPanel2.Controls.Add(label6, 2, 0);
            tableLayoutPanel2.Controls.Add(label5, 1, 0);
            tableLayoutPanel2.Controls.Add(label4, 0, 0);
            tableLayoutPanel2.Controls.Add(txtQtySupplied, 2, 1);
            tableLayoutPanel2.Controls.Add(txtStartPartNumber, 1, 1);
            tableLayoutPanel2.Controls.Add(txtLogisticUniqueNumber, 0, 1);
            tableLayoutPanel2.Controls.Add(btnLogisticLabelOk, 4, 1);
            tableLayoutPanel2.Location = new Point(7, 29);
            tableLayoutPanel2.Margin = new Padding(3, 4, 3, 4);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 2;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 27F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle());
            tableLayoutPanel2.Size = new Size(1033, 88);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // btnLogisticLabelNok
            // 
            btnLogisticLabelNok.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnLogisticLabelNok.Enabled = false;
            btnLogisticLabelNok.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogisticLabelNok.ForeColor = Color.FromArgb(192, 0, 0);
            btnLogisticLabelNok.Location = new Point(930, 31);
            btnLogisticLabelNok.Margin = new Padding(3, 4, 3, 4);
            btnLogisticLabelNok.Name = "btnLogisticLabelNok";
            btnLogisticLabelNok.Size = new Size(45, 52);
            btnLogisticLabelNok.TabIndex = 8;
            btnLogisticLabelNok.Text = "✕";
            btnLogisticLabelNok.UseVisualStyleBackColor = true;
            btnLogisticLabelNok.Click += btnLogisticLabelNok_Click;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label6.AutoSize = true;
            label6.Location = new Point(621, 7);
            label6.Name = "label6";
            label6.Size = new Size(151, 20);
            label6.TabIndex = 6;
            label6.Text = "Quantidade da Caixa";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label5.AutoSize = true;
            label5.Location = new Point(312, 7);
            label5.Name = "label5";
            label5.Size = new Size(115, 20);
            label5.TabIndex = 6;
            label5.Text = "PartNumber ZF";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Location = new Point(3, 7);
            label4.Name = "label4";
            label4.Size = new Size(110, 20);
            label4.TabIndex = 6;
            label4.Text = "Número Único";
            // 
            // txtQtySupplied
            // 
            txtQtySupplied.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtQtySupplied.Enabled = false;
            txtQtySupplied.Font = new Font("Segoe UI", 18F);
            txtQtySupplied.Location = new Point(621, 31);
            txtQtySupplied.Margin = new Padding(3, 4, 3, 4);
            txtQtySupplied.Name = "txtQtySupplied";
            txtQtySupplied.Size = new Size(303, 47);
            txtQtySupplied.TabIndex = 6;
            txtQtySupplied.TextAlign = HorizontalAlignment.Center;
            txtQtySupplied.Enter += txtQtySupplied_Enter;
            txtQtySupplied.KeyPress += txtQtySupplied_KeyPress;
            // 
            // txtStartPartNumber
            // 
            txtStartPartNumber.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtStartPartNumber.Enabled = false;
            txtStartPartNumber.Font = new Font("Segoe UI", 18F);
            txtStartPartNumber.Location = new Point(312, 31);
            txtStartPartNumber.Margin = new Padding(3, 4, 3, 4);
            txtStartPartNumber.Name = "txtStartPartNumber";
            txtStartPartNumber.Size = new Size(303, 47);
            txtStartPartNumber.TabIndex = 6;
            txtStartPartNumber.TextAlign = HorizontalAlignment.Center;
            txtStartPartNumber.Enter += txtStartPartNumber_Enter;
            txtStartPartNumber.KeyPress += txtStartPartNumber_KeyPress;
            // 
            // txtLogisticUniqueNumber
            // 
            txtLogisticUniqueNumber.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtLogisticUniqueNumber.Enabled = false;
            txtLogisticUniqueNumber.Font = new Font("Segoe UI", 18F);
            txtLogisticUniqueNumber.Location = new Point(3, 31);
            txtLogisticUniqueNumber.Margin = new Padding(3, 4, 3, 4);
            txtLogisticUniqueNumber.Name = "txtLogisticUniqueNumber";
            txtLogisticUniqueNumber.Size = new Size(303, 47);
            txtLogisticUniqueNumber.TabIndex = 0;
            txtLogisticUniqueNumber.TextAlign = HorizontalAlignment.Center;
            txtLogisticUniqueNumber.Enter += txtLogisticUniqueNumber_Enter;
            txtLogisticUniqueNumber.KeyPress += txtLogisticUniqueNumber_KeyPress;
            // 
            // btnLogisticLabelOk
            // 
            btnLogisticLabelOk.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnLogisticLabelOk.Enabled = false;
            btnLogisticLabelOk.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogisticLabelOk.ForeColor = Color.FromArgb(0, 192, 0);
            btnLogisticLabelOk.Location = new Point(981, 31);
            btnLogisticLabelOk.Margin = new Padding(3, 4, 3, 4);
            btnLogisticLabelOk.Name = "btnLogisticLabelOk";
            btnLogisticLabelOk.Size = new Size(49, 52);
            btnLogisticLabelOk.TabIndex = 6;
            btnLogisticLabelOk.Text = "✓";
            btnLogisticLabelOk.UseVisualStyleBackColor = true;
            btnLogisticLabelOk.Click += btnLogisticLabelOk_Click;
            // 
            // gbZfSensorChecker
            // 
            gbZfSensorChecker.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gbZfSensorChecker.Controls.Add(btnConsultComponent);
            gbZfSensorChecker.Controls.Add(btnInterruptProcess);
            gbZfSensorChecker.Controls.Add(btnContinueProcess);
            gbZfSensorChecker.Controls.Add(btnForceChangeSupplierBox);
            gbZfSensorChecker.Controls.Add(btnRemoveSensor);
            gbZfSensorChecker.Controls.Add(listBoxReadedSensors);
            gbZfSensorChecker.Controls.Add(label7);
            gbZfSensorChecker.Controls.Add(lblCheckResult);
            gbZfSensorChecker.Controls.Add(txtComponentSerial);
            gbZfSensorChecker.Font = new Font("Segoe UI Semibold", 9F);
            gbZfSensorChecker.Location = new Point(14, 371);
            gbZfSensorChecker.Margin = new Padding(3, 4, 3, 4);
            gbZfSensorChecker.Name = "gbZfSensorChecker";
            gbZfSensorChecker.Padding = new Padding(3, 4, 3, 4);
            gbZfSensorChecker.Size = new Size(1047, 407);
            gbZfSensorChecker.TabIndex = 6;
            gbZfSensorChecker.TabStop = false;
            gbZfSensorChecker.Text = "SENSOR CHECKER";
            // 
            // btnConsultComponent
            // 
            btnConsultComponent.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnConsultComponent.Location = new Point(7, 131);
            btnConsultComponent.Margin = new Padding(3, 4, 3, 4);
            btnConsultComponent.Name = "btnConsultComponent";
            btnConsultComponent.Size = new Size(306, 37);
            btnConsultComponent.TabIndex = 7;
            btnConsultComponent.Text = "CONSULTAR COMPONENTE";
            btnConsultComponent.UseVisualStyleBackColor = true;
            btnConsultComponent.Click += btnConsultComponent_Click;
            // 
            // btnInterruptProcess
            // 
            btnInterruptProcess.Enabled = false;
            btnInterruptProcess.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnInterruptProcess.ForeColor = Color.DarkOrange;
            btnInterruptProcess.Location = new Point(7, 176);
            btnInterruptProcess.Margin = new Padding(3, 4, 3, 4);
            btnInterruptProcess.Name = "btnInterruptProcess";
            btnInterruptProcess.Size = new Size(306, 37);
            btnInterruptProcess.TabIndex = 8;
            btnInterruptProcess.Text = "INTERROMPER PROCESSO";
            btnInterruptProcess.UseVisualStyleBackColor = true;
            btnInterruptProcess.Click += btnInterruptProcess_Click;
            // 
            // btnContinueProcess
            // 
            btnContinueProcess.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnContinueProcess.Location = new Point(7, 221);
            btnContinueProcess.Margin = new Padding(3, 4, 3, 4);
            btnContinueProcess.Name = "btnContinueProcess";
            btnContinueProcess.Size = new Size(306, 37);
            btnContinueProcess.TabIndex = 6;
            btnContinueProcess.Text = "CONTINUAR PROCESSO";
            btnContinueProcess.UseVisualStyleBackColor = true;
            btnContinueProcess.Click += btnContinueProcess_Click;
            // 
            // btnForceChangeSupplierBox
            // 
            btnForceChangeSupplierBox.Enabled = false;
            btnForceChangeSupplierBox.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnForceChangeSupplierBox.Location = new Point(7, 266);
            btnForceChangeSupplierBox.Margin = new Padding(3, 4, 3, 4);
            btnForceChangeSupplierBox.Name = "btnForceChangeSupplierBox";
            btnForceChangeSupplierBox.Size = new Size(306, 37);
            btnForceChangeSupplierBox.TabIndex = 5;
            btnForceChangeSupplierBox.Text = "TROCAR SUPPLIER BOX";
            btnForceChangeSupplierBox.UseVisualStyleBackColor = true;
            btnForceChangeSupplierBox.Click += btnForceChangeSupplierBox_Click;
            // 
            // btnRemoveSensor
            // 
            btnRemoveSensor.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnRemoveSensor.Location = new Point(7, 311);
            btnRemoveSensor.Margin = new Padding(3, 4, 3, 4);
            btnRemoveSensor.Name = "btnRemoveSensor";
            btnRemoveSensor.Size = new Size(306, 37);
            btnRemoveSensor.TabIndex = 4;
            btnRemoveSensor.Text = "REMOVER (SCRAP)";
            btnRemoveSensor.UseVisualStyleBackColor = true;
            btnRemoveSensor.Click += btnRemoveSensor_Click;
            // 
            // listBoxReadedSensors
            // 
            listBoxReadedSensors.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listBoxReadedSensors.Font = new Font("Segoe UI", 9F);
            listBoxReadedSensors.FormattingEnabled = true;
            listBoxReadedSensors.Location = new Point(7, 356);
            listBoxReadedSensors.Margin = new Padding(3, 4, 3, 4);
            listBoxReadedSensors.Name = "listBoxReadedSensors";
            listBoxReadedSensors.Size = new Size(306, 24);
            listBoxReadedSensors.TabIndex = 3;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(7, 25);
            label7.Name = "label7";
            label7.Size = new Size(56, 20);
            label7.TabIndex = 1;
            label7.Text = "Leitura";
            // 
            // lblCheckResult
            // 
            lblCheckResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblCheckResult.BackColor = Color.Yellow;
            lblCheckResult.Font = new Font("Segoe UI", 36F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCheckResult.Location = new Point(320, 23);
            lblCheckResult.Name = "lblCheckResult";
            lblCheckResult.Size = new Size(720, 379);
            lblCheckResult.TabIndex = 2;
            lblCheckResult.Text = "REALIZAR LEITURA DA WORK ORDER";
            lblCheckResult.TextAlign = ContentAlignment.MiddleCenter;
            lblCheckResult.Click += lblCheckResult_Click;
            // 
            // txtComponentSerial
            // 
            txtComponentSerial.Enabled = false;
            txtComponentSerial.Font = new Font("Segoe UI", 27F);
            txtComponentSerial.Location = new Point(7, 49);
            txtComponentSerial.Margin = new Padding(3, 4, 3, 4);
            txtComponentSerial.Name = "txtComponentSerial";
            txtComponentSerial.Size = new Size(306, 67);
            txtComponentSerial.TabIndex = 0;
            txtComponentSerial.TextAlign = HorizontalAlignment.Center;
            txtComponentSerial.Enter += txtComponentSerial_Enter;
            txtComponentSerial.KeyPress += txtComponentSerial_KeyPress;
            // 
            // HSCMainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Window;
            ClientSize = new Size(1074, 793);
            Controls.Add(gbZfSensorChecker);
            Controls.Add(gbZfLabel);
            Controls.Add(gbSapWorkOrderInfo);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            MinimumSize = new Size(1085, 695);
            Name = "HSCMainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sensor Checker";
            WindowState = FormWindowState.Maximized;
            Load += HSCMainForm_Load;
            ((System.ComponentModel.ISupportInitialize)imgClientLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)imgZfLogo).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            gbSapWorkOrderInfo.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            gbZfLabel.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            gbZfSensorChecker.ResumeLayout(false);
            gbZfSensorChecker.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox imgClientLogo;
        private PictureBox imgZfLogo;
        private Label lblAppTitle;
        private Panel panel1;
        private Label lblComponentQty;
        private Label lblDebugMode;
        private GroupBox gbSapWorkOrderInfo;
        private TextBox txtWorkOrderNumber;
        private ComboBox cbWorkOrderQtyToSend;
        private Label label2;
        private Label label1;
        private TextBox txtWorkOrderMaterialNumber;
        private Label label3;
        private TableLayoutPanel tableLayoutPanel1;
        private Button btnWorkOrderOk;
        private GroupBox gbZfLabel;
        private TableLayoutPanel tableLayoutPanel2;
        private TextBox txtLogisticUniqueNumber;
        private Button btnLogisticLabelOk;
        private TextBox txtQtySupplied;
        private TextBox txtStartPartNumber;
        private Label label4;
        private Label label5;
        private Label label6;
        private GroupBox gbZfSensorChecker;
        private Label lblCheckResult;
        private Label label7;
        private TextBox txtComponentSerial;
        private ListBox listBoxReadedSensors;
        private Button btnWorkOrderNok;
        private Button btnLogisticLabelNok;
        private Button btnNewUser;
        private Button btnNewProduct;
        private Button btnLogs;

        // NEW/EXISTING
        private Button btnRemoveSensor;
        private Button btnForceChangeSupplierBox;
        private Button btnConsultComponent;
        private Button btnContinueProcess;
        private Button btnInterruptProcess;
    }
}
