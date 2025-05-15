using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusRetailPOSServer
{
    public class Response
    {
        public int statusCode { get; set; }
        public string statusDescription { get; set; }
        public string data { get; set; }
    }

    public class PeriodData
    {
        public int Period_ID { get; set; }
        public int Shift_number { get; set; }
    }
}
