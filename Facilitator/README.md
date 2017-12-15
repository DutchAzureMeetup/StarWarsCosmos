# Cosmos DB: TIE Fighter Troubles - Facilitator Resources

This folder contains the required resources for facilitating this Cosmos DB lab:
a planet name lookup table and a dashboard API & client application for displaying attendee achievements.

## Prerequisites

To run the Cosmos DB lab facilitators should first create an Azure Cosmos DB instance configured for the Table API.
The table will store a lookup table of planets used in the second mission as well as mission achievements.

## Import Planets lookup table

1. Open the **ImportKeyValueDatabase** solution.

2. Update the **tableStorage** setting in **App.config** with the connection string for your Cosmos DB Table API database.

3. Run the **ConsoleApp1** project to import the planet data.

## Deploy the dashboard web service

1. Open the **StarWarsDashboard** solution.

2. Navigate to the **StarWarsDashboard** project.

3. Update the **StorageAccount** setting in **Web.config**. This is the same connection string as previously used for the Planets lookup table.

4. Deploy the API somewhere reachable by attendees (e.g. Azure Web Apps)

## Start the dashboard client application

The client application is a Console Application that displays attendee achievements retrieved from the dashboard API.
If no achievements are unlocked yet, the application will simply display an empty screen.

1. Navigate to the **ClientConsole** project.

2. Update the **ApiAddress** setting in **App.config** to the address of the deployed dashboard API (e.g. http://starwarsdashboard20171213060739.azurewebsites.net/).

3. Start the application.