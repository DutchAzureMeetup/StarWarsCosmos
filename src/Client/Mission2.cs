using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Azure.Documents.Client;

namespace Client
{
    public class Mission2
    {
        /// <summary>
        /// Registers change feed observer to update changes read on change feed to destination 
        /// collection. Deregisters change feed observer and closes process when enter key is pressed
        /// </summary>
        /// <returns>A Task to allow asynchronous execution</returns>
        public async Task RunChangeFeed()
        {
            string hostName = Guid.NewGuid().ToString();

            // monitored collection info 
            DocumentCollectionInfo starshipCollectionInfo = new DocumentCollectionInfo
            {
                Uri = new Uri(ConfigurationManager.AppSettings["SqlApi.EndpointUrl"]),
                MasterKey = ConfigurationManager.AppSettings["SqlApi.AuthorizationKey"],
                DatabaseName = ConfigurationManager.AppSettings["SqlApi.DatabaseName"],
                CollectionName = ConfigurationManager.AppSettings["SqlApi.StarshipCollectionName"]
            };

            // lease collection info 
            DocumentCollectionInfo leaseCollectionInfo = new DocumentCollectionInfo
            {
                Uri = new Uri(ConfigurationManager.AppSettings["SqlApi.EndpointUrl"]),
                MasterKey = ConfigurationManager.AppSettings["SqlApi.AuthorizationKey"],
                DatabaseName = ConfigurationManager.AppSettings["SqlApi.DatabaseName"],
                CollectionName = ConfigurationManager.AppSettings["SqlApi.LeaseCollectionName"]
            };

            await CreateCollectionIfNotExistsAsync(starshipCollectionInfo);
            await CreateCollectionIfNotExistsAsync(leaseCollectionInfo);

            // Customizable change feed option and host options 
            ChangeFeedOptions feedOptions = new ChangeFeedOptions();

            // ie customize StartFromBeginning so change feed reads from beginning
            // can customize MaxItemCount, PartitonKeyRangeId, RequestContinuation, SessionToken and StartFromBeginning
            feedOptions.StartFromBeginning = true;

            ChangeFeedHostOptions feedHostOptions = new ChangeFeedHostOptions();

            // ie. customizing lease renewal interval to 15 seconds
            // can customize LeaseRenewInterval, LeaseAcquireInterval, LeaseExpirationInterval, FeedPollDelay 
            feedHostOptions.LeaseRenewInterval = TimeSpan.FromSeconds(15);

            using (DocumentClient client = new DocumentClient(starshipCollectionInfo.Uri, starshipCollectionInfo.MasterKey))
            {
                DocumentFeedObserverFactory docObserverFactory = new DocumentFeedObserverFactory(client, starshipCollectionInfo);

                ChangeFeedEventHost host = new ChangeFeedEventHost(hostName, starshipCollectionInfo, leaseCollectionInfo, feedOptions, feedHostOptions);

                await host.RegisterObserverFactoryAsync(docObserverFactory).ConfigureAwait(false);

                Console.WriteLine("Running... Press enter to stop.");
                Console.ReadLine();

                await host.UnregisterObserversAsync();
            }
        }

        public async Task CreateCollectionIfNotExistsAsync(DocumentCollectionInfo collectionInfo)
        {
            // connecting client 
            using (DocumentClient client = new DocumentClient(collectionInfo.Uri, collectionInfo.MasterKey))
            {
                await client.CreateDatabaseIfNotExistsAsync(new Database { Id = collectionInfo.DatabaseName });

                // create collection if it does not exist 
                await client.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(collectionInfo.DatabaseName),
                    new DocumentCollection { Id = collectionInfo.CollectionName },
                    new RequestOptions { OfferThroughput = 1000 });
            }
        }
    }
}
