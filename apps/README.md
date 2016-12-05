# Pushing Anomoly Detection Model to Production
Based on samples from https://github.com/azure/azure-event-hubs-dotnet

## Main components 

**Sender** - Sends Data from Channel to Orignal EventHub (Simulated with CSV)

**Processor** - Pulls data from event hub uses the MLService to tag anomlies and pushes to back to Visulization Event Hub

**MLService** - Is a python REST service to tag anaomolies this can also be done using the [Azure ML Client API](https://github.com/Azure/Azure-MachineLearning-ClientLibrary-Python)


### To Use

1. Prerequisites

 - [Visual Studio 2015](http://www.visualstudio.com).
 - [.NET Core Visual Studio 2015 Tooling](http://www.microsoft.com/net/core).
 - An Azure subscription.
 - An Event Hubs namespace.

1. Create the following Azure Services in the Azure Portal
   - Sender Event Hub
   - Visualization Event Hub
   - Stream analytics 
   
2. Update the Following Keys and Endpoints in the following scripts
   - sender/program.cs
   
     ```cs
      private const string EhConnectionString = "{Event Hubs connection string}";
      private const string EhEntityPath = "{Stream Event Hub path/name}";
     ```
   - processor/program.cs
     ```cs
      private const string EhConnectionString = "{Event Hubs connection string}";
      private const string EhEntityPath = "{Stream Event Hub path/name}";
      private const string StorageContainerName = "{Storage account container name}";
      private const string StorageAccountName = "{Storage account name}";
      private const string StorageAccountKey = "{Storage account key}";

      private static readonly string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);
      ```   
   - processor/ehsender.cs
   
     ```cs
      private const string EhConnectionString = "{ Event Hubs connection string}";
      private const string EhEntityPath = "{Visulization Hub path/name}";
     ```

4. Modify sender/program.cs to process your Stream instead of the provided csv.

5. Update the classification script in ML_Service/protomodel to custom model 
 
6. Push sender, proccesor and ML service to azure as [webjobs](https://docs.microsoft.com/en-us/azure/app-service-web/web-sites-create-web-jobs)

7. Link [Stream Analytics to the visulization event hub and visualize with PowerBI](https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-power-bi-dashboard)

