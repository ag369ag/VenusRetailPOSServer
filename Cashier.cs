using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusRetailPOSServer
{
    public class Cashier
    {
        public int id { get; set; }
        public int number { get; set; }
        public string name { get; set; }
        public object password { get; set; }
        public int sellFuel { get; set; }
        public int sellDry { get; set; }
        public int openCDraw { get; set; }
        public int refunds { get; set; }
        public int delItem { get; set; }
        public int driveOff { get; set; }
        public int testDel { get; set; }
        public int clsShift { get; set; }
        public int clsDay { get; set; }
        public int clsMonth { get; set; }
        public int bof { get; set; }
        public int overRide { get; set; }
        public int gm { get; set; }
        public int printRep { get; set; }
        public int ej { get; set; }
        public int roleManager { get; set; }
        public int cumsale { get; set; }
        public int clsCDraw { get; set; }
        public int printShift { get; set; }
        public int printCDraw { get; set; }
        public int printMonth { get; set; }
        public int printDay { get; set; }
        public int voidItem { get; set; }
    }
}
