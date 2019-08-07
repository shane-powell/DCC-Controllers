using System;
using System.Collections.Generic;
using System.Text;

namespace DCCMobileController.ViewModels
{
    using System.Collections.ObjectModel;

    using DccControllersLibNetStandard;

    using DCCMobileController.Bluetooth;

    using Xamarin.Forms;

    public class DccControllerBaseViewModel
    {
        private int sleepTime = 0;

        private bool isConnected = true;

        private Command connectCommand = null;

        private Command disconnectCommand = null;


        public DccControllerBaseViewModel()
        {
            MessagingCenter.Subscribe<App>(this, "Sleep", (obj) =>
                {
                    // When the app "sleep", I close the connection with bluetooth
                    if (this.isConnected)
                        DependencyService.Get<IBluetooth>().Cancel();

                });


            MessagingCenter.Subscribe<App>(this, "Resume", (obj) =>
                {

                    // When the app "resume" I try to restart the connection with bluetooth
                    if (this.isConnected)
                        DependencyService.Get<IBluetooth>().Start(SelectedBthDevice, this.sleepTime, true);

                });


            this.ConnectCommand = new Command(() => {

                    // Try to connect to a bth device
                    DependencyService.Get<IBluetooth>().Start(SelectedBthDevice, this.sleepTime, true);
                    this.isConnected = true;

                    // Receive data from bth device
                    MessagingCenter.Subscribe<App, string>(this, "Barcode", (sender, arg) => {

                            // Add the barcode to a list (first position)
                        });
                });

            this.DisconnectCommand = new Command(() => {

                    // Disconnect from bth device
                    DependencyService.Get<IBluetooth>().Cancel();
                    MessagingCenter.Unsubscribe<App, string>(this, "Barcode");
                    this.isConnected = false;
                });


            try
            {
                // At startup, I load all paired devices
                ListOfDevices = DependencyService.Get<IBluetooth>().PairedDevices();
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Attention", ex.Message, "Ok");
            }
        }

        public string SelectedBthDevice { get; set; } = "test";

        public ObservableCollection<string> ListOfDevices { get; set; }

        public Command ConnectCommand
        {
            get
            {
                return this.connectCommand;
            }
            set
            {
                this.connectCommand = value;
            }
        }

        public Command DisconnectCommand
        {
            get
            {
                return this.disconnectCommand;
            }
            set
            {
                this.disconnectCommand = value;
            }
        }
    }
}
