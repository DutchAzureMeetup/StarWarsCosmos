using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Microsoft.Extensions.Configuration;

namespace CosmosDBClient
{
    public class Mission3
    {
        private readonly IConfigurationRoot _configuration;

        public Mission3(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

        public async Task Seed()
        {
            using (DocumentClient client = CreateDocumentClient())
            {
                var collection = await Connect(client);

                Console.WriteLine("Importing cargo route data into database, this may take a few minutes...");

                var queries = await File.ReadAllLinesAsync("cargoroutes.txt").ConfigureAwait(false);
                foreach (string query in queries)
                {
                    await ExecuteGremlinQueryAsync<dynamic>(client, collection, query).ConfigureAwait(false);
                }
            }
        }

        public async Task Query()
        {
            using (DocumentClient client = CreateDocumentClient())
            {
                var collection = await Connect(client);

                // The following query starts at the vertice representing the Serenity space port.
                // For this port, we find all cargo flights that arrived there and navigate to the cargo (using the payload edge).
                // Then we trace back from the cargo to find all other space ports that received the same type of cargo.
                string query = "g.V('moseisley.3').in('destination').out('payload').in('payload').out('destination').dedup()";

                (List<dynamic> results, double costs) = await ExecuteGremlinQueryAsync<dynamic>(client, collection, query)
                    .ConfigureAwait(false);

                foreach (dynamic result in results)
                {
                    Console.WriteLine($"{result}");
                }
            }
        }

        public async Task Interactive()
        {
            using (DocumentClient client = CreateDocumentClient())
            {
                var collection = await Connect(client);

                ConsoleColor defaultColor = Console.ForegroundColor;
                while (true)
                {
                    try
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("graph> ");

                        string query = Console.ReadLine();
                        if (query.ToLowerInvariant() == "quit")
                        {
                            break;
                        }

                        // execute
                        Console.ForegroundColor = defaultColor;
                        (List<dynamic> results, double costs) = await ExecuteGremlinQueryAsync<dynamic>(client, collection, query)
                            .ConfigureAwait(false);

                        foreach (dynamic result in results)
                        {
                            Console.WriteLine($"{result}");
                        }

                        Console.WriteLine($"\nQuery costs: {costs} RUs.\n");
                    }
                    catch
                    {
                        Console.WriteLine("Enable to execute query. Check the syntax.\n");
                    }
                }
            }
        }

        private async Task<(List<T> results, double costs)> ExecuteGremlinQueryAsync<T>(
            DocumentClient client, DocumentCollection collection, string queryText)
        {
            List<T> results = new List<T>();
            double costs = 0;

            IDocumentQuery<T> query = client.CreateGremlinQuery<T>(collection, queryText);
            while (query.HasMoreResults)
            {
                FeedResponse<T> response = await query.ExecuteNextAsync<T>().ConfigureAwait(false);
                costs = response.RequestCharge;

                foreach (T result in response)
                {
                    results.Add(result);
                }
            }

            return (results, costs);
        }

        private DocumentClient CreateDocumentClient()
        {
            var endpointUrl = new Uri(_configuration["graphApi:endpointUrl"]);
            var authorizationKey = _configuration["graphApi:authorizationKey"];

            return new DocumentClient(endpointUrl, authorizationKey);
        }

        private async Task<DocumentCollection> Connect(DocumentClient client)
        {
            var databaseName = _configuration["graphApi:databaseName"];
            var collectionName = _configuration["graphApi:collectionName"];

            // Create the database (if it doesn't exist yet).
            Database database = new Database { Id = databaseName };

            await client.CreateDatabaseIfNotExistsAsync(database);

            var databaseUri = UriFactory.CreateDatabaseUri(databaseName);

            // Create a collection in the database (if it doesn't exist yet).
            DocumentCollection collection = new DocumentCollection { Id = collectionName };
            RequestOptions requestOptions = new RequestOptions { OfferThroughput = 1000 };

            return await client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, collection, requestOptions);
        }
    }
}
