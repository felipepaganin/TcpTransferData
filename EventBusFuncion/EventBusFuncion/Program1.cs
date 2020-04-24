using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EventBusFuncion
{
    public class Program1
    {
        static void Main(string[] args)
        {
            string fileName = @"C:\Users\felipe.silveira\Desktop\EventBusFuncion\EventBusFuncion\access.log";

            var result = TextFileToSampleObject(fileName);
            Connect("127.0.0.1", result);
        }

        static List<SampleObject> TextFileToSampleObject(string fileName)
        {
            var allLines = new List<string>();
            var result = new List<SampleObject>();

            try
            {
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string log;
                    while ((log = sr.ReadLine()) != null)
                    {
                        allLines.Add(sr.ReadLine());

                    }
                }

                Parallel.For(0, allLines.Count, x =>
                {
                    result.Add(ToSampleObject(allLines[x]));
                });
            }
            finally
            {
                if (allLines != null)
                {
                    allLines = null;
                }
            }
            GC.Collect();
            return result;
        }

        static SampleObject ToSampleObject(string row)
        {
            var result = new SampleObject();
            result.URL = GetUrlFromString(row);
            result.Date = GetDateFromString(row);
            result.IP = GetIPFromString(row);

            return result;
        }
        static string GetUrlFromString(string subjectString)
        {
            var regexObj = new Regex(@"http://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?");
            Match matchResults = regexObj.Match(subjectString);
            return matchResults.Value;
        }

        static DateTime GetDateFromString(string subjectString)
        {
            var regexObj = new Regex(@"^.{0,14}");
            Match matchResults = regexObj.Match(subjectString);
            return UnixTimeStampToDateTime(matchResults.Value);
        }

        private static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds((long)Convert.ToDouble(unixTimeStamp));
            return dtDateTime;
        }

        static string GetIPFromString(string subjectString)
        {
            var regexObj = new Regex(@"((?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?\.){3}(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)))");
            Match matchResults = regexObj.Match(subjectString);
            return matchResults.Value;

        }

        static void Connect(String server, List<SampleObject> result)
        {
            try
            {
                Int32 port = 13000;
                TcpClient client = new TcpClient(server, port);

                BinaryFormatter binFormatter = new BinaryFormatter();
                var mStream = new MemoryStream();
                binFormatter.Serialize(mStream, result);


                Byte[] data = mStream.ToArray();
                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", result);

                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
                Console.Read();

                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
        }
    }
}

