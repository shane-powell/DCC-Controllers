// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DccController.cs" company="Shane Powell">
//   
// </copyright>
// <summary>
//   Defines the DccController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace DccControllersLibNetStandard
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using SerialCommsStandard;

    public class DccController : INotifyPropertyChanged
    {
        private List<DccDecoder> decoders = new List<DccDecoder>();

        private SerialAdapter serialAdapter;

        private string selectedComPort = string.Empty;

        private const int BaudRate = 115200;

        public List<DccDecoder> Decoders
        {
            get
            {
                return this.decoders;
            }
            set
            {
                this.decoders = value;
            }
        }

        public string SelectedComPort
        {
            get
            {
                return this.selectedComPort;
            }
            set
            {
                this.selectedComPort = value;
                this.OnPropertyChanged();
                this.ConnectToPort();
            }
        }

        private void ConnectToPort()
        {
            try
            {
                this.serialAdapter = new SerialAdapter(this.selectedComPort, BaudRate);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
