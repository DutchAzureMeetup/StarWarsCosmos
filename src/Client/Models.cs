using System.Collections.Generic;
using Microsoft.Azure.CosmosDB.Table;
using Newtonsoft.Json;

namespace Client
{
    public class Flight
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("flight_timestamp")]
        public string FlightTimestamp { get; set; }
    }

    public class CargoFlight : Flight
    {
        [JsonProperty("origin_id")]
        public string Origin { get; set; }

        [JsonProperty("destination_id")]
        public string Destination { get; set; }

        [JsonProperty("crew")]
        public int Crew { get; set; }

        [JsonProperty("cargo")]
        public FlightCargo Cargo { get; set; }
    }

    public class FighterFlight : Flight
    {
        [JsonProperty("base_id")]
        public string Base { get; set; }

        [JsonProperty("diagnostics")]
        public FlightDiagnostics Diagnostics { get; set; }
    }

    public class FlightDiagnostics
    {
        [JsonProperty("shield_rating")]
        public decimal ShieldRating { get; set; }

        [JsonProperty("hyperdrive_rating")]
        public decimal HyperdriveRating { get; set; }
    }

    public class FlightCargo
    {
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }

    public class Planet : TableEntity
    {
        public Planet()
        {

        }
        public Planet(string planetName, string coordinate)
        {
            this.PartitionKey = "planet";
            this.RowKey = coordinate;
            this.PlanetName = planetName;
        }

        public string PlanetName { get; set; }
    }

    public class QuizDto
    {
        public string Id { get; set; }

        public List<QuestionDto> Questions { get; set; }
    }

    public class QuestionDto
    {
        public string Id { get; set; }

        public string Text { get; set; }
    }

    public class AnswerDto
    {
        public string TeamName { get; set; }

        public string QuestionId { get; set; }

        public string Text { get; set; }
    }

    public class AnswerFeedbackDto
    {
        public bool IsCorrect { get; set; }

        public string Hint { get; set; }
    }
}
