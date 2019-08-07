// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ABluetooth.cs" company="Shane Powell">
//   
// </copyright>
// <summary>
//   Defines the ABluetooth type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using DCCMobileController.Droid.Bluetooth;

[assembly: Xamarin.Forms.Dependency(typeof(ABluetooth))]

namespace DCCMobileController.Droid.Bluetooth
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;

    using Android.Bluetooth;

    using DCCMobileController;
    using DCCMobileController.Bluetooth;

    using Java.IO;
    using Java.Util;

    /// <summary>
    /// An Android implementation of the IBluetooth interface.
    /// </summary>
    public class ABluetooth : IBluetooth
    {

        private CancellationTokenSource cancellationTokenSource { get; set; }

        const int RequestResolveError = 1000;

        /// <summary>
        /// The messages to send.
        /// </summary>
        private readonly List<string> messagesToSend = new List<string>();

        /// <summary>
        /// The message lock object.
        /// </summary>
        private readonly object messageLockObject = new object();

        public ABluetooth()
        {
        }


        /// <summary>
        /// Start the "reading" loop 
        /// </summary>
        /// <param name="name">Name of the paired bluetooth device (also a part of the name)</param>
        public void Start(string name, int sleepTime = 200, bool readAsCharArray = false)
        {

            Task.Run(async () => this.loop(name, sleepTime, readAsCharArray));
        }

        private async Task loop(string name, int sleepTime, bool readAsCharArray)
        {
            BluetoothDevice device = null;
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            BluetoothSocket BthSocket = null;

            this.cancellationTokenSource = new CancellationTokenSource();
            while (this.cancellationTokenSource.IsCancellationRequested == false)
            {

                try
                {
                    Thread.Sleep(sleepTime);

                    adapter = BluetoothAdapter.DefaultAdapter;

                    if (adapter == null)
                        System.Diagnostics.Debug.WriteLine("No Bluetooth adapter found.");
                    else
                        System.Diagnostics.Debug.WriteLine("Adapter found!!");

                    if (!adapter.IsEnabled)
                        System.Diagnostics.Debug.WriteLine("Bluetooth adapter is not enabled.");
                    else
                        System.Diagnostics.Debug.WriteLine("Adapter enabled!");

                    System.Diagnostics.Debug.WriteLine("Try to connect to " + name);

                    foreach (var bd in adapter.BondedDevices)
                    {
                        System.Diagnostics.Debug.WriteLine("Paired devices found: " + bd.Name.ToUpper());
                        if (bd.Name.ToUpper().IndexOf(name.ToUpper()) >= 0)
                        {

                            System.Diagnostics.Debug.WriteLine("Found " + bd.Name + ". Try to connect with it!");
                            device = bd;
                            break;
                        }
                    }

                    if (device == null)
                        System.Diagnostics.Debug.WriteLine("Named device not found.");
                    else
                    {
                        UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
                        BthSocket = (int)Android.OS.Build.VERSION.SdkInt >= 10 ? device.CreateInsecureRfcommSocketToServiceRecord(uuid) : device.CreateRfcommSocketToServiceRecord(uuid);

                        if (BthSocket != null)
                        {

                            await BthSocket.ConnectAsync();
                            var writer = new OutputStreamWriter(BthSocket.OutputStream);
                            if (BthSocket.IsConnected)
                            {
                                System.Diagnostics.Debug.WriteLine("Connected!");
                                var mReader = new InputStreamReader(BthSocket.InputStream);
                                var buffer = new BufferedReader(mReader);
                                while (this.cancellationTokenSource.IsCancellationRequested == false)
                                {
                                    lock (this.messageLockObject)
                                    {
                                        foreach (var message in this.messagesToSend)
                                        {
                                            writer.Write(message);
                                            writer.Flush();
                                        }
                                    }

                                    if (buffer.Ready())
                                    {

                                        char[] chr = new char[100];
                                        string incomingData = string.Empty;
                                        if (readAsCharArray)
                                        {

                                            await buffer.ReadAsync(chr);
                                            foreach (char c in chr)
                                            {
                                                if (c == '\0')
                                                {
                                                    break;
                                                }

                                                incomingData += c;
                                            }

                                        }
                                        else
                                            incomingData = await buffer.ReadLineAsync();

                                        if (incomingData.Length > 0)
                                        {
                                            System.Diagnostics.Debug.WriteLine("New Message: " + incomingData);
                                            Xamarin.Forms.MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "Barcode", incomingData);
                                        }
                                        else
                                            System.Diagnostics.Debug.WriteLine("No data");

                                    }
                                    else
                                        System.Diagnostics.Debug.WriteLine("No data to read");

                                    // A little stop to the uneverending thread...
                                    System.Threading.Thread.Sleep(sleepTime);
                                    if (!BthSocket.IsConnected)
                                    {
                                        System.Diagnostics.Debug.WriteLine("BthSocket.IsConnected = false, Throw exception");
                                        throw new Exception();
                                    }
                                }

                                System.Diagnostics.Debug.WriteLine("Exit the inner loop");

                            }
                        }
                        else
                            System.Diagnostics.Debug.WriteLine("BthSocket = null");

                    }


                }
                catch
                {
                }

                finally
                {
                    if (BthSocket != null)
                        BthSocket.Close();
                    device = null;
                    adapter = null;
                }
            }

            System.Diagnostics.Debug.WriteLine("Exit the external loop");
        }

        /// <summary>
        /// Cancel the Reading loop
        /// </summary>
        /// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
        public void Cancel()
        {
            if (this.cancellationTokenSource != null)
            {
                System.Diagnostics.Debug.WriteLine("Send a cancel to task!");
                this.cancellationTokenSource.Cancel();
            }
        }

        public void Send(string messageToSend)
        {
            lock (this.messageLockObject)
            {
                this.messagesToSend.Add(messageToSend);
            }
        }

        public ObservableCollection<string> PairedDevices()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            ObservableCollection<string> devices = new ObservableCollection<string>();

            foreach (var bd in adapter.BondedDevices)
                devices.Add(bd.Name);

            return devices;
        }

    }
}
