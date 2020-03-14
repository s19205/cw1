using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Cw2
{
    class Program
    {

        public static void ErrorLogging(Exception ex)
        {
            string logpath = @"Z:\4semestr\APBD\cw2\log.txt";
            if (!File.Exists(logpath))
            {
                File.Create(logpath).Dispose();
            }
            StreamWriter sw = File.AppendText(logpath);
            sw.WriteLine("logging errors");
            sw.WriteLine("Start " + DateTime.Now);
            sw.WriteLine("Error: " + ex.Message);
            sw.WriteLine("End " + DateTime.Now);
            sw.Close();
        }

        public static Dictionary<string, int> studies(string[] source)
        {
            var map = new Dictionary<string, int>();
            
            foreach (var str in source)
            {
                var fields = str.Split(',');

                string key = fields[2];
                if (!map.ContainsKey(key))
                {
                    map.Add(key, 1);
                }
                else
                {
                    map[key] = map[key] + 1;
                }

            }
            return map;
        }

        static void Main(string[] args)
        {
            try
            {
                string csvpath = Console.ReadLine();    //Z:\4semestr\APBD\cw2\dane.csv
                string xmlpath = Console.ReadLine();    //Z:\4semestr\APBD\cw2\
                string format = Console.ReadLine();     //xml

                var map = new Dictionary<string, int>();
                
                if (File.Exists(csvpath) && Directory.Exists(xmlpath))
                {
                    // Read into an array of strings.  
                    var dateTimeNow = DateTime.Now; 
                    var dateOnlyString = dateTimeNow.ToShortDateString();
                    string[] source = File.ReadAllLines(csvpath);

                    map = studies(source);

                    XElement xml = new XElement("uczelnia", 
                        new XAttribute("createdAt", DateTime.Now.ToString("dd.MM.yyyy")),
                        new XAttribute("autor","Polina Lozovska"),
                            new XElement("studenci",
                            from str in source
                            let fields = str.Split(',')
                                select new XElement("student",
                                    new XAttribute("indexNumber", "s" + fields[4]),
                                    new XElement("fname", fields[0]),
                                    new XElement("lname", fields[1]),
                                    new XElement("birthday", fields[5]),
                                    new XElement("email", fields[6]),
                                    new XElement("mothersname", fields[7]),
                                    new XElement("fathersname", fields[8]),
                                    new XElement("studies",
                                        new XElement("name", fields[2]),
                                        new XElement("mode", fields[3])
                                    )
                                )
                            ),
                            new XElement("activeStudies",
                                from item in map
                                select new XElement("studies", 
                                    new XAttribute("name", item.Key),
                                    new XAttribute("numberOfStudents", item.Value)
                                )
                            )
                    );
                    xml.Save(String.Concat(xmlpath + "result.xml"));
                }
                else
                {
                    if (!File.Exists(csvpath))
                    {
                        throw new Exception("File does not exist");
                    }
                    if (!Directory.Exists(xmlpath))
                    {
                        throw new Exception("Directory does not exist");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogging(ex);
            }







        }
    }
}
