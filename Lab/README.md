# Cosmos DB Lab: TIE Fighter Troubles

## Prerequisites
- In this lab you’ll work with the SQL (DocumentDB), Table and Graph (Gremlin) API’s. You’ll need two Cosmos DB databases for the SQL and Graph API missions. You can try Azure Cosmos DB for free without an Azure subscription (free or charge and commitments) at https://azure.microsoft.com/en-us/try/cosmosdb/.
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

2. Open the **CosmosDBClient** solution and edit the **App.config** class.
First of all, choose an epic team name and enter it in the TeamName settings.
There are also some settings defined here that you’ll need to change to be able to connect to your database. Use the values of the `URI` and `PRIMARY KEY` fields you’ve retrieved earlier to set the `SqlApi.EndpointUrl` and `SqlApi.AuthorizationKey` settings.

3. Open the **Mission1** class and examine the **Seed** method. This method first creates the database and collection if they don’t exist yet (see the **Connect** method). Then it should import the flight data into the collection.
Complete the **Seed** method by adding the appropriate API calls.

4. Examine the **flightdata.json** file. This file contains all the data to import. Each line is a separate JSON document. Note that there are two different types of JSON documents in the data set. And even though the two documents are from different schemas we're going to create them inside the same collection and work with them seamlessly together.
Cosmos DB requires an "id" for all documents. You can either supply your own unique value for id, or let Cosmos DB provide it for you. In the flight data we are supplying our own id so Cosmos DB will only ensure uniqueness of our values.

5. Run the **Seed** method from the command line using the following syntax (make sure to run the command from the *bin\Debug* folder):

```
client.exe mission1 seed 
```

6. You can view the added documents in the portal if you navigate to the **Data Explorer** blade of the database.

7. You can also use the .NET SDK to query Cosmos DB. Start the client app using the **query** command:

```
client.exe mission1 query
```

This will output the first 5 cargo flights with a crew of one. Examine the **Query** method in the **Mission1** class to see how the data is retrieved. Note that we are using classes from *Models.cs* to execute strongly-typed queries.

8. Answer the following questions by querying the imported data set:

*What’s the average size of the crew on cargo flights carrying a payload of less than or equal to 50 metric tons (round to nearest integer)?*

*How many non-cargo / fighter flights are there in the collection?*

*What are the coordinates of the planet where the new TIE Fighter prototypes are being tested? (Note that the new prototypes are equipped with an hyperdrive).*

Validate your answers using the following command:

```
client.exe mission1 quiz
```

## Mission 2: Change Feed
**Objective:**
In this mission you'll use the change feed feature to listen to database changes and enrich the inserted data.

1. Examine the **RunChangeFeed** method in the **Mission2** class. This method starts a change feed observer to listen to database changes.
Next, look at the **DocumentFeedObserver** class. This class contains an exercise placeholder where you must write code to listen to changed FighterFlight documents.
These documents contain a **base_id** property containing the coordinates for the planet of origin.
You must lookup the planet name using the Table API.

The partition key of the table is `planet`.
The row key of the table is the planet coordinates.
The planet name is stored in a `planet` column.
(There's also a useful **Planet** class in **Models.cs**)

Notice that you'll need to update the model classes to store the planet name, but there's no database schema to update.
Also beware of creating endless loops with the change feed (remember that every change is published on the feed).

You can start the change feed using the following command:

```
client.exe mission2 runchangefeed
```

The change feed keeps a checkpoint in the leases collection in your (DocumentDB) database. If you want to rerun the change feed from the beginning, remove the two documents from this collection first.

2. Answer the following question:

*What is the name of the planet where the new TIE Fighter prototypes are being tested?*

Validate your answers using the following command:

```
client.exe mission2 quiz
```

## Mission 3: Graph API 
**Objective:**
In this mission you’ll use the intelligence gathered so far to disrupt the supply lines to the hidden facility. First, you’ll import a cargo route data set into Cosmos DB using the Graph API. By querying the cargo route graph, you’ll determine the best place to attack in order to disrupt the production of the TIE Fighter prototypes.

1. Create a second Cosmos DB database; this time configured for the Graph API and data model. Navigate to the **Keys** blade in the newly created databases resource and note the `URI` and `PRIMARY KEY` fields. You will need these values later on.

2. Open the **CosmosDBClient** solution and edit the **App.config** class. There are some settings defined here that you’ll need to change to be able to connect to your database. Use the values of the `URI` and `PRIMARY KEY` fields you’ve retrieved earlier to set the `GraphApi.EndpointUrl` and `GraphApi.AuthorizationKey` settings.

3. Open the **Mission3** class and examine the **Seed** method. This method first creates the database and collection if they doesn’t exist yet (see the **Connect** method). Then it will import the cargo route data into the collection.

4. Have a look at the **cargoroutes.txt** file. It contains Gremlin queries to insert all cargo route data into the database. You can use this same syntax in the portal to add data to the graph.

5. Run the **Seed** method from the command line using the following syntax (make sure to run the command from the *bin/Debug* folder):

```
client.exe mission3 seed 
```

6. You can view the added vertices and edges in the portal if you navigate to the **Data Explorer** blade of the database.
It might be a good idea to draw a map for yourself showing the relations between the different object types in the database.

7. You can also use the .NET SDK to query Cosmos DB. Start the client app using the **query** command:

```
client.exe mission3 query
```

This will output all space ports that receive the same type of shipments as the Mos Eisley space port. Examine the **Query** method in the **Mission3** class to see how the data is retrieved. 

You can also run your own queries using the portal or opening an interactive console by running:

```
client.exe mission3 interactive
```

For more information on the Gremlin query syntax, see https://docs.microsoft.com/en-us/azure/cosmos-db/gremlin-support.

8.  Answer the following questions by querying the imported data set:

*What’s the name of the hidden facility? (Hint: there’s only one facility/port on the planet you’ve discovered in mission 2)*

*What’s the main mineral resource being transported to the hidden facility?*

*Advice on an attack plan to disrupt manufacturing of the new TIE models. Which facility / space port should we target?*

Validate your answers using the following command:

```
client.exe mission3 quiz
```
