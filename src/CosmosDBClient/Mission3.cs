using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Graphs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

                Console.WriteLine("Importing cargo route data into database, this may take a minute...");

                //// Load the cargo routes data.
                //JObject cargoRoutes;
                //using (var reader = new JsonTextReader(File.OpenText("cargoroutes.json")))
                //{
                //    cargoRoutes = JObject.Load(reader);
                //}

                //GenerateThorilideRoute(cargoRoutes);
                //GenerateMiscRoutes(cargoRoutes);

                //var queries2 = MapToQueries(cargoRoutes);

                //File.WriteAllLines("queries.txt", queries2.ToArray());

                var queries = await File.ReadAllLinesAsync("cargoroutes.txt").ConfigureAwait(false);
                foreach (string query in queries)
                {
                    await ExecuteGremlinQueryAsync<dynamic>(client, collection, query).ConfigureAwait(false);
                }
            }
        }

        static async Task<(List<T> results, double costs)> ExecuteGremlinQueryAsync<T>(DocumentClient client, DocumentCollection collection, string queryText)
        {
            List<T> results = new List<T>();
            double costs = 0;

            var query = client.CreateGremlinQuery<T>(collection, queryText);
            while (query.HasMoreResults)
            {
                #region simple
                //foreach (T result in await query.ExecuteNextAsync<T>().ConfigureAwait(false))
                //{
                //    results.Add(result);
                //}
                #endregion

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

        #region Data Generation Methods (To be deleted)

        private static Random rand = new Random();

        static List<string> MapToQueries(JObject graph)
        {
            var queries = new List<string>();
            queries.Add("g.V().drop()");

            var verticeSets = graph.Children().Cast<JProperty>();
            var verticeTypes = verticeSets.Select(s => s.Name).ToList();

            var idMap = new Dictionary<string, int>();

            foreach (JProperty vertices in verticeSets)
            {
                var verticeType = vertices.Name;

                foreach (JObject vertice in ((JArray)vertices.Value))
                {
                    var verticeId = GetId(vertice["name"].Value<string>(), verticeType, idMap);

                    var queryBuilder = new StringBuilder($"g.addV('{verticeType}')");

                    queryBuilder.Append($".property('id', '{verticeId}')");

                    var properties = vertice.Children().Cast<JProperty>();

                    foreach (var property in properties.Where(p => !(p.Value is JArray)))
                    {
                        if (property.Value.ToString() != "unknown")
                        {
                            queryBuilder.Append($".property('{property.Name}', '{property.Value}')");
                        }
                    }

                    queries.Add(queryBuilder.ToString());

                    foreach (var property in properties.Where(p => p.Value is JArray))
                    {
                        foreach (JObject edge in property.Value)
                        {
                            queryBuilder = new StringBuilder($"g.V('{verticeId}').addE('{edge["edge"]}')");

                            foreach (JProperty edgeProperty in edge.Children())
                            {
                                if (edgeProperty.Name != "edge" && edgeProperty.Name != "name")
                                {
                                    queryBuilder.Append($".property('{edgeProperty.Name}', '{edgeProperty.Value}')");
                                }
                            }

                            queryBuilder.Append($".to(g.V('{GetId(edge["name"].Value<string>(), property.Name, idMap)}'))");

                            queries.Add(queryBuilder.ToString());
                        }
                    }
                }
            }
            return queries;
        }

        static string GetId(string key, string verticeType, IDictionary<string, int> map)
        {
            if (!map.ContainsKey(verticeType))
            {
                map.Add(verticeType, map.Count + 1);
            }

            return key.ToLowerInvariant().Replace(" ", "").Replace("-", "") + "." + map[verticeType];
        }

        static void GenerateMiscRoutes(JObject graph)
        {
            for (int i = 0; i < 50; i++)
            {
                GenerateMiscRoute(graph);
            }
        }

        static void GenerateMiscRoute(JObject graph)
        {
            var ports = graph["port"]
                .Children()
                .Cast<JObject>()
                .ToArray();

            JObject origin;
            JObject destination;
            JObject cargo;

            origin = ports[rand.Next(0, ports.Length)];

            do
            {
                destination = ports[rand.Next(0, ports.Length)];
            }
            while (destination == origin);

            var cargos = graph["cargo"]
                .Children()
                .Cast<JObject>()
                .ToArray();

            cargo = ports[rand.Next(0, cargos.Length)];

            GenerateCargoRoute(graph, origin["name"].ToString(), destination["name"].ToString(), cargo["name"].ToString(), rand.Next(1, 10));
        }

        static void GenerateThorilideRoute(JObject graph)
        {
            GenerateCargoRoute(graph, "Glogg Terminal", "Gorse Port", "Baradium Bisulfate", 4);
            GenerateCargoRoute(graph, "Gorse Port", "Maw Installation", "Thorilide", 7);
            GenerateCargoRoute(graph, "Hemera Station", "Maw Installation", "Iron", 3);
            GenerateCargoRoute(graph, "Nova Base", "Maw Installation", "Aluminum", 4);
        }

        static void GenerateCargoRoute(JObject graph, string origin, string destination, string cargo, int count)
        {
            var shipmodels = graph["shipmodel"]
                .Children()
                .Cast<JObject>()
                .ToArray();

            var ship = shipmodels[rand.Next(0, shipmodels.Length)];

            var tripLength = TimeSpan.FromMinutes(rand.Next(240, 960)).Ticks;
            var departure = new DateTime(1977, 5, 25).Ticks + TimeSpan.FromMinutes(rand.Next(10, 240)).Ticks;

            var intervalBetweenFlights = TimeSpan.FromDays(7).Ticks / count;

            for (var i = 0; i < count; i++)
            {
                GenerateFlight(graph, ship, cargo, origin, destination, departure, departure + tripLength);

                departure += intervalBetweenFlights;
            }
        }

        static void GenerateFlight(JObject graph, JObject ship, string cargo, string origin, string destination, long departure, long arrival)
        {
            var flightNr = GenerateFlightNr();
            var flight = new JObject();

            var cargoCapacity = ship["cargo_capacity"].Value<int>();
            var payloadAmount = rand.Next((int)(cargoCapacity * 0.6), cargoCapacity);

            flight.Add("name", flightNr);

            flight.Add("shipmodel", new JArray(
                JObject.FromObject(new
                {
                    edge = "ship",
                    name = ship["name"].Value<string>()
                })));

            flight.Add("cargo", new JArray(
                JObject.FromObject(new
                {
                    edge = "payload",
                    name = cargo,
                    quantity = payloadAmount
                })));

            flight.Add("port", new JArray(
                JObject.FromObject(new
                {
                    edge = "origin",
                    name = origin,
                    departure = new DateTime(departure)
                }),
                JObject.FromObject(new
                {
                    edge = "destination",
                    name = destination,
                    arrival = new DateTime(arrival)
                })));

            var flights = (JArray)graph["flight"];
            flights.Add(flight);
        }

        static string GenerateFlightNr()
        {
            return string.Format("ICF-{0:000000}", rand.Next(100, 100000));
        }

        #endregion
    }
}
