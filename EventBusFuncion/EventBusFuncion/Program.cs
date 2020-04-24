//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Text.RegularExpressions;

//namespace EventBusFuncion
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            string[] dic_lines = File.ReadAllLines("path_to_dic_file.dic");
//            List<string> l_group1 = new List<string>();
//            List<string> l_group2 = new List<string>();

//            foreach (l_group1 in dic_lines)
//            {
//                Regex regexObj = new Regex(@"(\(.*?\))\s*(.*)\s*");
//                Match match = regexObj.Match(subjectString);
//                if (matchResults.Success)
//                {
//                    l_group1.Add(match.Groups[1].Value);
//                    l_group2.Add(match.Groups[2].Value);
//                }
//            }

//            File.WriteAllLines("outputfile.txt", l_group1);
//            File.AppendAllLines("outputfile.txt", l_group2);

//            UdpClient client = new UdpClient();

//            client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080));

//            Console.WriteLine(">");
//            string inputs = Console.ReadLine();

//            if (inputs != null)
//            {
//                byte[] bytesent = Encoding.ASCII.GetBytes(inputs);
//                client.Send(bytesent, bytesent.Length);
//                Console.WriteLine("deu certo carai");

//                client.Close();
//                Console.ReadLine();
//            }
//        }

//    }
//}
