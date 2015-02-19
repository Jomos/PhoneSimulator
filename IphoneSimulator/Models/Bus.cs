using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneSimulator.Models
{
    public class Bus
    {
        public int BusNumber { get; set; }
        public string BusLine { get; set; }
        public List<Passenger> Passengers { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public DateTime Time { get; set; }

    }
}
