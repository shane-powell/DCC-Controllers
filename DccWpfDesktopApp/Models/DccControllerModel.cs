using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DccWpfDesktopApp.Models
{
    using DccControllersLibNetStandard;

    internal class DccControllerModel
    {

        private readonly DccController controller = new DccController();

        public DccController Controller
        {
            get
            {
                return this.controller;
            }
        }

        public List<string> ComPortsList
        {
            get
            {
                return SerialCommsStandard.SerialAdapter.ComPortNames;
            }
        }
    }
}
