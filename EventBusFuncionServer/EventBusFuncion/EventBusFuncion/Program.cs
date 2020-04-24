using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace EventBusFuncion
{
    class Program
    {
        public static void Main()
        {
            Program main = new Program();
            var list = new List<SampleObject>();
            TcpListener server = null;
            try
            {
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                server = new TcpListener(localAddr, port);

                server.Start();

                Byte[] bytes = new Byte[16000000];
                List<SampleObject> data = null;

                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    NetworkStream stream = client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var mStream = new MemoryStream();
                        var binFormatter = new BinaryFormatter();

                        mStream.Write(bytes, 0, bytes.Length);
                        mStream.Position = 0;

                        data = binFormatter.Deserialize(mStream) as List<SampleObject>;
                        Console.WriteLine("Received: {0}", data);

                        break;
                    }
                    main.SaveData(data);
                    client.Close();

                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            server.Stop();


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
        public void SaveData(List<SampleObject> list)
        {
            string connectionString = "Server = N114\\SQLEXPRESS; Database = test; Trusted_Connection = True;";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    DataTable table = new DataTable();
                    table.Columns.Add("Id", typeof(Guid));
                    table.Columns.Add("IP", typeof(string));
                    table.Columns.Add("Date", typeof(DateTime));
                    table.Columns.Add("URL", typeof(string));

                    Stopwatch stopwatch = new Stopwatch();

                    stopwatch.Start();

                    foreach (SampleObject data in list)
                    {
                        DataRow row = table.NewRow();

                        row["Id"] = Guid.NewGuid();
                        row["IP"] = data.IP;
                        row["Date"] = data.Date;
                        row["URL"] = data.URL;

                        table.Rows.Add(row);
                    }

                    SqlBulkCopy bulkCopy = new SqlBulkCopy(connection);
                    bulkCopy.DestinationTableName = "Info";
                    bulkCopy.ColumnMappings.Add("IP", "IP");
                    bulkCopy.ColumnMappings.Add("Date", "Date");
                    bulkCopy.ColumnMappings.Add("URL", "URL");

                    connection.Open();
                    bulkCopy.WriteToServer(table);
                    stopwatch.Stop();

                    Console.WriteLine("Finish in {0}", stopwatch.ElapsedMilliseconds / 1000f, "seconds");

                    connection.Close();
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Error => {0}", exc);
                }
            }
        }

        //string commandText = "INSERT INTO Info(IP, Date, URL) VALUES (@IP, @Date, @URL)";

        //using (SqlConnection connection = new SqlConnection(connectionString))
        //{
        //    SqlCommand command = new SqlCommand(commandText, connection);
        //    command.Parameters.Add(new SqlParameter("@IP", SqlDbType.VarChar));
        //    command.Parameters.Add(new SqlParameter("@Date", SqlDbType.DateTime));
        //    command.Parameters.Add(new SqlParameter("@URL", SqlDbType.VarChar));

        //    Stopwatch stopwatch = new Stopwatch();

        //    stopwatch.Start();

        //    var i = 0;
        //    foreach (var item in data)
        //    {
        //        command.Parameters["@IP"].Value = item.IP;
        //        command.Parameters["@Date"].Value = item.Date;
        //        command.Parameters["@URL"].Value = item.URL;

        //        connection.Open();
        //        command.ExecuteNonQuery();
        //        Console.WriteLine("RowsAffected: {0}", i);
        //        connection.Close();
        //        i ++;
        //    }

        //    stopwatch.Stop();
        //    Console.WriteLine("Finish in {0}", stopwatch.ElapsedMilliseconds / 1000f, " seconds");
        //    Console.Read();
        //}
    }
}
