using System;
using System.IO.Ports;

namespace label2
{
    internal class ScaleReader
    {
        private SerialPort serialPort;

        public event Action<string> DataReceived;

        public ScaleReader(string portName = "COM1", int baudRate = 9600, int dataBits = 8, Parity parity = Parity.None, StopBits stopBits = StopBits.One)
        {
            serialPort = new SerialPort
            {
                PortName = portName,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = parity,
                StopBits = stopBits,
                ReadTimeout = 5000,
                WriteTimeout = 1000
            };

            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        public void Open()
        {
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
                Console.WriteLine("Serial port opened.");
            }
        }

        public void Close()
        {
            if (serialPort.IsOpen)
            {
                serialPort.Close();
                Console.WriteLine("Serial port closed.");
            }
        }

        public bool IsPortOpen()
        {
            return serialPort.IsOpen;
        }

        public void SendCommand(string command)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine(command); // Send command to the scale
                Console.WriteLine($"Command '{command}' sent.");
            }
            else
            {
                Console.WriteLine("Serial port is not open.");
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string response = serialPort.ReadExisting(); // Read any available response
                Console.WriteLine($"Received data: {response}");

                // Trigger the event with the response data if it's meaningful
                if (!string.IsNullOrWhiteSpace(response))
                {
                    DataReceived?.Invoke(response);
                }
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Read operation timed out.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data: {ex.Message}");
            }
        }

    }
}
