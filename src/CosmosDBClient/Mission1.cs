using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace CosmosDBClient
{
    public class Mission1
    {
        private readonly IConfigurationRoot _configuration;

        public Mission1(IConfigurationRoot configuration)
        {
            _configuration = configuration;
        }

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

        //static async Task QueryDocumentDb()
        //{
        //    using (DocumentClient documentClient = new DocumentClient(new Uri(EndpointUrl), AuthorizationKey))
        //    {
        //        // Create the database (if it doesn't exist yet).
        //        Database database = new Database { Id = DatabaseId };

        //        await documentClient.CreateDatabaseIfNotExistsAsync(database);

        //        var databaseUri = UriFactory.CreateDatabaseUri(DatabaseId);

        //        // Create a collection in the database (if it doesn't exist yet).
        //        DocumentCollection documentCollection = new DocumentCollection { Id = DocumentCollectionId };
        //        RequestOptions requestOptions = new RequestOptions { OfferThroughput = 1000 };

        //        await documentClient.CreateDocumentCollectionIfNotExistsAsync(databaseUri, documentCollection, requestOptions);

        //        var documentCollectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, DocumentCollectionId);




        //var cargoFlight = documentClient.CreateDocumentQuery<FighterFlight>(documentCollectionUri)
        //    .Where(x => x.Base != null && x.Diagnostics.HyperdriveRating == 3.0M)
        //    .AsEnumerable()
        //    .GroupBy(x => x.Base);

        //        var flights = documentClient.CreateDocumentQuery<CargoFlight>(documentCollectionUri)
        //            .Where(x => x.Cargo.Quantity <= 50)
        //            .
        //            .Average(x => x.Crew);
        //        //.AsEnumerable()
        //        //.Average(x => x.Crew);

        //        Console.WriteLine(flights);
        //        //                    .Take(5);

        //        //foreach (var flight in flights)
        //        //{
        //        //    Console.WriteLine($"{flight.Id}: {flight.Origin} => {flight.Destination}");
        //        //}
        //    }
        //}

        private DocumentClient CreateDocumentClient()
        {
            var endpointUrl = new Uri(_configuration["sqlApi:endpointUrl"]);
            var authorizationKey = _configuration["sqlApi:authorizationKey"];

            return new DocumentClient(endpointUrl, authorizationKey);
        }

        private async Task<Uri> Connect(DocumentClient client)
        {
            var databaseName = _configuration["sqlApi:databaseName"];
            var collectionName = _configuration["sqlApi:collectionName"];

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
