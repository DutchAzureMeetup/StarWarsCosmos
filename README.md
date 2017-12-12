# Cosmos DB Lab: TIE Fighter Troubles

## Prerequisites
- In this lab you’ll work with both the SQL (DocumentDB) and Graph (Gremlin) API’s, therefore you’ll need two Cosmos DB databases. You can try Azure Cosmos DB for free without an Azure subscription (free or charge and commitments) at https://azure.microsoft.com/en-us/try/cosmosdb/.
However, this is limited to a single API per Microsoft account at a time; so use multiple Microsoft accounts or join up with other rebel forces to have access to multiple free databases.
Alternatively, you can create Cosmos DB databases in your own Azure subscription if you have one, or create a trial Azure subscription.

- Clone or download the Dutch Azure Meetup Star Wars repository at https://github.com/DutchAzureMeetup/StarWarsCosmos.
This repository contains a sample client application that you will use to seed and query your databases.

## Background
Grand Admiral Redriss Rustariss is secretly developing a TIE/H Hunter starfighter for the Empire. The Tie Hunters are armed with concussion rockets and twin blaster cannons, including a hyperdrive, and cloaking device. 

By hacking Empire systems, the RESISTANCE has retrieved a data dump of Empire starship movements. This data must be analyzed to LOCATE the hidden manufacturing facility and DISRUPT mass production of the new starfighter...

## Mission 1: SQL API 
**Objective:**
In this mission you’ll import the Imperial starship data set into Cosmos DB using the SQL API. By querying the imported data, you’ll discover the location of the hidden facility where the new TIE Fighter prototypes are being assembled.

1. Create a Cosmos DB database with a SQL API and data model. Navigate to the **Keys** blade in the newly created database resource and note the `URI` and `PRIMARY KEY` fields. You will need these values later on.

2. Open the **CosmosDBClient** solution and edit the **appsettings.json** class. There are some settings defined here that you’ll need to change to be able to connect to your database. Use the values of the `URI` and `PRIMARY KEY` fields you’ve retrieved earlier to set the `endpointUrl` and `authorizationKey` fields of the `sqlApi` subsection.

3. Open the **Mission1** class and examine the **Seed** method. This method first creates the database and collection if they doesn’t exist yet (see the **Connect** method). Then it will import the flight data into the collection.

4. Examine the **flightdata.json** file. This file contains all the data to import. Each line is a separate JSON document. Note that there are two different types of JSON documents in the data set. And even though the two documents are from different schemas we're going to create them inside the same collection and work with them seamlessly together.
Cosmos DB requires an "id" for all documents. You can either supply your own unique value for id, or let Cosmos DB provide it for you. In the flight data we are supplying our own id so Cosmos DB will only ensure uniqueness of our values.

5. Run the **Seed** method from the command line using the following syntax (make sure to run the command from the *bin/Debug/netcoreapp2.0/* folder):

```
dotnet client.dll mission1 seed 
```

6. You can view the added documents in the portal if you navigate to the **Data Explorer** blade of the database.

[TODO MAYBE TALK ABOUT THE DOC PROPERTIES ADDED BY COSMOS DB]

7. You can also use the .NET SDK to query Cosmos DB. Start the client app using the **query** command:

```
dotnet client.dll mission1 query
```

This will output the first 5 cargo flights with a crew of one. Examine the **Query** method in the **Mission1** class to see how the data is retrieved. Note that we are using classes from *Models.cs* to execute strongly-typed queries.

8. Answer the following questions by querying the imported data set:

What’s the average size of the crew on cargo flights carrying a payload of less than or equal to 50 metric tons?

How many non-cargo / fighter flights are there in the collection?

What are the coordinates of the base where the new TIE Fighter prototypes are being tested? (Note that the new prototypes are equipped with an hyperdrive).