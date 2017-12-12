using ImportKeyValueDatabase;
using Microsoft.Azure;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.Azure.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var planetes = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText("planets.json"));

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("tableStorage"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table and delete it.
            CloudTable table = tableClient.GetTableReference("planets");
            table.DeleteIfExists();

            // Create the table if it doesn't exist.
            table.Create();

            foreach (var item in planetes)
            {
                table.Execute(TableOperation.Insert(new Planet(item.Key, item.Value)));
            }
        }
    }
}
