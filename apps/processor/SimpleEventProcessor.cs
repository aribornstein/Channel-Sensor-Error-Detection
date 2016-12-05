// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net.Http;
using System.Text;

namespace ehprocessor
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.EventHubs.Processor;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;


    public class SimpleEventProcessor : IEventProcessor
    {
        private static HttpClient client = new HttpClient();

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.FromResult<object>(null);
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            client.BaseAddress = new Uri("http://localhost:8080/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Console.WriteLine($"Http client initialized '{client.BaseAddress}'");
            return Task.FromResult<object>(null);
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.FromResult<object>(null);
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                //after recieving data we post it async to get scoring and write the results to another ehprocessor
                Console.WriteLine("test");
                try
                {
                    var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                    Console.WriteLine($"false json: {data}");
                    JObject datao = (JObject)JsonConvert.DeserializeObject(data);

                    //  Console.WriteLine(dataobj.GetType());
                    var dataset = datao.GetValue("set");
                    Console.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{dataset}'");
                    var ret = GetScores(dataset.ToString());
                    Console.WriteLine($"Scored data: {ret.Result}");
                    var time = datao["time"];
                    var vel = datao["velocity"];
                    var edit = datao["edited"];
                    var messjson = $"{{time:'{time}',velocity:{vel},edited:{edit},predicted:{ret.Result} }}";
                    ehsender.SendMessagesToEventHub(messjson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            return context.CheckpointAsync();
        }
        public async Task<string> GetScores(string values)
        {
            HttpResponseMessage response = await client.PostAsync("predict", new JsonContent(values));
            response.EnsureSuccessStatusCode();
           
            // we push answer to eventhub with correct rowId and predicted value + predicted probability
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

    }

    public class JsonContent : StringContent
    {
        public JsonContent(string content) :
          this(content, Encoding.UTF8){}

        private JsonContent(string content, Encoding encoding, string mediaType = "application/json") :
            base(content, encoding, mediaType){}
    }

}