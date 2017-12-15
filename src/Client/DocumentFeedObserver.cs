using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CosmosDB.Table;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Storage;

namespace Client
{
    /// <summary>
    /// This class implements the IChangeFeedObserver interface and is used to observe 
    /// changes on change feed. ChangeFeedEventHost will create as many instances of 
    /// this class as needed. 
    /// </summary>
    public class DocumentFeedObserver : IChangeFeedObserver
    {
        private static int _totalDocs = 0;

        private readonly DocumentClient _client;
        private readonly Uri _destinationCollectionUri;
        private readonly CloudTable _planetsTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentFeedObserver" /> class.
        /// Saves input DocumentClient and DocumentCollectionInfo parameters to class fields
        /// </summary>
        /// <param name="client"> Client connected to destination collection </param>
        /// <param name="destinationCollection"> Destination collection information </param>
        public DocumentFeedObserver(DocumentClient client, DocumentCollectionInfo destinationCollection)
        {
            _client = client;
            _destinationCollectionUri = UriFactory.CreateDocumentCollectionUri(destinationCollection.DatabaseName, destinationCollection.CollectionName);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["TableApi.StorageConnectionString"]);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            _planetsTable = tableClient.GetTableReference("planets");
        }

        /// <summary>
        /// Called when change feed observer is opened; 
        /// this function prints out observer partition key id. 
        /// </summary>
        /// <param name="context">The context specifying partition for this observer, etc.</param>
        /// <returns>A Task to allow asynchronous execution</returns>
        public Task OpenAsync(ChangeFeedObserverContext context)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Observer opened for partition Key Range: {0}", context.PartitionKeyRangeId);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called when change feed observer is closed; 
        /// this function prints out observer partition key id and reason for shut down. 
        /// </summary>
        /// <param name="context">The context specifying partition for this observer, etc.</param>
        /// <param name="reason">Specifies the reason the observer is closed.</param>
        /// <returns>A Task to allow asynchronous execution</returns>
        public Task CloseAsync(ChangeFeedObserverContext context, ChangeFeedObserverCloseReason reason)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Observer closed, {0}", context.PartitionKeyRangeId);
            Console.WriteLine("Reason for shutdown, {0}", reason);
            return Task.CompletedTask;
        }

        /// <summary>
        /// When document changes are available on change feed, changes are copied to destination connection; 
        /// this function prints out the changed document ID. 
        /// </summary>
        /// <param name="context">The context specifying partition for this observer, etc.</param>
        /// <param name="docs">The documents changed.</param>
        /// <returns>A Task to allow asynchronous execution</returns>
        public async Task ProcessChangesAsync(ChangeFeedObserverContext context, IReadOnlyList<Document> docs)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Change feed: PartitionId {0} total {1} doc(s)", context.PartitionKeyRangeId, Interlocked.Add(ref _totalDocs, docs.Count));

            foreach (Document doc in docs)
            {
                FighterFlight flight = JsonConvert.DeserializeObject<FighterFlight>(doc.ToString());

                if (!string.IsNullOrWhiteSpace(flight.Base) && string.IsNullOrWhiteSpace(flight.PlanetName))
                {
                    string coordinates = flight.Base;

                    // Create a retrieve operation that takes a customer entity.
                    TableOperation retrieveOperation = TableOperation.Retrieve<Planet>("planet", coordinates);

                    // Execute the retrieve operation.
                    TableResult retrievedResult = _planetsTable.Execute(retrieveOperation);

                    if (retrievedResult.Result != null)
                    {
                        flight.PlanetName = ((Planet)retrievedResult.Result).PlanetName;
                    }

                    await _client.UpsertDocumentAsync(_destinationCollectionUri, flight);
                }
            }
        }
    }
}
