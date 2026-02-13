using System;
using System.Runtime.InteropServices;

public static class SnbcRawPrinterHelper
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private class DOCINFOA
    {
        [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
        [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
        [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
    }

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern bool OpenPrinter(string szPrinter, out IntPtr hPrinter, IntPtr pd);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true, CharSet = CharSet.Ansi)]
    private static extern bool StartDocPrinter(IntPtr hPrinter, int level, [In] DOCINFOA pDocInfo);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    public static bool SendRawString(string printerName, string data, out string error)
    {
        error = "";
        IntPtr hPrinter = IntPtr.Zero;
        IntPtr pUnmanaged = IntPtr.Zero;

        try
        {
            // IMPORTANT: bytes, not string length
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(data);

            if (!OpenPrinter(printerName, out hPrinter, IntPtr.Zero) || hPrinter == IntPtr.Zero)
            {
                error = "OpenPrinter failed: " + Marshal.GetLastWin32Error();
                return false;
            }

            var di = new DOCINFOA { pDocName = "SNBC RAW", pDataType = "RAW" };

            if (!StartDocPrinter(hPrinter, 1, di))
            {
                error = "StartDocPrinter failed: " + Marshal.GetLastWin32Error();
                return false;
            }

            if (!StartPagePrinter(hPrinter))
            {
                error = "StartPagePrinter failed: " + Marshal.GetLastWin32Error();
                EndDocPrinter(hPrinter);
                return false;
            }

            pUnmanaged = Marshal.AllocCoTaskMem(bytes.Length);
            Marshal.Copy(bytes, 0, pUnmanaged, bytes.Length);

            int written;
            bool ok = WritePrinter(hPrinter, pUnmanaged, bytes.Length, out written);

            EndPagePrinter(hPrinter);
            EndDocPrinter(hPrinter);

            if (!ok)
            {
                error = "WritePrinter failed: " + Marshal.GetLastWin32Error();
                return false;
            }

            if (written != bytes.Length)
            {
                error = $"WritePrinter partial write: wrote {written} of {bytes.Length}";
                return false;
            }

            return true;
        }
        finally
        {
            if (pUnmanaged != IntPtr.Zero) Marshal.FreeCoTaskMem(pUnmanaged);
            if (hPrinter != IntPtr.Zero) ClosePrinter(hPrinter);
        }
    }
}
