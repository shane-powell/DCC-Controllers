using System;
using System.Collections.Generic;
using System.Text;

namespace DCCMobileController.Bluetooth
{
    using System.Collections.ObjectModel;

    public interface IBluetooth
    {
        void Start(string name, int sleepTime, bool readAsCharArray);
        void Cancel();
        ObservableCollection<string> PairedDevices();
    }

}
