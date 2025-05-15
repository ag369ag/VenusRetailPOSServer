using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using System.Xml;

namespace VenusRetailPOSServer
{
    public class FileIO
    {

        /*
         * Generic method for reading text file.
         */
        public string[] ReadText(string path)
        {
            try
            {
                string[] lines = File.ReadAllLines(path);

                foreach (string xx in lines)
                {
                    Console.WriteLine(xx);
                }

                return lines;
            }
            catch (Exception)
            {
                //MessageBox.Show("File not Found!", "Error");
                return null;
            }

        }

        
        public void CreateTextFile(string path, List<string> lines)
        {
            if (!File.Exists(path))
            {

                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    foreach (string x in lines)
                    {
                        sw.WriteLine(x);
                    }
                }
            }
            else
            {
                //MessageBox.Show("File already exists", "File not Created", MessageBoxButtons.OK);
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(new string('=', 42));
                    foreach (string x in lines)
                    {
                        sw.WriteLine(x);
                    }
                }
            }
            Console.WriteLine("Text file created");
        }

        public static void CreateDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine("Directory exists");
                    return;
                }
                DirectoryInfo di = Directory.CreateDirectory(path);
            }
            catch { }

        }

        public static void RenameDirectory(string olddir, string newdir)
        {

            DirectoryInfo source = new DirectoryInfo(olddir);
            DirectoryInfo target = new DirectoryInfo(newdir);

            try
            {
                //Create archive directory
                if (!Directory.Exists("oldrct"))
                {
                    Directory.CreateDirectory("oldrct");
                }
                if (!Directory.Exists("oldreports"))
                {
                    Directory.CreateDirectory("oldreports");
                }

                //Check if source directory exists
                if (Directory.Exists(olddir))
                {
                    //check if new directory exists
                    if (Directory.Exists(newdir))
                    {
                        // Copy each file into it's new directory.
                        foreach (FileInfo fi in source.GetFiles())
                        {
                            Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                            fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                        }
                        Directory.Delete(olddir, true);
                    }
                    else
                    {
                        Directory.Move(olddir, newdir);
                    }

                }
            }
            catch { }
        }
        public static bool WriteToFile(List<string> str)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Exports";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = null;

            filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Exports\\AccumulatedSalesReport - " + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".csv";

            try
            {
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    foreach (string x in str)
                        sw.WriteLine(x);

                }

                MessageBox.Show("CSV export done!" + Environment.NewLine + "Check Exports folder for the csv file.", "Task successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

        }
        public static bool SaveSaleWindow(int LineNumber, int ItemID, int WindowNumber, decimal qty, bool NewFile)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Temp";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = null;

            filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\SW" + WindowNumber.ToString() + ".tmp";

            try
            {
                if (NewFile && File.Exists(filepath))
                {
                    File.Delete(filepath);
                }

                if (!File.Exists(filepath))
                {

                    // Create a file to write to.   
                    using (StreamWriter sw = File.CreateText(filepath))
                    {
                        sw.WriteLine(LineNumber + "," + ItemID + "," + qty);
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        sw.WriteLine(LineNumber + "," + ItemID + "," + qty);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Saving temp file");
                Console.WriteLine(e);
                return false;
            }
        }
        public static bool SaveSaleWindow(List<string> lines, int WindowNumber)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Temp";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = null;

            filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\SW" + WindowNumber.ToString() + ".tmp";

            try
            {
                if (!File.Exists(filepath))
                {
                    // Create a file to write to.   
                    using (StreamWriter sw = File.CreateText(filepath))
                    {
                        foreach (string x in lines)
                            sw.WriteLine(x);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Saving temp file");
                Console.WriteLine(e);
                //MessageBox.Show(e.Message);
                return false;
            }
        }
        public static bool WriteToFile(List<string> str, string filename)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Exports";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = null;

            filepath = AppDomain.CurrentDomain.BaseDirectory + $"\\Exports\\{filename}";

            try
            {
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                }
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    foreach (string x in str)
                        sw.WriteLine(x);

                }

                MessageBox.Show("CSV export done!" + Environment.NewLine + "Check Exports folder for the csv file.", "Task successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

        }
        public static List<string> ReadTemp(int WindowNumber)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\SW" + WindowNumber.ToString() + ".tmp";
            return File.ReadAllLines(path, Encoding.Default).ToList();
        }
        public static void DeleteTempFile(int WindowNumber)
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Temp\\SW" + WindowNumber.ToString() + ".tmp";
            try
            {
                File.Delete(filepath);
            }
            catch (Exception)
            {
                Console.WriteLine("File does not exists.");
            }
        }
    }
}
