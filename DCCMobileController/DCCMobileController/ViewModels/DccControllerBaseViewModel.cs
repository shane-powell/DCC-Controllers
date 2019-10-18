using System;
using System.Collections.Generic;
using System.Text;

namespace DCCMobileController.ViewModels
{
    using System.Collections.ObjectModel;

    using DccControllersLibNetStandard;

    using DCCMobileController.Bluetooth;
    using DCCMobileController.Views;

    using Xamarin.Forms;

    public class DccControllerBaseViewModel
    {
        private int sleepTime = 0;

        private bool isConnected = true;

        private Command connectCommand = null;

        private Command disconnectCommand = null;

        private string selectedDevice = string.Empty;

        /// <summary>
        /// The controller.
        /// </summary>
        private readonly DccController controller;

        private void OpenEditDecoderPage(DccDecoder dccDecoder)
        {
            // todo open edit page
        }

        public DccControllerBaseViewModel()
        {
            controller = new DccController(DependencyService.Get<IBluetooth>().Send);

            MessagingCenter.Subscribe<App>(this, "Sleep", (obj) =>
                {
                    // When the app "sleep", I close the connection with bluetooth
                    //if (this.isConnected)
                    //    DependencyService.Get<IBluetooth>().Cancel();

                });


            MessagingCenter.Subscribe<App>(this, "Resume", (obj) =>
                {

                    // When the app "resume" I try to restart the connection with bluetooth
                    //if (this.isConnected)
                    //    DependencyService.Get<IBluetooth>().Start(this.selectedDevice, this.sleepTime, true);

                });

            MessagingCenter.Subscribe<string>(this, "Bluetooth", (obj) =>
                {
                    Device.BeginInvokeOnMainThread(
                        () =>
                            {
                                Application.Current.MainPage.DisplayAlert("Attention", obj.ToString(), "Ok");
                            });
                });



            this.ConnectCommand = new Command(() =>
            {

                // Try to connect to a bth device
                if (DependencyService.Get<IBluetooth>().PairedDevices().Contains(this.selectedDevice))
                {
                    DependencyService.Get<IBluetooth>().Start(this.selectedDevice, this.sleepTime, true);
                    this.isConnected = true;
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Attention", "Bluetooth device is not paired", "Ok");
                }


            });

            this.DisconnectCommand = new Command(() =>
            {

                // Disconnect from bth device
                DependencyService.Get<IBluetooth>().Cancel();
                this.isConnected = false;
            });


            try
            {
                this.ListOfDevices = DependencyService.Get<IBluetooth>().PairedDevices();
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Attention", ex.Message, "Ok");
            }

            DependencyService.Get<IBluetooth>().SetIncomingMessageDelegate(this.controller.ProcessBaseStationReply);

            // DependencyService.Get<IBluetooth>().Start(this.selectedDevice, this.sleepTime, true);

        }


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

        public string SelectedDevice
        {
            get
            {
                return this.selectedDevice;
            }
            set
            {
                this.selectedDevice = value;
            }
        }

        public DccController Controller
        {
            get
            {
                return this.controller;
            }
        }
    }
}
