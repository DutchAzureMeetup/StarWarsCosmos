using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Client
{
    public class Mission1
    {
        public async Task Seed()
        {
            using (DocumentClient client = CreateDocumentClient())
            {
                var collectionUri = await Connect(client);

                Console.WriteLine("Importing flight data into database, this may take a minute...");

                // Load the flight data, each line is a separate JSON document which we'll store
                // in a JObject.
                var documents = File.ReadAllLines("flightdata.json")
                    .Select(line => JObject.Parse(line));

                // Add each document to the database. Usually, to do bulk insert of documents it is recommended to
                // use a Stored Procedure and pass a batch of documents to the Stored Prcoedure. This will cut down
                // on the number of roundtrips required. 
                foreach (var document in documents)
                {
                    await client.CreateDocumentAsync(collectionUri, document);
                }
            }
        }

        public async Task Query()
        {
            using (DocumentClient client = CreateDocumentClient())
            {
                var collectionUri = await Connect(client);

                var flights = client.CreateDocumentQuery<CargoFlight>(collectionUri)
                    .Where(x => x.Crew == 1)
                    .OrderBy(x => x.FlightTimestamp)
                    .Take(5)
                    .AsEnumerable();

                foreach (var flight in flights)
                {
                    Console.WriteLine($"Id={flight.Id}, Crew={flight.Crew}, Origin={flight.Origin}, Dest={flight.Destination}");
                }
            }
        }

        private DocumentClient CreateDocumentClient()
        {
            var endpointUrl = new Uri(ConfigurationManager.AppSettings["SqlApi.EndpointUrl"]);
            var authorizationKey = ConfigurationManager.AppSettings["SqlApi.AuthorizationKey"];

            return new DocumentClient(endpointUrl, authorizationKey);
        }

        private async Task<Uri> Connect(DocumentClient client)
        {
            var databaseName = ConfigurationManager.AppSettings["SqlApi.DatabaseName"];
            var collectionName = ConfigurationManager.AppSettings["SqlApi.StarshipCollectionName"];

            // Create the database (if it doesn't exist yet).
            Database database = new Database { Id = databaseName };

            await client.CreateDatabaseIfNotExistsAsync(database);

            var databaseUri = UriFactory.CreateDatabaseUri(databaseName);

            // Create a collection in the database (if it doesn't exist yet).
            DocumentCollection collection = new DocumentCollection { Id = collectionName };
            RequestOptions requestOptions = new RequestOptions { OfferThroughput = 1000 };

            await client.CreateDocumentCollectionIfNotExistsAsync(databaseUri, collection, requestOptions);

            return UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);
        }
    }
}
