using System;
using System.Drawing.Printing;
using System.Windows.Forms;
using Microsoft.Win32;

namespace label2
{
    public class LabelPrinter
    {
        public void PrintLabel(string printerPath, string line1, string line2, string line3, string line4, int copies)
        {
            try
            {
                string escPosCommands =
                    (char)27 + "A" + (char)49 + (char)49 + line1 + "\r" +
                    (char)27 + "A" + (char)49 + (char)50 + line2 + "\r" +
                    (char)27 + "A" + (char)49 + (char)50 + line3 + "\r" +
                    (char)27 + "A" + (char)49 + (char)50 + line4 + "\r" +
                    (char)12; // Form feed (new page)

                for (int i = 0; i < copies; i++)
                {
                    RawPrinterHelper.SendStringToPrinter(printerPath, escPosCommands);
                }

                MessageBox.Show("Print job sent successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to send print job: " + ex.Message);
            }
        }

        public string GetPrinterPathFromRegistry()
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

        public void SavePrinterPathToRegistry(string printerPath)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\MyAppName");
            key.SetValue("PrinterPath", printerPath);
            key.Close();
        }
    }
}
