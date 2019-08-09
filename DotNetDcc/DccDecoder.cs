using System;

namespace DccControllersLibNetStandard
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class DccDecoder : INotifyPropertyChanged
    {
        private RelayCommand toggleLightsCommand;

        private Action<string> sendCommandDelegate = null;

        private int address = 0;

        private string name = "New Loco";

        /// <summary>
        /// The last speed value given send to the decoder
        /// 0 - 126 (-1 for emergency stop)
        /// </summary>
        private int speed = 0;

        /// <summary>
        /// 1 = Forward, 0 = Reverse
        /// </summary>
        private int direction = 1;

        private bool lights = false;

        public int Address
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = value;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// The last speed value given send to the decoder
        /// 0 - 126 (-1 for emergency stop)
        /// </summary>
        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                this.UpdateLocomotion();
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// 1 = Forward, 0 = Reverse
        /// </summary>
        public int Direction
        {
            get { return direction; }
            set
            {
                direction = value;
                this.UpdateLocomotion();

            }
        }

        private void UpdateLocomotion()
        {
            this.sendCommandDelegate?.Invoke($"<t 1 {this.address} {this.speed} {this.direction}>");
        }

        public bool Lights
        {
            get { return lights; }
            set { lights = value; }
        }

        public RelayCommand ToggleLightsCommand
        {
            get { return toggleLightsCommand; }
            set { toggleLightsCommand = value; }
        }

        public DccDecoder(Action<string> sendCommandDelegate)
        {
            this.sendCommandDelegate = sendCommandDelegate;
            this.ToggleLightsCommand = new RelayCommand(this.ToggleLights);
        }

        private void ToggleLights()
        {
            lights = !lights;

            if (lights)
            {
                this.sendCommandDelegate?.Invoke($"<f {this.address} 144>");
            }
            else
            {
                this.sendCommandDelegate?.Invoke($"<f {this.address} 122>");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
