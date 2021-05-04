using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;


namespace ConsoleApplication4
{
    class Portlar
    {
        public string Port { get; set; }
        public string ForeignAdress { get; set; }
        
    }
    public class StreamClass
    {
        public bool success { get; set; }
        public string lowest_price { get; set; }
        public string volume { get; set; }
        public string median_price { get; set; }
    }
    class Program
    {

        public static List<Port> GetNetStatPorts()
        {

            var Ports = new List<Port>();
            List<string> isimler = new List<string>();

            isimler.Clear();
            try
            {
                using (Process p = new Process())
                {

                    ProcessStartInfo ps = new ProcessStartInfo();
                    ps.Arguments = "-a -n -o";
                    ps.FileName = "netstat.exe";
                    ps.UseShellExecute = false;
                    ps.WindowStyle = ProcessWindowStyle.Hidden;
                    ps.RedirectStandardInput = true;
                    ps.RedirectStandardOutput = true;
                    ps.RedirectStandardError = true;

                    p.StartInfo = ps;
                    p.Start();

                    StreamReader stdOutput = p.StandardOutput;
                    StreamReader stdError = p.StandardError;

                    string content = stdOutput.ReadToEnd() + stdError.ReadToEnd();
                    string exitStatus = p.ExitCode.ToString();

                    if (exitStatus != "0")
                    {
                        
                    }
                    string[] rows = Regex.Split(content, "\r\n");
                    foreach (string row in rows)
                    {
                        string[] tokens = Regex.Split(row, "\\s+");

                        if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                        {
                            string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                            isimler.Add(tokens[2].ToString());

                            string local = tokens[3].ToString();
                            string porttt = local.Split(':')[1];
                            if (tokens[3] != "0.0.0.0:0")
                            {
                                string foreign = tokens[3].ToString();
                                string portttt = foreign.Split(':')[1];
                                Portlar port1 = new Portlar()
                                {
                                    Port= portttt,
                                    ForeignAdress = tokens[3].ToString()
                                    
                                };
                                string stringJSON = JsonConvert.SerializeObject(port1);
                                Console.WriteLine(stringJSON);
                                string paths = @"C:\Users\nurca_7ebxvks\Desktop\program\program\bin\debug\json.txt";
                                using (var tw = new StreamWriter(paths, true))
                                {
                                    tw.WriteLine(stringJSON.ToString());
                                    tw.Close();
                                }
                                string pathss = @"C:\Users\nurca_7ebxvks\Desktop\program\program\bin\debug\json.json";
                                using (var tw = new StreamWriter(pathss, true))
                                {
                                    tw.WriteLine(stringJSON.ToString());
                                    tw.Close();
                                }
                            }
                            else
                            {
                                continue;
                            }
                            Ports.Add(new Port
                            {
                                protocol = localAddress.Contains("1.1.1.1") ? String.Format("{0}v6", tokens[1]) : String.Format("{0}v4", tokens[1]),
                                port_number = localAddress.Split(':')[1],
                                process_name = tokens[1] == "UDP" ? LookupProcess(Convert.ToInt16(tokens[4])) : LookupProcess(Convert.ToInt16(tokens[5]))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Ports;
            
        }

        public static string LookupProcess(int pid)
        {
            string procName;
            try { procName = Process.GetProcessById(pid).ProcessName; }
            catch (Exception) { procName = "-"; }
            return procName;
        }
        
        public class Port
        {
            public string name
            {
                get
                {
                    return string.Format("{0} ({1} port {2})", this.process_name, this.protocol, this.port_number);
                }
                set { }
            }
            public string port_number { get; set; }
            public string process_name { get; set; }
            public string protocol { get; set; }

        }
        static void Main(string[] args)
        {
            Console.WriteLine("[");
            GetNetStatPorts();
            Console.WriteLine("]");
            Console.ReadLine();
        }


    }
}
