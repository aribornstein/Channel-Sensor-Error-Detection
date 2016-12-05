// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace EHDataSender
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.EventHubs;

    public class Program
    {
       private const string EhConnectionString = "{Event Hubs connection string}";
       private const string EhEntityPath = "{Stream Event Hub path/name}";

        public static void Main(string[] args)
        {
            SendMessagesToEventHub("dataset1.csv").Wait();
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
        }


        private static List<string> readCSV(string path)
        {
            var retVal = new List<string>();
            using (StreamReader reader = File.OpenText(path))
            {
                //pass 1st line header
                reader.ReadLine();

                //reading testdata and formating json to send
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var data = line.Split(new[] { ',' });
                    var message = $"{{time:'{data[0]}', velocity:{data[2]}, edited:{data[3]}, set:[{data[1]},{data[2]},{data[5]},{data[4]},{data[7]},{data[6]},{data[8]},{data[7]}], predicted:0 }}";
                    retVal.Add(message);
                }
            }
            return retVal;
        }

        // Creates EventHubs client and sends all the events from csv file
        private static async Task SendMessagesToEventHub(string filePath)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(EhConnectionString)
            {
                EntityPath = EhEntityPath
            };

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
         
            var table = readCSV(filePath);
            var msgCnt = 0;

            foreach (var message in table)
            {
                //var yval = tabley[numMessagesToSend];
                try
                {
                    //  var message = ($"{{x: {val}, y:{yval}}}").Replace(Environment.NewLine, string.Empty);
                    Console.WriteLine($"Sending message: {message}");
                    await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
                    msgCnt++;
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
                }

                await Task.Delay(100);

            }

            Console.WriteLine($"{msgCnt} messages sent.");
        }
    }
}
