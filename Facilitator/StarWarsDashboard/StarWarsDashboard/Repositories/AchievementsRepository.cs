using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using StarWarsDashboard.Models;

namespace StarWarsDashboard.Repositories
{
    public class AchievementsRepository
    {
        private readonly CloudTable _achievementsTable;

        public AchievementsRepository()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["StorageAccount"].ConnectionString;
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();

            _achievementsTable = tableClient.GetTableReference("achievements");
            _achievementsTable.CreateIfNotExists();
        }

        public Task AddAchievement(string team, string achievement)
        {
            var operation = TableOperation.InsertOrReplace(new DynamicTableEntity("achievements", team + "_" + achievement)
            {
                ETag = "*",
                Properties = new Dictionary<string, EntityProperty>
                {
                    ["achievement"] = EntityProperty.GeneratePropertyForString(achievement),
                    ["team"] = EntityProperty.GeneratePropertyForString(team)
                }
            });

            return _achievementsTable.ExecuteAsync(operation);
        }

        public Task<List<AchievementDto>> GetAchievements()
        {
            var query = _achievementsTable.CreateQuery<DynamicTableEntity>();

            return Task.FromResult(query.Execute()
                .GroupBy(e => e.Properties["team"].StringValue)
                .Select(g => AggregateAchievements(g))
                .ToList());
        }

        private AchievementDto AggregateAchievements(IGrouping<string, DynamicTableEntity> achievements)
        {
            var names = achievements.Select(e => e.Properties["achievement"].StringValue).ToList();

            var result = new AchievementDto
            {
                Team = achievements.Key,
                Achievements = new List<string>()
            };

            // Ok, so the following is way to hard coded, but the meetup starts in 5 min and
            // I don't have time left to do this properly :-)

            if (names.Contains("Mission1.1")
                && names.Contains("Mission1.2")
                && names.Contains("Mission1.3"))
            {
                result.Achievements.Add("Mission 1");
            }

            if (names.Contains("Mission2.1"))
            {
                result.Achievements.Add("Mission 2");
            }

            if (names.Contains("Mission3.1")
                && names.Contains("Mission3.2")
                && names.Contains("Mission3.3"))
            {
                result.Achievements.Add("Mission 3");
            }

            return result;
        }
    }
}