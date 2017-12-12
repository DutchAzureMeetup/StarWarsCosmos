﻿using Newtonsoft.Json;

namespace CosmosDBClient
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
}