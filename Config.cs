using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusRetailPOSServer
{
    public class CONFIG
    {
        public int POS_ID { get; set; }
        public int POS_Number { get; set; }
        public int POS_Master { get; set; }
        public int POS_Trans_Number { get; set; }
        public int POS_Training_Mode { get; set; }
        public int Receipt_ID { get; set; }
        public int Display_ID { get; set; }
        public int BIR_Fuel_Receipt_ID { get; set; }
        public int BIR_Non_Fuel_Receipt_ID { get; set; }
        public int BIR_Fuel_Trans_Number { get; set; }
        public int BIR_Non_Fuel_Trans_Number { get; set; }
        public DateTime gt_open_date { get; set; }
    }
}
