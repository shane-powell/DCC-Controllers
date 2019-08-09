// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ABluetooth.cs" company="Shane Powell">
//   Copyright (c) Shane Powell. All rights reserved.
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

    using Xamarin.Forms;

    /// <summary>
    /// An Android implementation of the IBluetooth interface.
    /// </summary>
    public class ABluetooth : IBluetooth
    {
        /// <summary>
        /// The messages to send.
        /// </summary>
        private readonly Queue<string> messagesToSend = new Queue<string>();

        /// <summary>
        /// The message lock object.
        /// </summary>
        private readonly object messageLockObject = new object();

        /// <summary>
        /// A token to cancel the Bluetooth interaction task.
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ABluetooth"/> class.
        /// </summary>
        public ABluetooth()
        {
        }


        /// <summary>
        /// Start the "reading" DeviceConnectionLoop 
        /// </summary>
        /// <param name="name">Name of the paired bluetooth device (also a part of the name)</param>
        public void Start(string name, int sleepTime = 200, bool readAsCharArray = false)
        {
            this.cancellationTokenSource?.Cancel();
            Thread.Sleep(sleepTime);
            Task.Run(async () => this.DeviceConnectionLoop(name, sleepTime, readAsCharArray));
        }

        /// <summary>
        /// broadcasts the bluetooth status.
        /// </summary>
        /// <param name="statusMessage">
        /// The status message.
        /// </param>
        private void BroadcastBluetoothStatus(string statusMessage)
        {
            MessagingCenter.Send<App, string>((App)Application.Current, "BluetoothStatus", statusMessage);
        }

        private async Task DeviceConnectionLoop(string name, int sleepTime, bool readAsCharArray)
        {
            BluetoothDevice device = null;

            this.cancellationTokenSource = new CancellationTokenSource();
            while (this.cancellationTokenSource.IsCancellationRequested == false)
            {
                BluetoothAdapter adapter = null;
                try
                {
                    Thread.Sleep(sleepTime);

                    adapter = BluetoothAdapter.DefaultAdapter;

                    if (adapter == null)
                    {
                        this.BroadcastBluetoothStatus("No Bluetooth adapter found.");
                        return;
                    }

                    if (!adapter.IsEnabled)
                    {
                        this.BroadcastBluetoothStatus("Bluetooth is disabled.");
                        return;
                    }

                    // Try to find device with supplied name
                    foreach (var bd in adapter.BondedDevices)
                    {
                        System.Diagnostics.Debug.WriteLine("Paired devices found: " + bd.Name.ToUpper());
                        if (bd.Name.ToUpper().IndexOf(name.ToUpper(), StringComparison.Ordinal) >= 0)
                        {

                            System.Diagnostics.Debug.WriteLine("Found " + bd.Name + ". Try to connect with it!");
                            device = bd;
                            break;
                        }
                    }

                    if (device == null)
                    {
                        this.BroadcastBluetoothStatus($"Could not find device named {name}.");
                        return;
                    }
                    else
                    {
                        UUID uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
                        using (var socket = (int)Android.OS.Build.VERSION.SdkInt >= 10
                                                ? device.CreateInsecureRfcommSocketToServiceRecord(uuid)
                                                : device.CreateRfcommSocketToServiceRecord(uuid))
                        {
                            if (socket != null)
                            {
                                await socket.ConnectAsync();
                                using (var writer = new OutputStreamWriter(socket.OutputStream))
                                {
                                    //writer.Write("hello");
                                    //writer.Flush();

                                    if (socket.IsConnected)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Connected!");
                                        using (var mReader = new InputStreamReader(socket.InputStream))
                                        {
                                            using (var buffer = new BufferedReader(mReader))
                                            {
                                                while (this.cancellationTokenSource.IsCancellationRequested == false)
                                                {
                                                    lock (this.messageLockObject)
                                                    {
                                                        while (this.messagesToSend.Count > 0)
                                                        {
                                                            var message = this.messagesToSend.Dequeue();
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
                                                            System.Diagnostics.Debug.WriteLine(
                                                                "New Message: " + incomingData);
                                                            Xamarin.Forms.MessagingCenter.Send<App, string>(
                                                                (App)Xamarin.Forms.Application.Current,
                                                                "NewMessage",
                                                                incomingData);
                                                        }
                                                    }

                                                    System.Threading.Thread.Sleep(sleepTime);
                                                    if (!socket.IsConnected)
                                                    {
                                                        System.Diagnostics.Debug.WriteLine(
                                                            "BthSocket.IsConnected = false, Throw exception");
                                                        throw new Exception();
                                                    }
                                                }

                                                System.Diagnostics.Debug.WriteLine("Exit the inner DeviceConnectionLoop");
                                            }
                                        }
                                    }
                                }
                            }
                            else
                                System.Diagnostics.Debug.WriteLine("BthSocket = null");
                        }
                    }


                }
                catch (Exception ex)
                {
                    this.BroadcastBluetoothStatus("Connection Failed");
                    // ignored
                }
                finally
                {
                    //device = null;
                    //adapter = null;
                }
            }

            System.Diagnostics.Debug.WriteLine("Exit the external DeviceConnectionLoop");
        }

        /// <summary>
        /// Cancel the Reading DeviceConnectionLoop
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
                this.messagesToSend.Enqueue(messageToSend);
            }
        }

        public ObservableCollection<string> PairedDevices()
        {
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            ObservableCollection<string> devices = new ObservableCollection<string>();

            if (adapter?.BondedDevices != null)
            {
                foreach (var bd in adapter.BondedDevices)
                {
                    devices.Add(bd.Name);
                }
            }

            return devices;
        }

    }
}
