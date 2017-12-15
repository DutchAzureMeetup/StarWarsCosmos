using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://starwarsdashboard20171213060739.azurewebsites.net/");

            Console.Title = "Star Wars - TIE Fighter Troubles - Leaderboard";

            while (true)
            {
                var json = await client.GetStringAsync("api/achievements");

                var achievements = JsonConvert.DeserializeObject<List<AchievementDto>>(json)
                    .OrderByDescending(a => a.Achievements.Count)
                    .ThenBy(a => a.Team)
                    .ToList();

                foreach (var achievement in achievements)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(achievement.Team.Substring(0, Math.Min(30, achievement.Team.Length)).PadRight(35));
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Join(", ", achievement.Achievements).PadRight(40));
                }

                Console.SetCursorPosition(0, Console.CursorTop - achievements.Count);

                await Task.Delay(1000);
            }
        }
    }

    public class AchievementDto
    {
        public string Team { get; set; }

        public List<string> Achievements { get; set; }
    }
}
