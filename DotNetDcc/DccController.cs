// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DccController.cs" company="Shane Powell">
//   
// </copyright>
// <summary>
//   Defines the DccController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DccControllersLibNetStandard
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using SerialCommsStandard;

    public class DccController : INotifyPropertyChanged
    {
        private ObservableCollection<DccDecoder> decoders = new ObservableCollection<DccDecoder>();

        private SerialAdapter serialAdapter;

        private string selectedComPort = string.Empty;

        private const int BaudRate = 115200;

        public ObservableCollection<DccDecoder> Decoders
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

        public DccController()
        {
            for (int i = 0; i < 6; i++)
            {
                DccDecoder decoder = new DccDecoder(this.SendCommand) {Address = i, Name = $"Loco {i}"};
                this.decoders.Add(decoder);
            }
        }

        private void SendCommand(string command)
        {
            this?.serialAdapter.WriteString(command);
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
