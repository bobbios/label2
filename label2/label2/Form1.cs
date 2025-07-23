using System;
using System.Drawing.Text;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using POSLink2.CommSetting;
using POSLink2;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace label2
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            PopulateSalespersonDropdown();
            txtPrinter.Text = GetPrinterPathFromRegistry();
            textBox1.Text = "Pks Payments";
            textBox2.Text = "Store : ";
            textBox3.Text = "Mid : ";
            txtAmount.Text = ".05";
            txtInvoiceNum.Text = "35158415488";
            txtIpAddress.Text = "192.168.1.183";

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
            string[] salespersons = { "bob 215-868-2551", "Rayni 856-602-1491", "Keith Shin 267-407-7045" };

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
                Timeout = 10000
            };

            // Initialize POSLink
            var poslink = POSLink2.POSLink2.GetPOSLink2();
            var terminal = poslink.GetTerminal(commSetting);

            // Create the credit request
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

            // Handle the result
            switch (ret.GetErrorCode())
            {
                case POSLink2.ExecutionResult.Code.Ok:
                    string retMsg = $"Response Code: {doCreditRsp.ResponseCode}\nResponse Message: {doCreditRsp.ResponseMessage}";
                    MessageBox.Show(retMsg);
                    break;

                default:
                    MessageBox.Show("Error: " + ret.GetErrorCode().ToString());
                    break;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // Exits the entire application immediately
            Environment.Exit(0);
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
    }

}
