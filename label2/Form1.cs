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



namespace label2
{
    public partial class Form1 : Form
    {
        private SerialPort port; // Declare SerialPort at the class level
        private SerialPort divaPort;
        private ScaleReader scaleReader;
        public Form1()
        {
            InitializeComponent();
            PopulateSalespersonDropdown();
            txtPrinter.Text = GetPrinterPathFromRegistry();
            textBox1.Text = "Pks Payments";
            textBox2.Text = "Store : ";
            textBox3.Text = "Mid : ";
            txtAmount.Text = "5";
            txtInvoiceNum.Text = "35158415488";
            txtIpAddress.Text = "192.168.1.183";


            string[] saleType = { "CREDIT", "DEBIT", "EBT" };
            comboSaleType.Items.AddRange(saleType);
            string[] transType = { "SALE", "RETURN" };
            comboTransType.Items.AddRange(transType);



            // Initialize the SerialPort (without DataReceived handler)
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

            return string.Empty; // Return empty if the registry key or value doesn't exist
        }


        private void btnPrint_Click(object sender, EventArgs e)
        {
            // Network path to the shared printer
            //  string printerPath = txtPrinter.Text;
            SavePrinterPathToRegistry(txtPrinter.Text);


            string line1 = textBox1.Text;   // Replace with actual value
            string line2 = textBox2.Text;   // Replace with actual value
            string line3 = textBox3.Text;   // Replace with actual value
            string line4 = comboBoxSalespersons.Text;
            // Get the number of copies from textBox5
            int copies;
            if (!int.TryParse(textBox5.Text, out copies) || copies <= 0)
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

                // Loop to send the ESC/POS commands the specified number of times (copies)
                for (int i = 0; i < copies; i++)
                {
                    RawPrinterHelper.SendStringToPrinter(txtPrinter.Text, escPosCommands);
                }

                MessageBox.Show("Print job sent successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send print job: " + ex.Message);
            }
        }


        private void PopulateSalespersonDropdown()
        {
            // Example list of salesperson names
            string[] salespersons = { "bob 215-868-2551", "Rayni 856-602-1491", "Keith Shin 267-407-7045" , "Andy T 267-918-1738" };

            // Add items to the ComboBox
            comboBoxSalespersons.Items.AddRange(salespersons);

            // Optionally, select the first salesperson by default
            if (comboBoxSalespersons.Items.Count > 0)
            {
                comboBoxSalespersons.SelectedIndex = 0; // Select the first item by default
            }
        }
        private void SavePrinterPathToRegistry(string printerPath)
        {
            // Create or open a registry key for your application
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MyAppName");

            // Save the printer path to the registry
            key.SetValue("PrinterPath", printerPath);

            // Close the registry key after saving
            key.Close();
        }

        private void btnCredit_Click(object sender, EventArgs e)
        {
            // Set up communication settings
            var commSetting = new POSLink2.CommSetting.TcpSetting
            {
                Ip = txtIpAddress.Text,
                Port = 10009,
                Timeout = 45000
            };

            // Initialize POSLink
            var poslink = POSLink2.POSLink2.GetPOSLink2();
                  var terminal = poslink.GetTerminal(commSetting);
            if (terminal == null)
            {
                MessageBox.Show("Failed to initialize terminal. Please check the IP address and connection settings.", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (comboSaleType.SelectedItem == null || string.IsNullOrEmpty(comboSaleType.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select a valid Sale Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop further execution
            }

            if (comboTransType.SelectedItem == null || string.IsNullOrEmpty(comboTransType.SelectedItem.ToString()))
            {
                MessageBox.Show("Please select a valid Transaction Type.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop further execution
            }

            var transType = comboSaleType.SelectedItem.ToString();
            var saleReturn = comboTransType.SelectedItem.ToString();

            MessageBox.Show("this is transtyupe./  " + transType);

            switch (transType)
            {
                case ("CREDIT"):
                    if (saleReturn == "SALE")
                    {
                        var doCreditReq = new POSLink2.Transaction.DoCreditReq
                        {
                            TransactionType = POSLink2.Const.TransType.Sale, // Replace "Sale" with the appropriate enum value
                        };
                        doCreditReq.AmountInformation.TransactionAmount = txtAmount.Text;
                        doCreditReq.TraceInformation.EcrRefNum = txtInvoiceNum.Text;

                        // Create the credit response
                        var doCreditRsp = new POSLink2.Transaction.DoCreditRsp();

                        // Execute the DoCredit method
                        var ret = terminal.Transaction.DoCredit(doCreditReq, out doCreditRsp);
                        var retResponse = doCreditRsp;

                        if (ret.GetErrorCode() == POSLink2.ExecutionResult.Code.Ok)
                        {
                            // Access approval-related details using retResponse
                            string approvalCode = retResponse.HostInformation?.HostResponseCode ?? "N/A";
                            string responseCode = retResponse.ResponseCode;
                            string responseMessage = retResponse.ResponseMessage;
                            string traceNumber = retResponse.TraceInformation?.AuthorizationResponse ?? "N/A";

                            // Display the information
                            string message = $"Approval Code: {approvalCode}\n" +
                                             $"Response Code: {responseCode}\n" +
                                             $"Response Message: {responseMessage}\n" +
                                             $"Trace Number: {traceNumber}";
                            MessageBox.Show(message, "Transaction Approved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // Handle the error case and show more details if available
                            string errorCode = ret.GetErrorCode().ToString();
                            string errorMsg = retResponse.ResponseMessage ?? "No additional error message available.";
                            MessageBox.Show($"Error: {errorCode}\nMessage: {errorMsg}", "Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                    else
                    {
                        break;
                    }

                case ("DEBIT"):
                    if (saleReturn == "SALE")
                    {
                        var doDeditReq = new POSLink2.Transaction.DoDebitReq
                        {
                            TransactionType = POSLink2.Const.TransType.Sale, // Replace "Sale" with the appropriate enum value
                        };
                        doDeditReq.AmountInformation.TransactionAmount = txtAmount.Text;
                        doDeditReq.TraceInformation.EcrRefNum = txtInvoiceNum.Text;

                        // Create the debit response
                        var doDebitRsp = new POSLink2.Transaction.DoDebitRsp();

                        // Execute the DoDebit method
                        var ret = terminal.Transaction.DoDebit(doDeditReq, out doDebitRsp);
                        var retResponse = doDebitRsp;

                        if (ret.GetErrorCode() == POSLink2.ExecutionResult.Code.Ok)
                        {
                            // Access approval-related details using retResponse
                            string approvalCode = retResponse.HostInformation?.HostResponseCode ?? "N/A";
                            string responseCode = retResponse.ResponseCode;
                            string responseMessage = retResponse.ResponseMessage;
                            string traceNumber = retResponse.TraceInformation?.AuthorizationResponse ?? "N/A";

                            // Display the information
                            string message = $"Approval Code: {approvalCode}\n" +
                                             $"Response Code: {responseCode}\n" +
                                             $"Response Message: {responseMessage}\n" +
                                             $"Trace Number: {traceNumber}";
                            MessageBox.Show(message, "Transaction Approved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // Handle the error case and show more details if available
                            string errorCode = ret.GetErrorCode().ToString();
                            string errorMsg = retResponse.ResponseMessage ?? "No additional error message available.";
                            MessageBox.Show($"Error: {errorCode}\nMessage: {errorMsg}", "Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                    else
                    {
                        break;
                    }

                case ("EBT"):

                    if (saleReturn == "SALE")
                    {
                        // Validate and convert amount
                        decimal transactionAmount;
                        try
                        {
                            transactionAmount = Convert.ToDecimal(txtAmount.Text);
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Please enter a valid numeric amount.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        var doEbtReq = new POSLink2.Transaction.DoEbtReq
                        {
                            TransactionType = POSLink2.Const.TransType.Sale, // Ensure this is the correct type for a sale
                            AmountInformation = { TransactionAmount = txtAmount.Text },
                            TraceInformation = { EcrRefNum = txtInvoiceNum.Text },
                            AccountInformation = new POSLink2.Util.AccountReq
                            {
                                EbtType = "F" // Set to "F" for Food Stamp
                                // EbtType = "C" // Set to "C" for Food Stamp Cash
                            }
                        };

                        // Create the EBT response
                        var doEbtRsp = new POSLink2.Transaction.DoEbtRsp();

                        // Execute the DoEbt method
                        var ret = terminal.Transaction.DoEbt(doEbtReq, out doEbtRsp);
                        var retResponse = doEbtRsp;

                        if (ret.GetErrorCode() == POSLink2.ExecutionResult.Code.Ok)
                        {
                            // Access approval-related details using retResponse
                            string approvalCode = retResponse.HostInformation?.HostResponseCode ?? "N/A";
                            string responseCode = retResponse.ResponseCode;
                            string responseMessage = retResponse.ResponseMessage;
                            string traceNumber = retResponse.TraceInformation?.AuthorizationResponse ?? "N/A";

                            // Display the information
                            string message = $"Approval Code: {approvalCode}\n" +
                                             $"Response Code: {responseCode}\n" +
                                             $"Response Message: {responseMessage}\n" +
                                             $"Trace Number: {traceNumber}";
                            MessageBox.Show(message, "Transaction Approved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            // Handle the error case and show more details if available
                            string errorCode = ret.GetErrorCode().ToString();
                            string errorMsg = retResponse.ResponseMessage ?? "No additional error message available.";
                            MessageBox.Show($"Error: {errorCode}\nMessage: {errorMsg}", "Transaction Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid Sale Return Type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    break;

                default:
                    MessageBox.Show("Invalid transaction type or sale return type.");
                    break;
           
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // Exits the entire application immediately
            Environment.Exit(0);
        }
   
      


        // Helper method to extract numeric data from the response
        private string ExtractNumericValue(string response)
        {
            // Use regular expression to find the first numeric value in the response
            var match = System.Text.RegularExpressions.Regex.Match(response, @"\d*\.\d+");
            return match.Success ? match.Value : "0.00"; // Return "0.00" if no match is found
        }

       

        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Read all available data from the buffer
                string data = port.ReadExisting();

                // Log the received data to the console
                Console.WriteLine("Data Received from scale: " + data);

                // Optional: Update the UI, if needed
                Invoke(new Action(() =>
                {
                    // Example: Log to a UI component, e.g., a TextBox
                    // textBoxOutput.AppendText(data + Environment.NewLine);
                }));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading from scale: " + ex.Message);
            }
        }


        private void btnTest1_Click(object sender, EventArgs e)
        {
            // Set the command to send
           string command = "W\r\n";
            // string command = "W\r\n";  // Adjust terminators if needed
            Console.WriteLine(command);

            // Check if the port is open and close it if needed
            if (port.IsOpen)
            {
                port.Close();
            }

            // Set serial port parameters
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
                // Check if the port is already open
                if (!port.IsOpen)
                {
                    port.Open();
                    MessageBox.Show("COM1 port opened successfully.", "Connection Status");
            
                }
                else
                {
                    MessageBox.Show("COM1 port is already open.", "Connection Status");
                }

                // Test communication by sending a simple command and reading a response
                port.DiscardInBuffer();  // Clear buffer before sending
                port.WriteLine(command);  // Send command to request weight
                Console.WriteLine("Sent command: " + command);

                // Check if any response is available
                byte[] buffer = new byte[1024];
                int bytesRead = port.Read(buffer, 0, buffer.Length);

                Console.WriteLine("bytesRead: " + bytesRead);


                if (bytesRead > 0)
                {
                    // Convert to ASCII for readable characters
                    string asciiResponse = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("ASCII response: " + asciiResponse);

                    // Convert to Hexadecimal format
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
                MessageBox.Show("7 Error communicating with scale: " + ex.Message, "Connection Status");
            }
        }


      

        private void btnTestPassiveRead_Click(object sender, EventArgs e)
        {
            if (!port.IsOpen)
            {
                port.Open();
            }

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
                if (port.IsOpen)
                {
                    port.Close();
                }
            }
        }

        private void btnTestPort_Click(object sender, EventArgs e)
        {
            // Define possible values for each setting
          //  int[] baudRates = { 9600, 19200, 38400, 57600, 115200 };
            int[] baudRates = { 9600, 19200 };
            Parity[] parities = { Parity.None, Parity.Odd, Parity.Even };
            int[] dataBitsOptions = { 7};
            StopBits[] stopBitsOptions = { StopBits.One };
            //StopBits[] stopBitsOptions = { StopBits.One, StopBits.OnePointFive, StopBits.Two };
            string command = "W\r\n"; // Sample command, adjust if needed

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
                                // Initialize serial port with current combination
                                using (SerialPort testPort = new SerialPort("COM1", baudRate, parity, dataBits, stopBits))
                                {
                                    testPort.Handshake = Handshake.None;
                                    testPort.ReadTimeout = 1500;
                                    testPort.Encoding = System.Text.Encoding.ASCII;

                                    testPort.Open();
                                    Console.WriteLine($"Testing {baudRate} baud, {parity} parity, {dataBits} data bits, {stopBits} stop bits");

                                    // Send test command
                                    testPort.DiscardInBuffer();
                                    testPort.WriteLine(command);

                                    // Capture the response
                                    System.Threading.Thread.Sleep(500); // Allow some time for response
                                    string response = testPort.ReadExisting();

                                    // Check for clean result
                                    if (!string.IsNullOrEmpty(response) && !response.Contains("?"))
                                    {
                                        Console.WriteLine($"Clean response found with settings: Baud={baudRate}, Parity={parity}, DataBits={dataBits}, StopBits={stopBits}");
                                        Console.WriteLine("Response: " + response);
                                        MessageBox.Show($"Clean response found with settings:\nBaud={baudRate}, Parity={parity}, DataBits={dataBits}, StopBits={stopBits}\n\nResponse:\n{response}", "Successful Configuration");
                                    }
                                    else
                                    {
                                        Console.WriteLine("No clean result with this configuration.");
                                    }

                                    testPort.Close();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error with combination Baud={baudRate}, Parity={parity}, DataBits={dataBits}, StopBits={stopBits}: {ex.Message}");
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
                    if (port.IsOpen)
                    {
                        port.Close();
                    }

                    port.Open();
                    port.DiscardInBuffer();

                    // Send the command
                    port.WriteLine(command);
                    Console.WriteLine("Sent command: " + command);

                    System.Threading.Thread.Sleep(500); // Allow time for response

                    // Read the response
                    byte[] buffer = new byte[1024];
                    int bytesRead = port.Read(buffer, 0, buffer.Length);

                    if (bytesRead > 0)
                    {
                        // Define and assign asciiResponse
                        string asciiResponse = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        string hexResponse = BitConverter.ToString(buffer, 0, bytesRead).Replace("-", " ");
                        Console.WriteLine($"ASCII response: {asciiResponse}");
                        Console.WriteLine($"Hexadecimal response: {hexResponse}");

                        // Check for CMD-ERR in the last command attempt
                        if (command == commands[commands.Length - 1] && asciiResponse.Contains("CMD-ERR"))
                        {
                            Console.WriteLine("Command Error received in the last command.");
                        }

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
                    if (port.IsOpen)
                    {
                        port.Close();
                    }
                }
            }


            MessageBox.Show("Command testing complete.", "Test Complete");
        }

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


        private async void butTest2_Click(object sender, EventArgs e)
        {
            // Disable the button to prevent multiple clicks
            butTest2.Enabled = false;

            // Cancel any existing operation if already running
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }

            // Initialize a new CancellationTokenSource for the new task
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                await Task.Run(() => ListenForWeight(cancellationTokenSource.Token));
            }
            finally
            {
                // Re-enable the button once the task is complete
                butTest2.Enabled = true;
            }
        }



        private void ListenForWeight(CancellationToken token)
        {
            bool weightCaptured = false;
            string asciiResponse = "";
            // Retrieve combo box values safely on the UI thread
            string portName = "COM1";
            int baudRate = 9600;
            Parity parity = Parity.None;
            int dataBits = 7;
            StopBits stopBits = StopBits.One;
            string weightData = "";
            string weightValue = "";


            try
            {
                // Access each combo box on the UI thread
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

            // Configure the serial port
            port.PortName = portName;
            port.BaudRate = baudRate;
            port.Parity = parity;
            port.DataBits = dataBits;
            port.StopBits = stopBits;

            // Console output to verify the values
            UpdateTextBox("Serial Port Configuration:");
            UpdateTextBox("  Port Name: " + port.PortName);
            UpdateTextBox("  Baud Rate: " + port.BaudRate);
            UpdateTextBox("  Parity: " + port.Parity);
            UpdateTextBox("  Data Bits: " + port.DataBits);
            UpdateTextBox("  Stop Bits: " + port.StopBits);


            try
            {
                if (port.IsOpen)
                {
                    port.Close();
                }

                port.Open();
                port.DiscardInBuffer();

                UpdateTextBox("Listening for weight data...");

                while (!weightCaptured && !token.IsCancellationRequested)
                {
                    try
                    {
                        // Read bytes into buffer, assuming data will fit within 1024 bytes per read
                        byte[] buffer = new byte[1024];
                        int bytesRead = port.Read(buffer, 0, buffer.Length);

                        if (bytesRead > 0)
                        {
                            // Append the current read to asciiResponse
                            asciiResponse += System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);

                            // Check for complete packet (using '<' as end delimiter for example)
                            if (asciiResponse.Contains("ST,GS:") && asciiResponse.Contains("lb"))
                            {
                                if (asciiResponse.Contains("S:") || asciiResponse.Contains("GS:"))
                                {
                                    // Step 1: Sanitize input by removing extraneous characters
                                    string sanitizedResponse = Regex.Replace(asciiResponse, @"[^a-zA-Z0-9:.lb\s]", "");

                                    // Step 2: Use Regex to find patterns like "S: <value>lb" or "GS: <value>lb"
                                    // This will strictly match only valid weight data
                                    var match = Regex.Match(sanitizedResponse, @"(S:|GS:)\s*(\d*\.\d+|\d+)lb");


                                    if (match.Success)
                                    {
                                        // Extract the numeric weight part (e.g., "0.79")
                                       weightValue = match.Groups[2].Value;
                                        // Check if there's no decimal in the weight
                                        if (!weightValue.Contains("."))
                                        {
                                            // Insert decimal before the last two digits
                                            int length = weightValue.Length;
                                            weightValue = weightValue.Insert(length - 2, ".");
                                        }
                                        // Ignore zero values
                                        if (weightValue != "0000.00" && weightValue != "0.00") 
                                        {

                                            UpdateTextBox("Original value " + sanitizedResponse);
                                            UpdateTextBox("Extracted weight value: " + weightValue);
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
                if (port.IsOpen)
                {
                    port.Close();
                }
                // Show a completion message on the UI thread
                this.Invoke(new Action(() =>
                {
                    UpdateTextBox( "Process Complete : " + weightValue + "Weight capture complete.");
                }));
            }
        }


        private void UpdateTextBox(string text)
        {
            if (txtBoxScale.InvokeRequired)
            {
                txtBoxScale.Invoke(new Action(() => txtBoxScale.Text = text));
            }
            else
            {
                txtBoxScale.Text = text;
            }
        }

        // Method to stop listening and cancel the operation
        private void StopListeningForWeight()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
        }

        private void btnDetectDiva_Click(object sender, EventArgs e)
        {
            try
            {
                // Initialize SerialPort with Diva scale's settings
                divaPort = new SerialPort
                {
                    PortName = "COM1",            // Replace with actual COM port or provide a UI option to select it
                    BaudRate = 9600,              // Set according to Diva scale specs
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    ReadTimeout = 3000,           // Adjust as needed
                    WriteTimeout = 3000,
                    Encoding = System.Text.Encoding.ASCII
                };

                divaPort.DataReceived += DivaDataReceivedHandler; // Event handler for receiving data

                // Open the connection to the Diva scale
                divaPort.Open();
                MessageBox.Show("Connection to Diva scale opened. Listening for data...", "Connection Status");

                // Optionally send an initialization command if needed by the scale
                // divaPort.WriteLine("INIT_COMMAND"); // Replace "INIT_COMMAND" if Diva requires one to start sending weight
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening connection to Diva scale: " + ex.Message, "Connection Error");
            }
        }

        private void DivaDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                // Read data from Diva scale when available
                string data = divaPort.ReadLine();
                Console.WriteLine("Data from Diva scale: " + data);

                // Display the received data in a TextBox or label if desired
                this.Invoke(new Action(() => {
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
            if (divaPort != null && divaPort.IsOpen)
            {
                divaPort.Close();
                MessageBox.Show("Connection to Diva scale closed.", "Connection Status");
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
                    // Configure the serial port with common settings
                    testPort.BaudRate = 9600;    // Typical default setting; adjust if needed
                    testPort.DataBits = 8;       // 8 data bits (common)
                    testPort.Parity = Parity.None;
                    testPort.StopBits = StopBits.One;
                    testPort.Handshake = Handshake.None;
                    testPort.ReadTimeout = 1000; // Short timeout for testing
                    testPort.WriteTimeout = 1000;
                    testPort.Encoding = System.Text.Encoding.ASCII;

                    // Open the port
                    testPort.Open();
                    Console.WriteLine("COM1 opened successfully.");

                    // Attempt to read any initial data (some devices send data on connection)
                    System.Threading.Thread.Sleep(500); // Short delay for potential data arrival
                    if (testPort.BytesToRead > 0)
                    {
                        response = testPort.ReadExisting();
                        UpdateTextBox("Data received from COM1 on connection: " + response);
                        isDeviceConnected = true;
                    }
                    else
                    {
                        UpdateTextBox("No data received on connection, sending test command...");

                        // Send a test command to see if the device responds (use a common string like "TEST")
                        string testCommand = "TEST\r\n";
                        testPort.WriteLine(testCommand);
                        System.Threading.Thread.Sleep(500); // Wait for response

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
                    // Ensure the port is closed after checking
                    if (testPort.IsOpen)
                    {
                        testPort.Close();
                    }
                }
            }

            // Show the result in a MessageBox
            UpdateTextBox(isDeviceConnected ? $"Device detected on COM1. Response: {response}" : "No device detected on COM1.");
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
            // Sending SCALE_MONITOR command to test scale response
            scaleReader.SendCommand("7"); // Send SCALE_MONITOR (assuming command 7)
        }

        private void btnSendReset_Click(object sender, EventArgs e)
        {
            // Sending SCANNER_RESET command to reset the device
            scaleReader.SendCommand("1"); // Send SCANNER_RESET (assuming command 1)
        }

        private void btnSendGetSentryStatus_Click(object sender, EventArgs e)
        {
            // Sending DIO_GET_SCALE_SENTRY_STATUS to test if it retrieves scale status
            scaleReader.SendCommand("352"); // Send DIO_GET_SCALE_SENTRY_STATUS (command 352)
        }

        private void OnDataReceived(string data)
        {
            // Display received data from the scale
            Invoke((MethodInvoker)delegate
            {
                MessageBox.Show($"Received Data: {data}");
            });
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure the serial port is closed on exit
            scaleReader.Close();
        }

        private void btnBatchClose_Click(object sender, EventArgs e)
        {
            try
            {
                // Set up communication settings for the terminal
                var commSetting = new POSLink2.CommSetting.TcpSetting
                {
                    Ip = txtIpAddress.Text,
                    Port = 10009,
                    Timeout = 45000
                };

                // Initialize POSLink and terminal
                var poslink = POSLink2.POSLink2.GetPOSLink2();
                var terminal = poslink.GetTerminal(commSetting);

                if (terminal == null)
                {
                    MessageBox.Show("Failed to initialize terminal. Please check the IP address and connection settings.",
                                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Create the batch close request
                var batchCloseReq = new POSLink2.Batch.BatchCloseReq();
                           // Execute the batch close
                var batchCloseRsp = new POSLink2.Batch.BatchCloseRsp();
                var ret = terminal.Batch.BatchClose(batchCloseReq, out batchCloseRsp);

                if (ret.GetErrorCode() == POSLink2.ExecutionResult.Code.Ok)
                {
                    // Display success message with details
                    string message = $"Batch Close Successful!\n" +
                                     $"Batch Number: {batchCloseRsp.ResponseCode}\n" +
                                     $"Total Transactions: {batchCloseRsp.TotalCount}\n" +
                                     $"Total Amount: {batchCloseRsp.TotalAmount}";
                    MessageBox.Show(message, "Batch Close Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Handle error case
                    string errorCode = ret.GetErrorCode().ToString();
                    string errorMsg = batchCloseRsp.ResponseMessage ?? "No additional error message available.";
                    MessageBox.Show($"Error: {errorCode}\nMessage: {errorMsg}", "Batch Close Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                // Display exception details
                MessageBox.Show("Error during batch close: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnDebugLocalDetailReportReq_Click(object sender, EventArgs e)
        {
            try
            {
                // Step 1: Set up communication settings
                var commSetting = new POSLink2.CommSetting.TcpSetting
                {
                    Ip = txtIpAddress.Text,
                    Port = 10009,
                    Timeout = 45000
                };

                // Step 2: Initialize POSLink and get terminal
                var poslink = POSLink2.POSLink2.GetPOSLink2();
                var terminal = poslink.GetTerminal(commSetting);

                if (terminal == null)
                {
                    MessageBox.Show("Failed to initialize terminal. Check IP settings.", "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Step 3: Create Local Detail Report Request
                var reportRequest = new POSLink2.Report.LocalDetailReportReq();

                // Step 4: Execute the request
                POSLink2.Report.LocalDetailReportRsp reportResponse;
                var ret = terminal.Report.LocalDetailReport(reportRequest, out reportResponse);

                // Step 5: Check if request was successful
                if (ret.GetErrorCode() == POSLink2.ExecutionResult.Code.Ok)
                {
                    StringBuilder reportDetails = new StringBuilder("Transaction Log:\n");

                    // Step 6: Extract Transaction Data
                    if (reportResponse.TraceInformation != null && reportResponse.AmountInformation != null)
                    {
                        reportDetails.AppendLine("--- Transactions ---");

                        // Extract transaction number (EcrRefNum) and approval code
                        string transactionNumber = reportResponse.TraceInformation?.EcrRefNum ?? "Unknown";
                        string approvalCode = reportResponse.TraceInformation?.AuthorizationResponse ?? "N/A";

                        // Extract the amount from AmountInformation
                        string transactionAmount = reportResponse.AmountInformation?.ApproveAmount ?? "0.00";

                        // Display transaction details
                        reportDetails.AppendLine($"Transaction ID: {transactionNumber}");
                        reportDetails.AppendLine($"Approval Code: {approvalCode}");
                        reportDetails.AppendLine($"Amount: ${transactionAmount}");
                        reportDetails.AppendLine("---------------------------------");
                    }
                    else
                    {
                        reportDetails.AppendLine("No transactions found.");
                    }

                    MessageBox.Show(reportDetails.ToString(), "Transaction Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Error retrieving transactions: {reportResponse.ResponseMessage ?? "Unknown error"}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving Transaction Log: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




    }


}
