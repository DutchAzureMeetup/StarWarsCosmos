//using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.CosmosDB.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImportKeyValueDatabase
{
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
}
