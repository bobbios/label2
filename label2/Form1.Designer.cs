namespace label2
{
    partial class Form1
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
            this.btnPrint = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPrinter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBoxNumOfPrint = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.comboBoxSalespersons = new System.Windows.Forms.ComboBox();
            this.btnCredit = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.txtIpAddress = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.txtInvoiceNum = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.comboSaleType = new System.Windows.Forms.ComboBox();
            this.comboTransType = new System.Windows.Forms.ComboBox();
            this.comboBoxStopBits = new System.Windows.Forms.ComboBox();
            this.comboBoxPort = new System.Windows.Forms.ComboBox();
            this.comboBoxDataBits = new System.Windows.Forms.ComboBox();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.comboBoxParity = new System.Windows.Forms.ComboBox();
            this.comboBoxCommands = new System.Windows.Forms.ComboBox();
            this.butTest2 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.btnTest1 = new System.Windows.Forms.Button();
            this.btnTestPassiveRead = new System.Windows.Forms.Button();
            this.btnTestPort = new System.Windows.Forms.Button();
            this.btnTestCommand = new System.Windows.Forms.Button();
            this.lblScaleData = new System.Windows.Forms.Label();
            this.txtBoxScale = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.btnReadMagellan = new System.Windows.Forms.Button();
            this.btnOpenPort = new System.Windows.Forms.Button();
            this.btnSendScaleMonitor = new System.Windows.Forms.Button();
            this.btnSendReset = new System.Windows.Forms.Button();
            this.btnSendGetSentryStatus = new System.Windows.Forms.Button();
            this.btnBatchClose = new System.Windows.Forms.Button();
            this.btnGetLastTransactin = new System.Windows.Forms.Button();
            this.btnPreAddTip = new System.Windows.Forms.Button();
            this.btnPassThru = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.chkBoxReverseText = new System.Windows.Forms.CheckBox();
            this.comboBoxLines = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBoxDate = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(16, 298);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(139, 33);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "Print zebra";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnZebra_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 250);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "\\\\computer\\label_printer";
            // 
            // txtPrinter
            // 
            this.txtPrinter.Location = new System.Drawing.Point(16, 266);
            this.txtPrinter.Name = "txtPrinter";
            this.txtPrinter.Size = new System.Drawing.Size(139, 20);
            this.txtPrinter.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(85, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "first line";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(70, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "second line";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(85, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "third line";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(64, 102);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "sales Person";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(137, 14);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(140, 20);
            this.textBox1.TabIndex = 9;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(137, 40);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(140, 20);
            this.textBox2.TabIndex = 10;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(137, 67);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(140, 20);
            this.textBox3.TabIndex = 11;
            // 
            // textBoxNumOfPrint
            // 
            this.textBoxNumOfPrint.Location = new System.Drawing.Point(137, 123);
            this.textBoxNumOfPrint.Name = "textBoxNumOfPrint";
            this.textBoxNumOfPrint.Size = new System.Drawing.Size(70, 20);
            this.textBoxNumOfPrint.TabIndex = 13;
            this.textBoxNumOfPrint.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(92, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "# label";
            // 
            // comboBoxSalespersons
            // 
            this.comboBoxSalespersons.FormattingEnabled = true;
            this.comboBoxSalespersons.Location = new System.Drawing.Point(137, 93);
            this.comboBoxSalespersons.Name = "comboBoxSalespersons";
            this.comboBoxSalespersons.Size = new System.Drawing.Size(142, 21);
            this.comboBoxSalespersons.TabIndex = 15;
            // 
            // btnCredit
            // 
            this.btnCredit.Location = new System.Drawing.Point(79, 253);
            this.btnCredit.Name = "btnCredit";
            this.btnCredit.Size = new System.Drawing.Size(184, 24);
            this.btnCredit.TabIndex = 16;
            this.btnCredit.Text = "test ccard";
            this.btnCredit.UseVisualStyleBackColor = true;
            this.btnCredit.Click += new System.EventHandler(this.btnCredit_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(21, 458);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(1057, 32);
            this.button3.TabIndex = 17;
            this.button3.Text = "close";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtIpAddress
            // 
            this.txtIpAddress.Location = new System.Drawing.Point(96, 25);
            this.txtIpAddress.Name = "txtIpAddress";
            this.txtIpAddress.Size = new System.Drawing.Size(184, 20);
            this.txtIpAddress.TabIndex = 18;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(93, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "ipaddress";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(93, 52);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 13);
            this.label8.TabIndex = 20;
            this.label8.Text = "amount no decimal";
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(96, 68);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(184, 20);
            this.txtAmount.TabIndex = 23;
            // 
            // txtInvoiceNum
            // 
            this.txtInvoiceNum.Location = new System.Drawing.Point(96, 115);
            this.txtInvoiceNum.Name = "txtInvoiceNum";
            this.txtInvoiceNum.Size = new System.Drawing.Size(184, 20);
            this.txtInvoiceNum.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(93, 99);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 13);
            this.label9.TabIndex = 25;
            this.label9.Text = "txtInvoiceNum";
            // 
            // comboSaleType
            // 
            this.comboSaleType.FormattingEnabled = true;
            this.comboSaleType.Location = new System.Drawing.Point(96, 148);
            this.comboSaleType.Name = "comboSaleType";
            this.comboSaleType.Size = new System.Drawing.Size(179, 21);
            this.comboSaleType.TabIndex = 26;
            // 
            // comboTransType
            // 
            this.comboTransType.FormattingEnabled = true;
            this.comboTransType.Location = new System.Drawing.Point(96, 188);
            this.comboTransType.Name = "comboTransType";
            this.comboTransType.Size = new System.Drawing.Size(179, 21);
            this.comboTransType.TabIndex = 27;
            // 
            // comboBoxStopBits
            // 
            this.comboBoxStopBits.FormattingEnabled = true;
            this.comboBoxStopBits.Location = new System.Drawing.Point(18, 256);
            this.comboBoxStopBits.Name = "comboBoxStopBits";
            this.comboBoxStopBits.Size = new System.Drawing.Size(234, 21);
            this.comboBoxStopBits.TabIndex = 34;
            // 
            // comboBoxPort
            // 
            this.comboBoxPort.FormattingEnabled = true;
            this.comboBoxPort.Location = new System.Drawing.Point(18, 147);
            this.comboBoxPort.Name = "comboBoxPort";
            this.comboBoxPort.Size = new System.Drawing.Size(234, 21);
            this.comboBoxPort.TabIndex = 33;
            // 
            // comboBoxDataBits
            // 
            this.comboBoxDataBits.FormattingEnabled = true;
            this.comboBoxDataBits.Location = new System.Drawing.Point(18, 202);
            this.comboBoxDataBits.Name = "comboBoxDataBits";
            this.comboBoxDataBits.Size = new System.Drawing.Size(234, 21);
            this.comboBoxDataBits.TabIndex = 32;
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.FormattingEnabled = true;
            this.comboBoxBaudRate.Location = new System.Drawing.Point(18, 175);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(234, 21);
            this.comboBoxBaudRate.TabIndex = 31;
            // 
            // comboBoxParity
            // 
            this.comboBoxParity.FormattingEnabled = true;
            this.comboBoxParity.Location = new System.Drawing.Point(18, 229);
            this.comboBoxParity.Name = "comboBoxParity";
            this.comboBoxParity.Size = new System.Drawing.Size(234, 21);
            this.comboBoxParity.TabIndex = 30;
            // 
            // comboBoxCommands
            // 
            this.comboBoxCommands.FormattingEnabled = true;
            this.comboBoxCommands.Location = new System.Drawing.Point(18, 119);
            this.comboBoxCommands.Name = "comboBoxCommands";
            this.comboBoxCommands.Size = new System.Drawing.Size(234, 21);
            this.comboBoxCommands.TabIndex = 29;
            // 
            // butTest2
            // 
            this.butTest2.Location = new System.Drawing.Point(148, 309);
            this.butTest2.Name = "butTest2";
            this.butTest2.Size = new System.Drawing.Size(104, 32);
            this.butTest2.TabIndex = 35;
            this.butTest2.Text = "test 2Com";
            this.butTest2.UseVisualStyleBackColor = true;
            this.butTest2.Click += new System.EventHandler(this.butTest2_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(76, 221);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(206, 26);
            this.label10.TabIndex = 36;
            this.label10.Text = "setting >> time zone\r\nsetting >> securty >> schedule reboot time";
            // 
            // btnTest1
            // 
            this.btnTest1.Location = new System.Drawing.Point(18, 309);
            this.btnTest1.Name = "btnTest1";
            this.btnTest1.Size = new System.Drawing.Size(107, 32);
            this.btnTest1.TabIndex = 38;
            this.btnTest1.Text = "test Scale1";
            this.btnTest1.UseVisualStyleBackColor = true;
            this.btnTest1.Click += new System.EventHandler(this.btnTest1_Click);
            // 
            // btnTestPassiveRead
            // 
            this.btnTestPassiveRead.Location = new System.Drawing.Point(85, 84);
            this.btnTestPassiveRead.Name = "btnTestPassiveRead";
            this.btnTestPassiveRead.Size = new System.Drawing.Size(84, 25);
            this.btnTestPassiveRead.TabIndex = 39;
            this.btnTestPassiveRead.Text = "passivemode";
            this.btnTestPassiveRead.UseVisualStyleBackColor = true;
            this.btnTestPassiveRead.Click += new System.EventHandler(this.btnTestPassiveRead_Click);
            // 
            // btnTestPort
            // 
            this.btnTestPort.Location = new System.Drawing.Point(16, 85);
            this.btnTestPort.Name = "btnTestPort";
            this.btnTestPort.Size = new System.Drawing.Size(63, 25);
            this.btnTestPort.TabIndex = 40;
            this.btnTestPort.Text = "textPort";
            this.btnTestPort.UseVisualStyleBackColor = true;
            this.btnTestPort.Click += new System.EventHandler(this.btnTestPort_Click);
            // 
            // btnTestCommand
            // 
            this.btnTestCommand.Location = new System.Drawing.Point(170, 84);
            this.btnTestCommand.Name = "btnTestCommand";
            this.btnTestCommand.Size = new System.Drawing.Size(82, 24);
            this.btnTestCommand.TabIndex = 41;
            this.btnTestCommand.Text = "testCommand";
            this.btnTestCommand.UseVisualStyleBackColor = true;
            this.btnTestCommand.Click += new System.EventHandler(this.btnTestCommand_Click);
            // 
            // lblScaleData
            // 
            this.lblScaleData.AutoSize = true;
            this.lblScaleData.Location = new System.Drawing.Point(133, 17);
            this.lblScaleData.Name = "lblScaleData";
            this.lblScaleData.Size = new System.Drawing.Size(67, 13);
            this.lblScaleData.TabIndex = 43;
            this.lblScaleData.Text = "lblScaleData";
            // 
            // txtBoxScale
            // 
            this.txtBoxScale.Location = new System.Drawing.Point(18, 283);
            this.txtBoxScale.Name = "txtBoxScale";
            this.txtBoxScale.Size = new System.Drawing.Size(234, 20);
            this.txtBoxScale.TabIndex = 45;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 54);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 13);
            this.label11.TabIndex = 46;
            this.label11.Text = "ipaddress";
            // 
            // btnReadMagellan
            // 
            this.btnReadMagellan.Location = new System.Drawing.Point(267, 84);
            this.btnReadMagellan.Name = "btnReadMagellan";
            this.btnReadMagellan.Size = new System.Drawing.Size(75, 42);
            this.btnReadMagellan.TabIndex = 47;
            this.btnReadMagellan.Text = "read magellan";
            this.btnReadMagellan.UseVisualStyleBackColor = true;
            // 
            // btnOpenPort
            // 
            this.btnOpenPort.Location = new System.Drawing.Point(267, 134);
            this.btnOpenPort.Name = "btnOpenPort";
            this.btnOpenPort.Size = new System.Drawing.Size(75, 23);
            this.btnOpenPort.TabIndex = 48;
            this.btnOpenPort.Text = "btnOpenPort";
            this.btnOpenPort.UseVisualStyleBackColor = true;
            this.btnOpenPort.Click += new System.EventHandler(this.btnOpenPort_Click);
            // 
            // btnSendScaleMonitor
            // 
            this.btnSendScaleMonitor.Location = new System.Drawing.Point(267, 167);
            this.btnSendScaleMonitor.Name = "btnSendScaleMonitor";
            this.btnSendScaleMonitor.Size = new System.Drawing.Size(77, 25);
            this.btnSendScaleMonitor.TabIndex = 49;
            this.btnSendScaleMonitor.Text = "btnSendScaleMonitor";
            this.btnSendScaleMonitor.UseVisualStyleBackColor = true;
            this.btnSendScaleMonitor.Click += new System.EventHandler(this.btnSendScaleMonitor_Click);
            // 
            // btnSendReset
            // 
            this.btnSendReset.Location = new System.Drawing.Point(267, 201);
            this.btnSendReset.Name = "btnSendReset";
            this.btnSendReset.Size = new System.Drawing.Size(78, 24);
            this.btnSendReset.TabIndex = 50;
            this.btnSendReset.Text = "btnSendReset";
            this.btnSendReset.UseVisualStyleBackColor = true;
            this.btnSendReset.Click += new System.EventHandler(this.btnSendReset_Click);
            // 
            // btnSendGetSentryStatus
            // 
            this.btnSendGetSentryStatus.Location = new System.Drawing.Point(267, 231);
            this.btnSendGetSentryStatus.Name = "btnSendGetSentryStatus";
            this.btnSendGetSentryStatus.Size = new System.Drawing.Size(75, 23);
            this.btnSendGetSentryStatus.TabIndex = 51;
            this.btnSendGetSentryStatus.Text = "btnSendGetSentryStatus";
            this.btnSendGetSentryStatus.UseVisualStyleBackColor = true;
            this.btnSendGetSentryStatus.Click += new System.EventHandler(this.btnSendGetSentryStatus_Click);
            // 
            // btnBatchClose
            // 
            this.btnBatchClose.Location = new System.Drawing.Point(79, 285);
            this.btnBatchClose.Name = "btnBatchClose";
            this.btnBatchClose.Size = new System.Drawing.Size(184, 26);
            this.btnBatchClose.TabIndex = 52;
            this.btnBatchClose.Text = "batch close";
            this.btnBatchClose.UseVisualStyleBackColor = true;
            this.btnBatchClose.Click += new System.EventHandler(this.btnBatchClose_Click);
            // 
            // btnGetLastTransactin
            // 
            this.btnGetLastTransactin.Location = new System.Drawing.Point(79, 319);
            this.btnGetLastTransactin.Name = "btnGetLastTransactin";
            this.btnGetLastTransactin.Size = new System.Drawing.Size(184, 26);
            this.btnGetLastTransactin.TabIndex = 53;
            this.btnGetLastTransactin.Text = "get last transactions";
            this.btnGetLastTransactin.UseVisualStyleBackColor = true;
            this.btnGetLastTransactin.Click += new System.EventHandler(this.btnGetLastTransactin_Click);
            // 
            // btnPreAddTip
            // 
            this.btnPreAddTip.Location = new System.Drawing.Point(79, 353);
            this.btnPreAddTip.Name = "btnPreAddTip";
            this.btnPreAddTip.Size = new System.Drawing.Size(184, 20);
            this.btnPreAddTip.TabIndex = 55;
            this.btnPreAddTip.Text = "preAddTip/not working";
            this.btnPreAddTip.UseVisualStyleBackColor = true;
            this.btnPreAddTip.Click += new System.EventHandler(this.btnPreAddTip_Click);
            // 
            // btnPassThru
            // 
            this.btnPassThru.Location = new System.Drawing.Point(79, 381);
            this.btnPassThru.Name = "btnPassThru";
            this.btnPassThru.Size = new System.Drawing.Size(184, 25);
            this.btnPassThru.TabIndex = 56;
            this.btnPassThru.Text = "passThru";
            this.btnPassThru.UseVisualStyleBackColor = true;
            this.btnPassThru.Click += new System.EventHandler(this.btnPassThru_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(175, 298);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(139, 33);
            this.button2.TabIndex = 57;
            this.button2.Text = "Print BTP";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnSnbc_Click);
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(175, 268);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(139, 20);
            this.textBox10.TabIndex = 62;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(172, 250);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(167, 13);
            this.label12.TabIndex = 61;
            this.label12.Text = "\\\\\\\\DESKTOP-HPLQJ4E\\\\SNBC";
            // 
            // chkBoxReverseText
            // 
            this.chkBoxReverseText.AutoSize = true;
            this.chkBoxReverseText.Location = new System.Drawing.Point(137, 171);
            this.chkBoxReverseText.Name = "chkBoxReverseText";
            this.chkBoxReverseText.Size = new System.Drawing.Size(85, 17);
            this.chkBoxReverseText.TabIndex = 63;
            this.chkBoxReverseText.Text = "reverse Text";
            this.chkBoxReverseText.UseVisualStyleBackColor = true;
            // 
            // comboBoxLines
            // 
            this.comboBoxLines.FormattingEnabled = true;
            this.comboBoxLines.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.comboBoxLines.Location = new System.Drawing.Point(175, 226);
            this.comboBoxLines.Name = "comboBoxLines";
            this.comboBoxLines.Size = new System.Drawing.Size(139, 21);
            this.comboBoxLines.TabIndex = 64;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(219, 207);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(51, 13);
            this.label13.TabIndex = 65;
            this.label13.Text = "print lines";
            // 
            // checkBoxDate
            // 
            this.checkBoxDate.AutoSize = true;
            this.checkBoxDate.Checked = true;
            this.checkBoxDate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxDate.Location = new System.Drawing.Point(137, 149);
            this.checkBoxDate.Name = "checkBoxDate";
            this.checkBoxDate.Size = new System.Drawing.Size(70, 17);
            this.checkBoxDate.TabIndex = 66;
            this.checkBoxDate.Text = "add Date";
            this.checkBoxDate.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.groupBox1.Controls.Add(this.comboBoxSalespersons);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.checkBoxDate);
            this.groupBox1.Controls.Add(this.comboBoxLines);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox10);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.chkBoxReverseText);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox3);
            this.groupBox1.Controls.Add(this.textBoxNumOfPrint);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnPrint);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtPrinter);
            this.groupBox1.Location = new System.Drawing.Point(21, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(345, 412);
            this.groupBox1.TabIndex = 67;
            this.groupBox1.TabStop = false;
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.groupBox2.Controls.Add(this.comboTransType);
            this.groupBox2.Controls.Add(this.txtIpAddress);
            this.groupBox2.Controls.Add(this.btnPassThru);
            this.groupBox2.Controls.Add(this.btnPreAddTip);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.btnGetLastTransactin);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.btnBatchClose);
            this.groupBox2.Controls.Add(this.txtAmount);
            this.groupBox2.Controls.Add(this.txtInvoiceNum);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.comboSaleType);
            this.groupBox2.Controls.Add(this.btnCredit);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Location = new System.Drawing.Point(372, 40);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(352, 412);
            this.groupBox2.TabIndex = 68;
            this.groupBox2.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.groupBox3.Controls.Add(this.btnSendReset);
            this.groupBox3.Controls.Add(this.comboBoxCommands);
            this.groupBox3.Controls.Add(this.comboBoxParity);
            this.groupBox3.Controls.Add(this.btnSendGetSentryStatus);
            this.groupBox3.Controls.Add(this.comboBoxBaudRate);
            this.groupBox3.Controls.Add(this.comboBoxDataBits);
            this.groupBox3.Controls.Add(this.btnSendScaleMonitor);
            this.groupBox3.Controls.Add(this.comboBoxPort);
            this.groupBox3.Controls.Add(this.btnOpenPort);
            this.groupBox3.Controls.Add(this.comboBoxStopBits);
            this.groupBox3.Controls.Add(this.btnReadMagellan);
            this.groupBox3.Controls.Add(this.butTest2);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.btnTest1);
            this.groupBox3.Controls.Add(this.txtBoxScale);
            this.groupBox3.Controls.Add(this.btnTestPassiveRead);
            this.groupBox3.Controls.Add(this.lblScaleData);
            this.groupBox3.Controls.Add(this.btnTestPort);
            this.groupBox3.Controls.Add(this.btnTestCommand);
            this.groupBox3.Location = new System.Drawing.Point(730, 40);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(348, 412);
            this.groupBox3.TabIndex = 69;
            this.groupBox3.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1160, 590);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Name = "Form1";
            this.Text = "Form1";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPrinter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBoxNumOfPrint;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox comboBoxSalespersons;
        private System.Windows.Forms.Button btnCredit;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtIpAddress;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.TextBox txtInvoiceNum;
        private System.Windows.Forms.Label label9;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ComboBox comboSaleType;
        private System.Windows.Forms.ComboBox comboTransType;
        private System.Windows.Forms.ComboBox comboBoxStopBits;
        private System.Windows.Forms.ComboBox comboBoxPort;
        private System.Windows.Forms.ComboBox comboBoxDataBits;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.Windows.Forms.ComboBox comboBoxParity;
        private System.Windows.Forms.ComboBox comboBoxCommands;
        private System.Windows.Forms.Button butTest2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnTest1;
        private System.Windows.Forms.Button btnTestPassiveRead;
        private System.Windows.Forms.Button btnTestPort;
        private System.Windows.Forms.Button btnTestCommand;
        private System.Windows.Forms.Label lblScaleData;
        private System.Windows.Forms.TextBox txtBoxScale;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnReadMagellan;
        private System.Windows.Forms.Button btnOpenPort;
        private System.Windows.Forms.Button btnSendScaleMonitor;
        private System.Windows.Forms.Button btnSendReset;
        private System.Windows.Forms.Button btnSendGetSentryStatus;
        private System.Windows.Forms.Button btnBatchClose;
        private System.Windows.Forms.Button btnGetLastTransactin;
        private System.Windows.Forms.Button btnPreAddTip;
        private System.Windows.Forms.Button btnPassThru;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox chkBoxReverseText;
        private System.Windows.Forms.ComboBox comboBoxLines;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBoxDate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}
