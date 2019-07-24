// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerialAdapter.cs" company="Shane Powell">
//   
// </copyright>
// <summary>
//   Basic serial adapter for sending and receiving message with the base station.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SerialCommsStandard
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Threading.Tasks;

    /// <summary>
    /// The serial adapter.
    /// </summary>
    public class SerialAdapter
    {
        /// <summary>
        /// List of common com ports to choose from
        /// </summary>
        public static readonly List<string> ComPortNames = new List<string>() { "COM1", "COM2", "COM3", "COM4", "COM5" };

        /// <summary>
        /// The serial port.
        /// </summary>
        private SerialPort serialPort;

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialAdapter"/> class.
        /// </summary>
        /// <param name="portName">
        /// The port name.
        /// </param>
        /// <param name="baudRate">
        /// The baud rate.
        /// </param>
        public SerialAdapter(string portName, int baudRate)
        {
            this.serialPort = new SerialPort(portName, baudRate, Parity.None, 8, StopBits.One);

            this.serialPort.Open();

            this.serialPort.DataReceived += this.SerialPort_DataReceived;
        }

        public void WriteString(string stringToWrite)
        {
            if (this.serialPort.IsOpen)
            {
                this.serialPort.WriteLine(stringToWrite);
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var stuff = this.serialPort.ReadExisting();

            Console.WriteLine(stuff);


        }



        /// <summary>
        /// Finalizes an instance of the <see cref="SerialAdapter"/> class. 
        /// </summary>
        ~SerialAdapter()
        {
            try
            {
                this.serialPort?.Dispose();
            }
            catch (Exception)
            {
                // ignored
            }
        }

    }
}
