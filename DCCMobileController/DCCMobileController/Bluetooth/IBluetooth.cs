// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBluetooth.cs" company="Shane Powell">
//   Copyright (c) Shane Powell. All rights reserved.
// </copyright>
// <summary>
//   Defines the IBluetooth type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DCCMobileController.Bluetooth
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The Bluetooth interface.
    /// </summary>
    public interface IBluetooth
    {
        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sleepTime">
        /// The sleep time.
        /// </param>
        /// <param name="readAsCharArray">
        /// The read as char array.
        /// </param>
        void Start(string name, int sleepTime, bool readAsCharArray);

        /// <summary>
        /// Cancels interaction with the Bluetooth device.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Sends data to the Bluetooth device.
        /// </summary>
        /// <param name="messageToSend">
        /// The message to send.
        /// </param>
        void Send(string messageToSend);

        /// <summary>
        /// Holds the collection of paired devices.
        /// </summary>
        /// <returns>
        /// A collection of paired devices.
        /// </returns>
        ObservableCollection<string> PairedDevices();

        void SetIncomingMessageDelegate(Action<string> incomingMessageDelegate);
    }

}
