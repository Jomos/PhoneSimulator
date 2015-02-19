using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneSimulator.Models
{
    public class Passenger
    {
        public string Id { get; set; }
        public bool Sitting { get; set; }
        public double XPosition { get; set; }
        public double YPosition { get; set; }
    }
}
