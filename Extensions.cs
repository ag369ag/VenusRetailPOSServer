using IniParser.Model;
using IniParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VenusRetailPOSServer
{
    public static class Extensions
    {
        
        //EXTENTION METHODS FOR TAX
        public static string GetTaxName(this List<Tax> tx, int id)
        {
            return (from tax in tx
                    where tax.id == id
                    select tax.name
                                 ).Single();


        }
        public static int GetTaxID(this List<Tax> tx, string name)
        {
            return (from tax in tx
                    where tax.name == name
                    select tax.id
                      ).Single();
        }
        public static decimal GetTaxRateByID(this List<Tax> tx, int id)
        {
            return (from tax in tx
                    where tax.id == id
                    select tax.rate
                       ).Single();
        }
        public static decimal GetTaxRateByName(this List<Tax> tx, string name)
        {
            return (from tax in tx
                    where tax.name == name
                    select tax.rate
                      ).Single();
        }
      
        public static string CustomShift(int shiftNum)
        {
            switch (shiftNum)
            {
                case 1:
                    return "GY";
                case 2:
                    return "AM";
                case 3:
                    return "PM";
                default:
                    return "US";
            }
        }
        public static bool IsSet(this object value)
        {
            return object.ReferenceEquals(value, null) ? false : true;
        }
        public static string IsNotSet(this String value)
        {
            return String.IsNullOrEmpty(value) ? null : value;
        }
        public static IniData ReadConfig(string path)
        {
            try
            {
                var parser = new FileIniDataParser();
                return parser.ReadFile(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
    }
}
