using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;

namespace CosmosDBClient
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Syntax: dotnet client.dll <missionx> <command>");
                Console.WriteLine();
                Console.WriteLine("Example: dotnet client.dll mission1 seed");
                return;
            }

            var mission = args[0];
            var command = args[1];

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            IConfigurationRoot configuration = builder.Build();

            try
            {
                // Create an instance of the correct Mission class.
                var missionType = Type.GetType($"CosmosDBClient.{mission}", true, true);
                var missionInstance = Activator.CreateInstance(missionType, configuration);

                // Execute the mission method requested by the user.
                var commandMethod = missionType.GetMethod(command, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                var commandTask = (Task)commandMethod.Invoke(missionInstance, null);
                commandTask.GetAwaiter().GetResult();
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
        }
    }
}
