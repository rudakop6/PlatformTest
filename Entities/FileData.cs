using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Platform
{
    public class FileData
    {
        private int _numberStop = 0;
        public string DepartureTimes { get; set; }
        public string TicketPrices { get; set; }

        public List<string> Routes = new List<string>();

        public int NumberBus { get; set; }
        public int NumberStop
        {
            get { return _numberStop; }
            set
            {
                _numberStop = value;
                MainWindow.Instance.InitPointCB(value);
            }
        }
    }
}
