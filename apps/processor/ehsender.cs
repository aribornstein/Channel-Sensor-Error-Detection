using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

public static class ehsender
{
    // public static async Task<Uri> 
    public static async Task SendMessagesToEventHub(string message)
    {

        // Creates an EventHubsConnectionStringBuilder object from a the connection string, and sets the EntityPath.
        // Typically the connection string should have the Entity Path in it, but for the sake of this simple scenario
        // we are using the connection string from the namespace.
        private const string EhConnectionString = "{ Event Hubs connection string}";
        private const string EhEntityPath = "{Visulization Hub path/name}";
 
        var cstrbuilder = new EventHubsConnectionStringBuilder(EhConnectionString)
        {
            EntityPath = EhEntityPath
        };

        var eventHubClient = EventHubClient.CreateFromConnectionString(cstrbuilder.ToString());

        // for (var i = 0; i < numMessagesToSend; i++)
        {
            try
            {
                await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} > Exception: {exception.Message}");
            }
        }
    }

}