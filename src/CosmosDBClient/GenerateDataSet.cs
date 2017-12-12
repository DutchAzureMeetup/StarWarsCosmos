using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CosmosDBClient
{
    public static class GenerateDataSet
    {
        public static void Run()
        {
            var planets = CreatePlanetCoordinates();
            var rand = new Random();

            var planetJson = JsonConvert.SerializeObject(planets, Formatting.Indented);
            File.WriteAllText("planets.json", planetJson);

            var docs = new List<JObject>();
            docs.AddRange(CreateTieDocuments(planets, rand, rand.Next(40, 60)));
            docs.AddRange(CreateTieHDocuments(planets, rand, 50));
            docs.AddRange(CreateFreighterDocuments(planets, rand, 100));

            var docStrings = docs
                .Select(d => JsonConvert.SerializeObject(d))
                .OrderBy(_ => rand.Next())
                .ToList();

            File.WriteAllLines("output.json", docStrings);

            Console.WriteLine(docStrings.Count);
        }

        private static Dictionary<string, string> CreatePlanetCoordinates()
        {
            var planets = new string[]
            {
                "Alderaan",
                "Yavin IV",
                "Hoth",
                "Dagobah",
                "Bespin",
                "Endor",
                "Naboo",
                "Coruscant",
                "Kamino",
                "Geonosis",
                "Utapau",
                "Mustafar",
                "Kashyyyk",
                "Polis Massa",
                "Mygeeto",
                "Felucia",
                "Cato Neimoidia",
                "Saleucami",
                "Stewjon",
                "Eriadu",
                "Corellia",
                "Rodia",
                "Nal Hutta",
                "Dantooine",
                "Bestine IV",
                "Ord Mantell",
                "Trandosha",
                "Socorro",
                "Mon Cala",
                "Chandrila",
                "Sullust",
                "Toydaria",
                "Malastare",
                "Dathomir",
                "Ryloth",
                "Aleen Minor",
                "Vulpter",
                "Troiken",
                "Tund",
                "Haruun Kal",
                "Cerea",
                "Glee Anselm",
                "Iridonia",
                "Tholoth",
                "Iktotch",
                "Quermia",
                "Dorin",
                "Champala",
                "Mirial",
                "Serenno",
                "Concord Dawn",
                "Ojom",
                "Skako",
                "Muunilinst",
                "Shili",
                "Kalee",
                "Umbara",
                "Tatooine",
                "Jakku",
                "Kessel",
                "Gorse",
                "Cynda"
            };

            var results = new Dictionary<string, string>();
            var rand = new Random();

            foreach (var planet in planets)
            {
                results[planet] = string.Format("{0:000000.00};{1:000000.00};{2:000000.00}",
                    rand.Next(10000000, 99999999) / 100D,
                    rand.Next(10000000, 99999999) / 100D,
                    rand.Next(10000000, 99999999) / 100D);
            }

            return results;
        }

        private static IEnumerable<JObject> CreateTieHDocuments(Dictionary<string, string> planets, Random rand, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return CreateTieDocument(planets["Kessel"], 2.0M, 3.0M, rand);
            }
        }

        private static JObject CreateTieHDocument(Dictionary<string, string> planets, Random rand)
        {
            return CreateTieDocument(planets["Kessel"], 2.0M, 3.0M, rand);
        }

        private static IEnumerable<JObject> CreateTieDocuments(Dictionary<string, string> planets, Random rand, int count)
        {
            foreach (var planet in planets.OrderBy(_ => rand.Next()).Take(10))
            {
                if (planet.Key != "Kessel")
                {
                    for (var i = 0; i < count; i++)
                    {
                        yield return CreateTieDocument(planet.Value, 1.0M, 0M, rand);
                    }
                }
            }
        }

        private static JObject CreateTieDocument(string origin, decimal shieldRating, decimal hyperdriveRating, Random rand)
        {
            return JObject.FromObject(new
            {
                id = Guid.NewGuid(),
                base_id = origin,
                flight_timestamp = rand.Next(100000, 999999),
                diagnostics = new
                {
                    shield_rating = shieldRating,
                    hyperdrive_rating = hyperdriveRating
                }
            });
        }

        private static IEnumerable<JObject> CreateFreighterDocuments(Dictionary<string, string> planets, Random rand, int count)
        {
            foreach (var origin in planets.OrderBy(_ => rand.Next()).Take(10))
            {
                if (origin.Key != "Kessel")
                {
                    KeyValuePair<string, string> dest;
                    do
                    {
                        dest = planets.ElementAt(rand.Next(0, planets.Count));
                    }
                    while (dest.Key == origin.Key);

                    for (var i = 0; i < count; i++)
                    {
                        yield return CreateFreighterDocument(origin.Value, dest.Value, rand.Next(1, 4), rand.Next(50, 150), rand);
                    }
                }
            }
        }

        private static JObject CreateFreighterDocument(string origin, string destination, int crew, int cargo, Random rand)
        {
            return JObject.FromObject(new
            {
                id = Guid.NewGuid(),
                origin_id = origin,
                destination_id = destination,
                crew = crew,
                flight_timestamp = rand.Next(100000, 999999),
                cargo = new
                {
                    quantity = cargo
                }
            });
        }
    }
}
