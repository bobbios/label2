using System;
using System.Drawing.Text;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using POSLink2.CommSetting;
using POSLink2.Batch;
using POSLink2.Report;
using POSLink2;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Diagnostics.Eventing.Reader;
using POSLink2.Transaction;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using POSLink2.Const;
using POSLink2.Manage;   // Option A (most common for v2)
using System.Runtime.CompilerServices;

namespace label2
{
    public partial class Form1 : Form
    {
        private SerialPort port; // Declare SerialPort at the class level
        private SerialPort divaPort;
        private ScaleReader scaleReader;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public Form1()
        {
            InitializeComponent();

            // IMPORTANT: ensure we always clean up ports/logs
            this.FormClosing += Form1_FormClosing;

            CreateDynamicPosLinkIni();   // <-- ADD THIS

            // Load settings file (create if missing)
            var s = AppSettingsStore.LoadOrCreate();

            // Apply to UI
            if (!string.IsNullOrWhiteSpace(s.ZebraPrinterPath))
                txtPrinter.Text = s.ZebraPrinterPath;

            if (!string.IsNullOrWhiteSpace(s.SnbcPrinterPath))
                textBox10.Text = s.SnbcPrinterPath;

            if (!string.IsNullOrWhiteSpace(s.CreditTerminalIp))
                txtIpAddress.Text = s.CreditTerminalIp;

            PopulateSalespersonDropdown();

            textBox1.Text = "Pks Payments";
            textBox2.Text = "Store : ";
            textBox3.Text = "Mid : ";
            txtAmount.Text = "5";
            txtInvoiceNum.Text = "35158415488";

            string[] saleType = { "CREDIT", "DEBIT", "EBT" };
            comboSaleType.Items.AddRange(saleType);

            string[] transType = { "SALE", "RETURN" };
            comboTransType.Items.AddRange(transType);

            // Initialize the SerialPort
            port = new SerialPort();
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);

            // Populate comboBoxCommands with CAS scale commands
            comboBoxCommands.Items.Add("Request Weight (W)");
            comboBoxCommands.Items.Add("Zero Scale (Z)");
            comboBoxCommands.Items.Add("Tare Scale (T)");
            comboBoxCommands.Items.Add("Clear Tare (C)");
            comboBoxCommands.Items.Add("Set Unit to kg (Ukg)");
            comboBoxCommands.Items.Add("Set Unit to lbs (Ulbs)");
            comboBoxCommands.SelectedIndex = 0;

            // Populate comboBoxPort with available COM ports
            comboBoxPort.Items.Add("COM1");
            comboBoxPort.Items.Add("COM2");
            comboBoxPort.SelectedIndex = 0;

            // Populate comboBoxBaudRate with common baud rates
            comboBoxBaudRate.Items.Add("9600");
            comboBoxBaudRate.Items.Add("19200");
            comboBoxBaudRate.Items.Add("38400");
            comboBoxBaudRate.Items.Add("57600");
            comboBoxBaudRate.Items.Add("115200");
            comboBoxBaudRate.SelectedIndex = 0;

            // Populate comboBoxParity with parity options
            comboBoxParity.Items.Add(Parity.None.ToString());
            comboBoxParity.Items.Add(Parity.Odd.ToString());
            comboBoxParity.Items.Add(Parity.Even.ToString());
            comboBoxParity.Items.Add(Parity.Mark.ToString());
            comboBoxParity.Items.Add(Parity.Space.ToString());
            comboBoxParity.SelectedIndex = 0;

            // Populate comboBoxDataBits with data bit options
            comboBoxDataBits.Items.Add("7");
            comboBoxDataBits.Items.Add("8");
            comboBoxDataBits.SelectedIndex = 0;

            // Populate comboBoxStopBits with stop bit options
            comboBoxStopBits.Items.Add(StopBits.One.ToString());
            comboBoxStopBits.Items.Add(StopBits.OnePointFive.ToString());
            comboBoxStopBits.Items.Add(StopBits.Two.ToString());
            comboBoxStopBits.SelectedIndex = 0;

            // Your ScaleReader wrapper (assumed defined elsewhere in your project)
            scaleReader = new ScaleReader("COM1", 9600, 8, Parity.None, StopBits.One);
            scaleReader.DataReceived += OnDataReceived;
        }

        // Method to retrieve the printer path from the Windows Registry
        private string GetPrinterPathFromRegistry()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\MyAppName");
            if (key != null)
            {
                object printerPath = key.GetValue("PrinterPath");
                key.Close();
                return printerPath != null ? printerPath.ToString() : string.Empty;
            }
            return string.Empty;
        }

        private void CreateDynamicPosLinkIni()
        {
            try
            {
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string logFolder = Path.Combine(desktop, "PKSLogs");

                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
                string iniPath = Path.Combine(exeFolder, "POSLink2.ini");

                string iniContent =
                    "[LOG]" + Environment.NewLine +
                    "LOGPATH=" + logFolder + Environment.NewLine +
                    "LOGLEVEL=3";

                File.WriteAllText(iniPath, iniContent);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create dynamic POSLink2.ini: " + ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop any running background listeners
            try
            {
                cancellationTokenSource?.Cancel();
            }
            catch { }

            // Close ScaleReader safely
            try
            {
                scaleReader?.Close();
            }
            catch { }

            // Close local serial ports safely
            try
            {
                if (port != null)
                {
                    port.DataReceived -= port_DataReceived;
                    if (port.IsOpen) port.Close();
                    port.Dispose();
                }
            }
            catch { }

            try
            {
                if (divaPort != null)
                {
                    divaPort.DataReceived -= DivaDataReceivedHandler;
                    if (divaPort.IsOpen) divaPort.Close();
                    divaPort.Dispose();
                }
            }
            catch { }

            // Move POSLink2 log
            try
            {
                // Where POSLink writes log (EXE folder)
                string exeFolder = AppDomain.CurrentDomain.BaseDirectory;
                string original = Path.Combine(exeFolder, "POSLink2.log");

                if (!File.Exists(original))
                    return;

                // Where YOU want it
                string desktopFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "PKSLogs"
                );

                if (!Directory.Exists(desktopFolder))
                    Directory.CreateDirectory(desktopFolder);

                string today = DateTime.Now.ToString("yyyyMMdd");
                string destination = Path.Combine(desktopFolder, $"POSLinkLog{today}.log");

                if (File.Exists(destination))
                    File.Delete(destination);

                File.Move(original, destination);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log move failed: " + ex.Message);
            }
        }

        private void SaveLocalSettingsFile()
        {
            var s = new AppSettings
            {
                ZebraPrinterPath = txtPrinter.Text?.Trim() ?? "",
                SnbcPrinterPath = textBox10.Text?.Trim() ?? "",
                CreditTerminalIp = txtIpAddress.Text?.Trim() ?? ""
            };

            AppSettingsStore.Save(s);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            SaveLocalSettingsFile();

            string line1 = textBox1.Text;
            string line2 = textBox2.Text;
            string line3 = textBox3.Text;
            string line4 = comboBoxSalespersons.Text;

            int copies;
            if (!int.TryParse(textBoxNumOfPrint.Text, out copies) || copies <= 0)
            {
                MessageBox.Show("Please enter a valid number of copies.");
                return;
            }

            try
            {
                // Build the ESC/POS command string
                string escPosCommands =
                    (char)27 + "A" + (char)49 + (char)49 + line1 + "\r" +
                    (char)27 + "A" + (char)49 + (char)50 + line2 + "\r" +
                    (char)27 + "A" + (char)49 + (char)50 + line3 + "\r" +
                    (char)27 + "A" + (char)49 + (char)50 + line4 + "\r" +
                    (char)12; // Form feed (new page)

                for (int i = 0; i < copies; i++)
                    RawPrinterHelper.SendStringToPrinter(txtPrinter.Text, escPosCommands);

                MessageBox.Show("Print job sent successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send print job: " + ex.Message);
            }
        }

        private void btnPrintDouble_Click(object sender, EventArgs e)
        {
            SavePrinterPathToRegistry(txtPrinter.Text);

            string line1 = textBox1.Text;
            string line2 = textBox2.Text;
            string line3 = textBox3.Text;

            int copies;
            if (!int.TryParse(textBoxNumOfPrint.Text, out copies) || copies <= 0)
            {
                MessageBox.Show("Please enter a valid number of copies.");
                return;
            }

            string escPosCommands =
                (char)27 + "A" + (char)49 + (char)50 + " " + line1 + "\r" +
                (char)27 + "A" + (char)49 + (char)50 + " " + line2 + "\r" +
                (char)27 + "A" + (char)49 + (char)50 + " " + line3 + "\r" +
                (char)12;

            try
            {
                for (int i = 0; i < copies; i++)
                    RawPrinterHelper.SendStringToPrinter(txtPrinter.Text, escPosCommands);

                MessageBox.Show("3-line label printed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnReallyBig_Click(object sender, EventArgs e)
        {
            SavePrinterPathToRegistry(txtPrinter.Text);

            string line1 = textBox1.Text;
            string line2 = textBox2.Text;

            int copies;
            if (!int.TryParse(textBoxNumOfPrint.Text, out copies) || copies <= 0)
            {
                MessageBox.Show("Please enter a valid number of copies.");
                return;
            }

            string escPosCommands =
                (char)27 + "A" + (char)50 + (char)51 + " " + line1 + "\r" +
                (char)27 + "A" + (char)50 + (char)51 + " " + line2 + "\r" +
                (char)12;

            try
            {
                for (int i = 0; i < copies; i++)
                    RawPrinterHelper.SendStringToPrinter(txtPrinter.Text, escPosCommands);

                MessageBox.Show("BIG label printed!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void PopulateSalespersonDropdown()
        {
            string[] salespersons =
            {
                "bob 215-868-2551",
                "Rayni 856-602-1491",
                "Keith Shin 267-407-7045",
                "Andy T 267-918-1738"
            };

            comboBoxSalespersons.Items.AddRange(salespersons);

            if (comboBoxSalespersons.Items.Count > 0)
                comboBoxSalespersons.SelectedIndex = 0;
        }

        private void SavePrinterPathToRegistry(string printerPath)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MyAppName");
            key.SetValue("PrinterPath", printerPath);
            key.Close();
        }

        private void btnCredit_Click(object sender, EventArgs e)
        {
            SaveLocalSettingsFile();

            var commSetting = new POSLink2.CommSetting.TcpSetting
            {
                Ip = txtIpAddress.Text,
                Port = 10009,
                Timeout = 45000
            };

            var poslink = POSLink2.POSLink2.GetPOSLink2();
            var terminal = poslink.GetTerminal(commSetting);
            if (terminal == null)
            {
                MessageBox.Show("Failed to initialize terminal. Please check the IP address and connection settings.",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (comboSaleType.SelectedItem == null || string.IsNullOrEmpty(comboSaleType.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select a valid Sale Type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboTransType.SelectedItem == null || string.IsNullOrEmpty(comboTransType.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select a valid Transaction Type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var transType = comboSaleType.SelectedItem.ToString();
            var saleReturn = comboTransType.SelectedItem.ToString();

            switch (transType)
            {
                case "CREDIT":
                    if (saleReturn == "SALE")
                    {
                        var doCreditReq = new POSLink2.Transaction.DoCreditReq
                        {
                            TransactionType = POSLink2.Const.TransType.Sale,
                        };

                        // Ensure AmountInformation & TraceInformation exist (some builds initialize lazily)
                        var t = doCreditReq.GetType();
                        var pAmt = t.GetProperty("AmountInformation");
                        var pTrace = t.GetProperty("TraceInformation");
                        var pBeh = t.GetProperty("TransactionBehavior");

                        if (pAmt != null && pAmt.GetValue(doCreditReq, null) == null)
                            pAmt.SetValue(doCreditReq, Activator.CreateInstance(pAmt.PropertyType), null);
                        if (pTrace != null && pTrace.GetValue(doCreditReq, null) == null)
                            pTrace.SetValue(doCreditReq, Activator.CreateInstance(pTrace.PropertyType), null);
                        if (pBeh != null && pBeh.GetValue(doCreditReq, null) == null)
                            pBeh.SetValue(doCreditReq, Activator.CreateInstance(pBeh.PropertyType), null);

                        var amtObj = pAmt != null ? pAmt.GetValue(doCreditReq, null) : null;
                        var traceObj = pTrace != null ? pTrace.GetValue(doCreditReq, null) : null;
                        var behObj = pBeh != null ? pBeh.GetValue(doCreditReq, null) : null;

                        if (amtObj != null)
                        {
                            var amtProp = amtObj.GetType().GetProperty("TransactionAmount");
                            if (amtProp != null) amtProp.SetValue(amtObj, txtAmount.Text, null);
                        }

                        if (traceObj != null)
                        {
                            var ecrProp = traceObj.GetType().GetProperty("EcrRefNum");
                            if (ecrProp != null) ecrProp.SetValue(traceObj, txtInvoiceNum.Text, null);
                        }

                        if (behObj != null)
                        {
                            var tipProp = behObj.GetType().GetProperty("TipRequestFlag");
                            if (tipProp != null)
                            {
                                // valid values: 0=no pretip, 1=pretip
                                tipProp.SetValue(behObj, "1", null);
                            }
                        }

                        POSLink2.Transaction.DoCreditRsp doCreditRsp;
                        var ret = terminal.Transaction.DoCredit(doCreditReq, out doCreditRsp);

                        if (ret.GetErrorCode() == POSLink2.ExecutionResult.Code.Ok)
                        {
                            string approvalCode = doCreditRsp?.HostInformation?.HostResponseCode ?? "N/A";
                            string responseCode = doCreditRsp?.ResponseCode ?? "N/A";
                            string responseMessage = doCreditRsp?.ResponseMessage ?? "N/A";

                            string message =
                                $"Approval Code: {approvalCode}\n" +
                                $"Response Code: {responseCode}\n" +
                                $"Response Message: {responseMessage}";

                            MessageBox.Show(message, "Transaction Approved",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            string errorCode = ret.GetErrorCode().ToString();
                            string errorMsg = doCreditRsp?.ResponseMessage ?? "No additional error message available.";
                            MessageBox.Show($"Error: {errorCode}\nMessage: {errorMsg}", "Transaction Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case "DEBIT":
                    if (saleReturn == "SALE")
                    {
                        var doDeditReq = new POSLink2.Transaction.DoDebitReq
                        {
                            TransactionType = POSLink2.Const.TransType.Sale,
                        };

                        doDeditReq.AmountInformation.TransactionAmount = txtAmount.Text;
                        doDeditReq.TraceInformation.EcrRefNum = txtInvoiceNum.Text;

                        POSLink2.Transaction.DoDebitRsp doDebitRsp;
                        var ret = terminal.Transaction.DoDebit(doDeditReq, out doDebitRsp);

                        if (ret.GetErrorCode() == POSLink2.ExecutionResult.Code.Ok)
                        {
                            string approvalCode = doDebitRsp.HostInformation?.HostResponseCode ?? "N/A";
                            string responseCode = doDebitRsp.ResponseCode;
                            string responseMessage = doDebitRsp.ResponseMessage;
                            string traceNumber = doDebitRsp.TraceInformation?.AuthorizationResponse ?? "N/A";

                            string message = $"Approval Code: {approvalCode}\n" +
                                             $"Response Code: {responseCode}\n" +
                                             $"Response Message: {responseMessage}\n" +
                                             $"Trace Number: {traceNumber}";
                            MessageBox.Show(message, "Transaction Approved",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            string errorCode = ret.GetErrorCode().ToString();
                            string errorMsg = doDebitRsp.ResponseMessage ?? "No additional error message available.";
                            MessageBox.Show($"Error: {errorCode}\nMessage: {errorMsg}", "Transaction Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;

                case "EBT":
                    if (saleReturn == "SALE")
                    {
                        decimal transactionAmount;
                        try
                        {
                            transactionAmount = Convert.ToDecimal(txtAmount.Text);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Please enter a valid numeric amount.", "Input Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var doEbtReq = new POSLink2.Transaction.DoEbtReq
                        {
                            TransactionType = POSLink2.Const.TransType.Sale,
                            AmountInformation = { TransactionAmount = txtAmount.Text },
                            TraceInformation = { EcrRefNum = txtInvoiceNum.Text },
                            AccountInformation = new POSLink2.Util.AccountReq
                            {
                                EbtType = "F" // F = Food Stamp (C = Cash)
                            }
                        };

                        POSLink2.Transaction.DoEbtRsp doEbtRsp;
                        var ret = terminal.Transaction.DoEbt(doEbtReq, out doEbtRsp);

                        if (ret.GetErrorCode() == POSLink2.ExecutionResult.Code.Ok)
                        {
                            string approvalCode = doEbtRsp.HostInformation?.HostResponseCode ?? "N/A";
                            string responseCode = doEbtRsp.ResponseCode;
                            string responseMessage = doEbtRsp.ResponseMessage;
                            string traceNumber = doEbtRsp.TraceInformation?.AuthorizationResponse ?? "N/A";

                            string message = $"Approval Code: {approvalCode}\n" +
                                             $"Response Code: {responseCode}\n" +
                                             $"Response Message: {responseMessage}\n" +
                                             $"Trace Number: {traceNumber}";
                            MessageBox.Show(message, "Transaction Approved",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            string errorCode = ret.GetErrorCode().ToString();
                            string errorMsg = doEbtRsp.ResponseMessage ?? "No additional error message available.";
                            MessageBox.Show($"Error: {errorCode}\nMessage: {errorMsg}", "Transaction Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Sale Return Type.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    break;

                default:
                    MessageBox.Show("Invalid transaction type or sale return type.");
                    break;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string ExtractNumericValue(string response)
        {
            var match = System.Text.RegularExpressions.Regex.Match(response, @"\d*\.\d+");
            return match.Success ? match.Value : "0.00";
        }

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = port.ReadExisting();
                Console.WriteLine("Data Received from scale: " + data);

                Invoke(new Action(() =>
                {
                    // optional UI update
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading from scale: " + ex.Message);
            }
        }

        private void btnTest1_Click(object sender, EventArgs e)
        {
            string command = "W\r\n";
            Console.WriteLine(command);

            if (port.IsOpen) port.Close();

            port.PortName = comboBoxPort.SelectedItem.ToString();
            port.BaudRate = int.Parse(comboBoxBaudRate.SelectedItem.ToString());
            port.Parity = (Parity)Enum.Parse(typeof(Parity), comboBoxParity.SelectedItem.ToString());
            port.DataBits = int.Parse(comboBoxDataBits.SelectedItem.ToString());
            port.StopBits = (StopBits)Enum.Parse(typeof(StopBits), comboBoxStopBits.SelectedItem.ToString());
            port.Handshake = Handshake.None;
            port.ReadTimeout = 1500;
            port.Encoding = System.Text.Encoding.ASCII;

            try
            {
                if (!port.IsOpen)
                {
                    port.Open();
                    MessageBox.Show($"{port.PortName} port opened successfully.", "Connection Status");
                }

                port.DiscardInBuffer();
                port.WriteLine(command);
                Console.WriteLine("Sent command: " + command);

                byte[] buffer = new byte[1024];
                int bytesRead = port.Read(buffer, 0, buffer.Length);

                Console.WriteLine("bytesRead: " + bytesRead);

                if (bytesRead > 0)
                {
                    string asciiResponse = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("ASCII response: " + asciiResponse);

                    string hexResponse = BitConverter.ToString(buffer, 0, bytesRead).Replace("-", " ");
                    Console.WriteLine("Hexadecimal response: " + hexResponse);
                }
                else
                {
                    MessageBox.Show("No response received from the scale. Check the connection.", "Scale Communication");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error communicating with scale: " + ex.Message, "Connection Status");
            }
            finally
            {
                if (port.IsOpen) port.Close();
            }
        }

        private void btnTestPassiveRead_Click(object sender, EventArgs e)
        {
            if (!port.IsOpen) port.Open();

            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead = port.Read(buffer, 0, buffer.Length);

                if (bytesRead > 0)
                {
                    string asciiResponse = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    string hexResponse = BitConverter.ToString(buffer, 0, bytesRead).Replace("-", " ");
                    Console.WriteLine("ASCII response: " + asciiResponse);
                    Console.WriteLine("Hexadecimal response: " + hexResponse);
                }
                else
                {
                    Console.WriteLine("No data received from scale in passive mode.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error communicating with scale: " + ex.Message);
            }
            finally
            {
                if (port.IsOpen) port.Close();
            }
        }

        private void btnTestPort_Click(object sender, EventArgs e)
        {
            int[] baudRates = { 9600, 19200 };
            Parity[] parities = { Parity.None, Parity.Odd, Parity.Even };
            int[] dataBitsOptions = { 7 };
            StopBits[] stopBitsOptions = { StopBits.One };
            string command = "W\r\n";

            foreach (int baudRate in baudRates)
            {
                foreach (Parity parity in parities)
                {
                    foreach (int dataBits in dataBitsOptions)
                    {
                        foreach (StopBits stopBits in stopBitsOptions)
                        {
                            try
                            {
                                using (SerialPort testPort = new SerialPort("COM1", baudRate, parity, dataBits, stopBits))
                                {
                                    testPort.Handshake = Handshake.None;
                                    testPort.ReadTimeout = 1500;
                                    testPort.Encoding = System.Text.Encoding.ASCII;

                                    testPort.Open();
                                    Console.WriteLine($"Testing {baudRate} baud, {parity} parity, {dataBits} data bits, {stopBits} stop bits");

                                    testPort.DiscardInBuffer();
                                    testPort.WriteLine(command);

                                    Thread.Sleep(500);
                                    string response = testPort.ReadExisting();

                                    if (!string.IsNullOrEmpty(response) && !response.Contains("?"))
                                    {
                                        Console.WriteLine($"Clean response found: Baud={baudRate}, Parity={parity}, DataBits={dataBits}, StopBits={stopBits}");
                                        Console.WriteLine("Response: " + response);
                                        MessageBox.Show(
                                            $"Clean response found with settings:\nBaud={baudRate}, Parity={parity}, DataBits={dataBits}, StopBits={stopBits}\n\nResponse:\n{response}",
                                            "Successful Configuration");
                                    }
                                    else
                                    {
                                        Console.WriteLine("No clean result with this configuration.");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error with Baud={baudRate}, Parity={parity}, DataBits={dataBits}, StopBits={stopBits}: {ex.Message}");
                            }
                        }
                    }
                }
            }

            MessageBox.Show("Testing complete.", "Port Test");
        }

        private void btnTestCommand_Click(object sender, EventArgs e)
        {
            string[] commands = { "W" };

            foreach (string command in commands)
            {
                Console.WriteLine($"Testing command: {command}");

                try
                {
                    if (port.IsOpen) port.Close();

                    port.Open();
                    port.DiscardInBuffer();

                    port.WriteLine(command);
                    Console.WriteLine("Sent command: " + command);

                    Thread.Sleep(500);

                    byte[] buffer = new byte[1024];
                    int bytesRead = port.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        string asciiResponse = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        string hexResponse = BitConverter.ToString(buffer, 0, bytesRead).Replace("-", " ");
                        Console.WriteLine($"ASCII response: {asciiResponse}");
                        Console.WriteLine($"Hexadecimal response: {hexResponse}");

                        if (!asciiResponse.Contains("CMD-ERR"))
                        {
                            MessageBox.Show($"Successful response found:\n{asciiResponse}", "Success");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No response received with this command.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error with command {command}: {ex.Message}");
                }
                finally
                {
                    if (port.IsOpen) port.Close();
                }
            }

            MessageBox.Show("Command testing complete.", "Test Complete");
        }

        private async void butTest2_Click(object sender, EventArgs e)
        {
            butTest2.Enabled = false;

            // Cancel any existing operation if already running
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Run(() => ListenForWeight(cancellationTokenSource.Token));
            }
            finally
            {
                butTest2.Enabled = true;
            }
        }

        private void ListenForWeight(CancellationToken token)
        {
            bool weightCaptured = false;
            string asciiResponse = "";
            string portName = "COM1";
            int baudRate = 9600;
            Parity parity = Parity.None;
            int dataBits = 7;
            StopBits stopBits = StopBits.One;
            string weightValue = "";

            try
            {
                this.Invoke(new Action(() =>
                {
                    portName = comboBoxPort.SelectedItem?.ToString() ?? throw new InvalidOperationException("Port not selected");
                    baudRate = int.Parse(comboBoxBaudRate.SelectedItem?.ToString() ?? throw new InvalidOperationException("Baud rate not selected"));
                    parity = (Parity)Enum.Parse(typeof(Parity), comboBoxParity.SelectedItem?.ToString() ?? throw new InvalidOperationException("Parity not selected"));
                    dataBits = int.Parse(comboBoxDataBits.SelectedItem?.ToString() ?? throw new InvalidOperationException("Data bits not selected"));
                    stopBits = (StopBits)Enum.Parse(typeof(StopBits), comboBoxStopBits.SelectedItem?.ToString() ?? throw new InvalidOperationException("Stop bits not selected"));
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error setting SerialPort parameters: " + ex.Message);
                return;
            }

            port.PortName = portName;
            port.BaudRate = baudRate;
            port.Parity = parity;
            port.DataBits = dataBits;
            port.StopBits = stopBits;
            port.Handshake = Handshake.None;
            port.ReadTimeout = 1500;
            port.Encoding = Encoding.ASCII;

            UpdateTextBox("Serial Port Configuration:\r\n" +
                          "  Port Name: " + port.PortName + "\r\n" +
                          "  Baud Rate: " + port.BaudRate + "\r\n" +
                          "  Parity: " + port.Parity + "\r\n" +
                          "  Data Bits: " + port.DataBits + "\r\n" +
                          "  Stop Bits: " + port.StopBits);

            try
            {
                if (port.IsOpen) port.Close();
                port.Open();
                port.DiscardInBuffer();

                UpdateTextBox("Listening for weight data...");

                while (!weightCaptured && !token.IsCancellationRequested)
                {
                    try
                    {
                        byte[] buffer = new byte[1024];
                        int bytesRead = port.Read(buffer, 0, buffer.Length);

                        if (bytesRead > 0)
                        {
                            asciiResponse += Encoding.ASCII.GetString(buffer, 0, bytesRead);

                            if (asciiResponse.Contains("ST,GS:") && asciiResponse.Contains("lb"))
                            {
                                if (asciiResponse.Contains("S:") || asciiResponse.Contains("GS:"))
                                {
                                    string sanitizedResponse = Regex.Replace(asciiResponse, @"[^a-zA-Z0-9:.lb\s]", "");
                                    var match = Regex.Match(sanitizedResponse, @"(S:|GS:)\s*(\d*\.\d+|\d+)lb");

                                    if (match.Success)
                                    {
                                        weightValue = match.Groups[2].Value;

                                        if (!weightValue.Contains("."))
                                        {
                                            int length = weightValue.Length;
                                            if (length >= 3)
                                                weightValue = weightValue.Insert(length - 2, ".");
                                        }

                                        if (weightValue != "0000.00" && weightValue != "0.00")
                                        {
                                            UpdateTextBox("Original value: " + sanitizedResponse + "\r\n" +
                                                          "Extracted weight value: " + weightValue);
                                            weightCaptured = true;
                                        }
                                        else
                                        {
                                            UpdateTextBox("Ignored zero weight value.");
                                        }
                                    }
                                    else
                                    {
                                        UpdateTextBox("Filtered out noise: " + asciiResponse);
                                    }
                                }
                            }
                        }
                    }
                    catch (IOException ex)
                    {
                        UpdateTextBox("I/O error during read: " + ex.Message);
                        break;
                    }
                    catch (OperationCanceledException)
                    {
                        UpdateTextBox("Operation was canceled.");
                        break;
                    }
                }

                UpdateTextBox("Finished listening for weight data...");
            }
            catch (Exception ex)
            {
                UpdateTextBox($"Error reading weight data: {ex.Message}");
            }
            finally
            {
                if (port.IsOpen) port.Close();

                this.Invoke(new Action(() =>
                {
                    UpdateTextBox("Process Complete : " + weightValue + "  Weight capture complete.");
                }));
            }
        }

        private void UpdateTextBox(string text)
        {
            if (txtBoxScale.InvokeRequired)
                txtBoxScale.Invoke(new Action(() => txtBoxScale.Text = text));
            else
                txtBoxScale.Text = text;
        }

      

        private void DivaDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = divaPort.ReadLine();
                Console.WriteLine("Data from Diva scale: " + data);

                this.Invoke(new Action(() =>
                {
                    lblScaleData.Text = "Received data: " + data;
                }));
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Timeout: No data received from Diva scale.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading data: " + ex.Message);
            }
        }

        private void btnCloseDiva_Click(object sender, EventArgs e)
        {
            try
            {
                if (divaPort != null && divaPort.IsOpen)
                {
                    divaPort.Close();
                    MessageBox.Show("Connection to Diva scale closed.", "Connection Status");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error closing Diva port: " + ex.Message);
            }
        }

        private void btnTestCom1_Click(object sender, EventArgs e)
        {
            bool isDeviceConnected = false;
            string response = "";

            using (SerialPort testPort = new SerialPort("COM1"))
            {
                try
                {
                    testPort.BaudRate = 9600;
                    testPort.DataBits = 8;
                    testPort.Parity = Parity.None;
                    testPort.StopBits = StopBits.One;
                    testPort.Handshake = Handshake.None;
                    testPort.ReadTimeout = 1000;
                    testPort.WriteTimeout = 1000;
                    testPort.Encoding = Encoding.ASCII;

                    testPort.Open();
                    Console.WriteLine("COM1 opened successfully.");

                    Thread.Sleep(500);
                    if (testPort.BytesToRead > 0)
                    {
                        response = testPort.ReadExisting();
                        UpdateTextBox("Data received from COM1 on connection: " + response);
                        isDeviceConnected = true;
                    }
                    else
                    {
                        UpdateTextBox("No data received on connection, sending test command...");

                        string testCommand = "TEST\r\n";
                        testPort.WriteLine(testCommand);
                        Thread.Sleep(500);

                        if (testPort.BytesToRead > 0)
                        {
                            response = testPort.ReadExisting();
                            UpdateTextBox("Data received from COM1 after test command: " + response);
                            isDeviceConnected = true;
                        }
                        else
                        {
                            UpdateTextBox("No response received from COM1 after test command.");
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    UpdateTextBox("Access to COM1 is denied.");
                }
                catch (TimeoutException)
                {
                    UpdateTextBox("Timeout: No data was received from COM1.");
                }
                catch (IOException)
                {
                    UpdateTextBox("Error accessing COM1. It may be unavailable or in use.");
                }
                catch (Exception ex)
                {
                    UpdateTextBox("Unexpected error: " + ex.Message);
                }
                finally
                {
                    if (testPort.IsOpen) testPort.Close();
                }
            }

            UpdateTextBox(isDeviceConnected
                ? $"Device detected on COM1. Response: {response}"
                : "No device detected on COM1.");
        }

        private void btnOpenPort_Click(object sender, EventArgs e)
        {
            try
            {
                scaleReader.Open();
                MessageBox.Show("Port opened successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening port: {ex.Message}");
            }
        }

        private void btnSendScaleMonitor_Click(object sender, EventArgs e)
        {
            scaleReader.SendCommand("7");
        }

        private void btnSendReset_Click(object sender, EventArgs e)
        {
            scaleReader.SendCommand("1");
        }

        private void btnSendGetSentryStatus_Click(object sender, EventArgs e)
        {
            scaleReader.SendCommand("352");
        }

        private void OnDataReceived(string data)
        {
            Invoke((MethodInvoker)delegate
            {
                MessageBox.Show($"Received Data: {data}");
            });
        }

        private void btnBatchClose_Click(object sender, EventArgs e)
        {
            try
            {
                var commSetting = new POSLink2.CommSetting.TcpSetting
                {
                    Ip = txtIpAddress.Text,
                    Port = 10009,
                    Timeout = 45000
                };

                var poslink = POSLink2.POSLink2.GetPOSLink2();
                var terminal = poslink.GetTerminal(commSetting);

                if (terminal == null)
                {
                    MessageBox.Show("Failed to initialize terminal. Please check the IP address and connection settings.",
                                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var batchCloseReq = new POSLink2.Batch.BatchCloseReq();
                POSLink2.Batch.BatchCloseRsp batchCloseRsp;
                var ret = terminal.Batch.BatchClose(batchCloseReq, out batchCloseRsp);

                if (ret.GetErrorCode() == POSLink2.ExecutionResult.Code.Ok)
                {
                    string message = $"Batch Close Successful!\n" +
                                     $"Batch Number: {batchCloseRsp.ResponseCode}\n" +
                                     $"Total Transactions: {batchCloseRsp.TotalCount}\n" +
                                     $"Total Amount: {batchCloseRsp.TotalAmount}";
                    MessageBox.Show(message, "Batch Close Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string errorCode = ret.GetErrorCode().ToString();
                    string errorMsg = batchCloseRsp.ResponseMessage ?? "No additional error message available.";
                    MessageBox.Show($"Error: {errorCode}\nMessage: {errorMsg}", "Batch Close Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error during batch close: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGetLastTransactin_Click(object sender, EventArgs e)
     

        {
            try
            {
                var commSetting = new POSLink2.CommSetting.TcpSetting
                {
                    Ip = txtIpAddress.Text,
                    Port = 10009,
                    Timeout = 45000
                };

                var poslink = POSLink2.POSLink2.GetPOSLink2();
                var terminal = poslink.GetTerminal(commSetting);

                if (terminal == null)
                {
                    MessageBox.Show("Terminal not initialized.");
                    return;
                }

                var list = new List<(int Index, string Type, decimal Amount)>();

                // Try both Credit and Debit
                var edcTypes = new[]
                {
            POSLink2.Const.EdcType.Credit,
            POSLink2.Const.EdcType.Debit
        };

                foreach (var edc in edcTypes)
                {
                    // First call to get total
                    var reqTotal = new POSLink2.Report.LocalDetailReportReq();
                    reqTotal.EdcType = edc;

                    POSLink2.Report.LocalDetailReportRsp rspTotal;
                    var ret = terminal.Report.LocalDetailReport(reqTotal, out rspTotal);

                    if (ret.GetErrorCode() != POSLink2.ExecutionResult.Code.Ok || rspTotal == null)
                        continue;

                    if (!int.TryParse(rspTotal.TotalRecord, out int total))
                        continue;

                    // Loop through each record
                    for (int i = 0; i < total; i++)
                    {
                        var req = new POSLink2.Report.LocalDetailReportReq();
                        req.EdcType = edc;
                        req.RecordNumber = i.ToString();   // IMPORTANT: string!

                        POSLink2.Report.LocalDetailReportRsp rsp;
                        var ret2 = terminal.Report.LocalDetailReport(req, out rsp);

                        if (ret2.GetErrorCode() != POSLink2.ExecutionResult.Code.Ok || rsp == null)
                            continue;

                        decimal amount = 0m;

                        string raw = rsp?.AmountInformation?.ApproveAmount;

                        if (!string.IsNullOrWhiteSpace(raw) &&
                            decimal.TryParse(raw, out var parsed))
                        {
                            amount = parsed / 100m;   // ALWAYS divide
                        }



                        list.Add((i, edc.ToString().ToLower(), amount));
                    }
                }

                // --------------------
                // Display results
                // --------------------
                if (list.Count == 0)
                {
                    MessageBox.Show("No transactions found.");
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("Index   Type     Amount");
                sb.AppendLine("---------------------------");

                foreach (var item in list)
                {
                    sb.AppendLine(
                        item.Index.ToString().PadRight(6) +
                        item.Type.PadRight(9) +
                        item.Amount.ToString("0.00")
                    );
                }

                MessageBox.Show(sb.ToString(), "All Transactions");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }



    


        private static void SetIntIfExists(object obj, string[] propNames, int value)
        {
            foreach (var name in propNames)
            {
                var p = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null || !p.CanWrite) continue;

                try
                {
                    if (p.PropertyType == typeof(int))
                        p.SetValue(obj, value);
                    else if (p.PropertyType == typeof(string))
                        p.SetValue(obj, value.ToString());
                    else
                        continue;

                    // stop after first success for this group
                    return;
                }
                catch { }
            }
        }

        private static void DumpRecurse(object obj, StringBuilder sb, string indent, int depth, int maxItems, HashSet<object> seen)
        {
            if (obj == null) { sb.AppendLine(indent + "(null)"); return; }
            if (seen.Contains(obj)) { sb.AppendLine(indent + "(ref loop)"); return; }
            seen.Add(obj);

            var t = obj.GetType();
            sb.AppendLine($"{indent}{t.FullName}");

            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                object v = null;
                try { v = p.GetValue(obj); } catch { }

                if (v == null)
                {
                    sb.AppendLine($"{indent}- {p.Name}: (null)");
                    continue;
                }

                // show simple
                if (v is string || v.GetType().IsPrimitive || v is decimal)
                {
                    sb.AppendLine($"{indent}- {p.Name}: {v}");
                    continue;
                }

                // show enumerable counts
                if (v is System.Collections.IEnumerable en && !(v is System.Collections.IDictionary))
                {
                    int c = 0;
                    foreach (var _ in en) { c++; if (c > maxItems) break; }
                    sb.AppendLine($"{indent}- {p.Name}: IEnumerable count~{c}" + (c > maxItems ? "+" : ""));
                    continue;
                }

                sb.AppendLine($"{indent}- {p.Name}: {v.GetType().FullName}");

                if (depth > 0)
                    DumpRecurse(v, sb, indent + "  ", depth - 1, maxItems, seen);
            }
        }

        // ref equality for object loop protection
        private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();
            public new bool Equals(object x, object y) => ReferenceEquals(x, y);
            public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
        }



 




        // Sets EdcType / EDCType robustly across builds
        private static bool TrySetEdcType(object req, POSLink2.Const.EdcType edc)
        {
            if (req == null) return false;

            // common names seen across builds
            var names = new[] { "EdcType", "EDCType", "EicType", "EICType" };

            var t = req.GetType();
            foreach (var name in names)
            {
                var p = t.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p == null || !p.CanWrite) continue;

                try
                {
                    // if property type is already EdcType enum
                    if (p.PropertyType.IsEnum)
                    {
                        p.SetValue(req, Enum.Parse(p.PropertyType, edc.ToString(), true));
                        return true;
                    }

                    // sometimes it's string like "CREDIT"/"DEBIT"
                    if (p.PropertyType == typeof(string))
                    {
                        p.SetValue(req, edc.ToString().ToUpperInvariant());
                        return true;
                    }

                    // sometimes it's int
                    if (p.PropertyType == typeof(int))
                    {
                        p.SetValue(req, (int)(object)edc);
                        return true;
                    }
                }
                catch
                {
                    // keep trying other names
                }
            }

            return false;
        }


 
      
      


        private string FormatTx(object obj)
        {
            try
            {
                string GetProp(string name)
                {
                    var p = obj.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (p == null) return "";
                    var v = p.GetValue(obj);
                    return v?.ToString() ?? "";
                }

                string amount = GetProp("ApproveAmount");
                if (string.IsNullOrWhiteSpace(amount)) amount = GetProp("TransactionAmount");
                if (string.IsNullOrWhiteSpace(amount)) amount = GetProp("TotalAmount");

                string ecr = GetProp("EcrRefNum");
                string appr = GetProp("HostResponseCode");
                string resp = GetProp("ResponseMessage");

                object amtObj = obj.GetType().GetProperty("AmountInformation")?.GetValue(obj);
                object traceObj = obj.GetType().GetProperty("TraceInformation")?.GetValue(obj);
                object hostObj = obj.GetType().GetProperty("HostInformation")?.GetValue(obj);

                if (amtObj != null && string.IsNullOrWhiteSpace(amount))
                {
                    amount = amtObj.GetType().GetProperty("ApproveAmount")?.GetValue(amtObj)?.ToString()
                          ?? amtObj.GetType().GetProperty("TransactionAmount")?.GetValue(amtObj)?.ToString()
                          ?? "";
                }

                if (traceObj != null && string.IsNullOrWhiteSpace(ecr))
                {
                    ecr = traceObj.GetType().GetProperty("EcrRefNum")?.GetValue(traceObj)?.ToString() ?? "";
                }

                if (hostObj != null && string.IsNullOrWhiteSpace(appr))
                {
                    appr = hostObj.GetType().GetProperty("HostResponseCode")?.GetValue(hostObj)?.ToString() ?? "";
                }

                if (string.IsNullOrWhiteSpace(resp))
                    resp = GetProp("ResponseCode");

                return
                    $"Amount:   {amount}\r\n" +
                    $"ECR Ref:  {ecr}\r\n" +
                    $"Approval: {appr}\r\n" +
                    $"Response: {resp}";
            }
            catch
            {
                return obj?.ToString() ?? "";
            }
        }

        private void btnPreAddTip_Click(object sender, EventArgs e)
        {
        }

        private static string AsciiToHex(string s)
        {
            var sb = new StringBuilder(s.Length * 2);
            foreach (var c in s) sb.Append(((int)c).ToString("X2"));
            return sb.ToString();
        }

        private static string HexToAscii(string hex)
        {
            hex = hex.Replace(" ", "");
            var bytes = new byte[hex.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            return Encoding.ASCII.GetString(bytes);
        }

        private void btnPassThru_Click(object sender, EventArgs e)
        {
            try
            {
                var commSetting = new POSLink2.CommSetting.TcpSetting
                {
                    Ip = txtIpAddress.Text,
                    Port = 10009,
                    Timeout = 45000
                };
                var poslink = POSLink2.POSLink2.GetPOSLink2();
                var terminal = poslink.GetTerminal(commSetting);
                if (terminal == null)
                {
                    MessageBox.Show("Failed to initialize terminal. Check IP/port.", "Init Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string prompt = "Enter Phone Number:";
                string payloadHex = "09" + "01" + "00" + "0A" + AsciiToHex(prompt);

                var asmList = AppDomain.CurrentDomain.GetAssemblies()
                                 .Where(a => a.GetName().Name.StartsWith("POSLink2"))
                                 .ToList();

                Type reqType = asmList.SelectMany(a => a.GetTypes())
                                      .FirstOrDefault(t => t.Name.Equals("PassThruReq", StringComparison.OrdinalIgnoreCase));
                Type rspType = asmList.SelectMany(a => a.GetTypes())
                                      .FirstOrDefault(t => t.Name.Equals("PassThruRsp", StringComparison.OrdinalIgnoreCase));

                if (reqType == null || rspType == null)
                {
                    MessageBox.Show("Passthru types not found in this POSLink2 build.", "SDK Missing Types",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                object req = Activator.CreateInstance(reqType);

                var passthruDataProp = reqType.GetProperty("PassthruData");
                if (passthruDataProp == null)
                {
                    MessageBox.Show("Req.PassthruData property not found.", "SDK Incompatibility",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                passthruDataProp.SetValue(req, payloadHex);

                var utilObj = terminal.GetType().GetProperty("Utility")?.GetValue(terminal)
                            ?? terminal.GetType().GetProperty("Util")?.GetValue(terminal);

                if (utilObj == null)
                {
                    MessageBox.Show("Terminal.Utility service not found in this SDK build.", "SDK Incompatibility",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var utilType = utilObj.GetType();
                var mPass = utilType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                    .FirstOrDefault(m =>
                                        (m.Name.Equals("PassThru", StringComparison.OrdinalIgnoreCase) ||
                                         m.Name.Equals("PassThrough", StringComparison.OrdinalIgnoreCase)) &&
                                        m.GetParameters().Length == 2);

                if (mPass == null)
                {
                    MessageBox.Show("Utility.PassThru method not found.", "SDK Incompatibility",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                object[] args = new object[] { req, null };
                var ret = mPass.Invoke(utilObj, args);
                object rsp = args[1];

                var getErr = ret?.GetType().GetMethod("GetErrorCode");
                var code = getErr?.Invoke(ret, null);
                string codeStr = code?.ToString() ?? "";

                bool success = codeStr.Equals("Ok", StringComparison.OrdinalIgnoreCase) ||
                               codeStr.EndsWith(".Ok", StringComparison.OrdinalIgnoreCase) ||
                               codeStr == "0";

                if (success)
                {
                    var rspDataProp = rspType.GetProperty("PassthruData");
                    string raw = rspDataProp?.GetValue(rsp)?.ToString() ?? "";

                    string ascii;
                    try { ascii = HexToAscii(raw); }
                    catch { ascii = raw; }

                    string phone = Regex.Replace(ascii, @"\D", "");
                    MessageBox.Show($"Customer phone: {phone}", "Phone Captured",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var rspMsg = rspType.GetProperty("ResponseMessage")?.GetValue(rsp)?.ToString();
                    MessageBox.Show($"Passthru failed: {codeStr}\n{rspMsg}", "Passthru Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in Passthru: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveLocalSettingsFile();

            bool showDate = checkBoxDate.Checked;
            string dateText = DateTime.Now.ToString("MM/dd/yyyy");
            int lines = GetIntSafe(comboBoxLines.Text, 1, 4, 2);
            int copies = GetIntSafe(textBoxNumOfPrint.Text, 1, 50, 1);

            bool reverse = chkBoxReverseText.Checked;
            string textMode = reverse ? "R" : "N";

            string[] items = new[]
            {
                textBox1.Text,
                textBox2.Text,
                textBox3.Text,
                comboBoxSalespersons.Text
            };

            const int labelWidthDots = 406;   // 2" at 203dpi
            const int labelHeightDots = 203;  // 1" at 203dpi

            int x = 20;
            int topMargin = showDate ? 40 : 8;
            int gap = 6;

            int font = 4;
            int wMult = 2;
            const int baseFontHeightDots = 20;

            int available = labelHeightDots - topMargin - 6 - ((lines - 1) * gap);
            int hMult = Math.Max(1, Math.Min(4, available / (lines * baseFontHeightDots)));

            int maxChars = CalcMaxChars(labelWidthDots, x, font, wMult);

            int lineHeight = baseFontHeightDots * hMult;
            int[] yPos = new int[lines];
            for (int i = 0; i < lines; i++)
                yPos[i] = topMargin + i * (lineHeight + gap);

            var sb = new StringBuilder();
            sb.Append("N\r\n");
            sb.Append("q406\r\n");
            sb.Append("Q203,16\r\n");

            if (showDate)
            {
                int dateFont = 4;
                int dateW = 1;
                int dateH = 1;
                int dateX = 20;
                int dateY = 4;

                sb.Append($"A{dateX},{dateY},0,{dateFont},{dateW},{dateH},N,\"{dateText}\"\r\n");
            }

            for (int i = 0; i < lines; i++)
            {
                string txt = (i < items.Length) ? (items[i] ?? "") : "";
                if (string.IsNullOrWhiteSpace(txt)) continue;

                txt = txt.Trim();

                string printable = reverse
                    ? PadOrTrim(txt, maxChars)
                    : TrimTo(txt, maxChars);

                sb.Append($"A{x},{yPos[i]},0,{font},{wMult},{hMult},{textMode},\"{printable}\"\r\n");
            }

            sb.Append($"P{copies}\r\n");

            bool ok = SnbcRawPrinterHelper.SendRawString(textBox10.Text, sb.ToString(), out string err);
            // MessageBox.Show(ok ? "SNBC EPL SENT" : ("SNBC FAILED:\r\n" + err));
        }

        private static int GetIntSafe(string s, int min, int max, int fallback)
        {
            if (!int.TryParse(s, out int v)) return fallback;
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        private static int CalcMaxChars(int labelWidthDots, int x, int font, int wMult)
        {
            int baseCharWidthDots = (font == 4) ? 12 : 10;
            int usable = Math.Max(0, labelWidthDots - x - 10);
            int perChar = baseCharWidthDots * Math.Max(1, wMult);
            int chars = usable / Math.Max(1, perChar);
            return Math.Max(6, chars);
        }

        private static string PadOrTrim(string s, int maxChars)
        {
            if (maxChars <= 0) return "";
            s = s ?? "";
            if (s.Length > maxChars) return s.Substring(0, maxChars);
            return s.PadRight(maxChars, ' ');
        }

        private static string TrimTo(string s, int maxChars)
        {
            if (maxChars <= 0) return "";
            s = s ?? "";
            if (s.Length > maxChars) return s.Substring(0, maxChars);
            return s;
        }

        private void btnSnbcHello_Click(object sender, EventArgs e)
        {
            string itemName = "pepsi soda";
            string upc = "12345";

            string epl =
                "N\r\n" +
                "q640\r\n" +
                "Q180,0\r\n" +
                $"A20,15,0,4,1,1,N,\"{EscapeEpl(itemName)}\"\r\n" +
                $"A20,55,0,3,1,1,N,\"UPC: {EscapeEpl(upc)}\"\r\n" +
                $"B20,90,0,3,2,6,100,B,\"{EscapeEpl(upc)}\"\r\n" +
                "P1\r\n";

            bool ok = SnbcRawPrinterHelper.SendRawString(textBox10.Text, epl, out string err);
            MessageBox.Show(ok ? "SNBC EPL SENT" : ("SNBC FAILED:\r\n" + err));
        }

        private static string EscapeEpl(string s)
        {
            return (s ?? "").Replace("\"", "");
        }

        private void label3_Click(object sender, EventArgs e)
        {
        }




        private static string DumpAny(object o, int depth = 0)
        {
            if (o == null) return "";
            if (depth > 4) return "…(max depth)…";

            var ind = new string(' ', depth * 2);
            var sb = new StringBuilder();

            var t = o.GetType();
            sb.AppendLine($"{ind}{t.FullName}");

            foreach (var p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                object v = null;
                try { v = p.GetValue(o); } catch { }

                if (v == null) continue;

                // strings
                if (v is string s)
                {
                    if (!string.IsNullOrWhiteSpace(s))
                        sb.AppendLine($"{ind}- {p.Name}: {s}");
                    continue;
                }

                // primitive-ish
                if (v.GetType().IsPrimitive || v is decimal || v is DateTime)
                {
                    sb.AppendLine($"{ind}- {p.Name}: {v}");
                    continue;
                }

                // IEnumerable (list)
                if (v is System.Collections.IEnumerable en && !(v is System.Collections.IDictionary))
                {
                    int count = 0;
                    sb.AppendLine($"{ind}- {p.Name}:");
                    foreach (var item in en)
                    {
                        count++;
                        sb.AppendLine($"{ind}  [{count}]");
                        sb.AppendLine(DumpAny(item, depth + 2));
                        if (count >= 10) { sb.AppendLine($"{ind}  …(truncated)…"); break; }
                    }
                    if (count == 0) sb.AppendLine($"{ind}  (empty)");
                    continue;
                }

                // nested object
                sb.AppendLine($"{ind}- {p.Name}:");
                sb.AppendLine(DumpAny(v, depth + 1));
            }

            return sb.ToString();
        }




    }
}

/*
    IMPORTANT FIX:
    You had this at the bottom:

        namespace POSLink2.Const { class TipRequestFlag { } }

    That conflicts with the real POSLink2 SDK namespace/types and can cause compile errors
    (or weird type resolution). It MUST NOT exist.

    So it has been removed.
*/
