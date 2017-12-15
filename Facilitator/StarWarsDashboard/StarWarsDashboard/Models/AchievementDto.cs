using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarWarsDashboard.Models
{
    public class AchievementDto
    {
        public string Team { get; set; }

        public List<string> Achievements { get; set; }
    }
}