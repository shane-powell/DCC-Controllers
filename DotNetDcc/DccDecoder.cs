using System;

namespace DccControllersLibNetStandard
{
    public class DccDecoder
    {
        private int address = 0;

        private string name = string.Empty;

        /// <summary>
        /// The last speed value given send to the decoder
        /// 0 - 126 (-1 for emergency stop)
        /// </summary>
        private int speed = 0;

        /// <summary>
        /// 1 = Forward, 0 = Reverse
        /// </summary>
        private int direction = 1;

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
    }
}
