using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using StarWarsDashboard.Models;
using StarWarsDashboard.Repositories;

namespace StarWarsDashboard.Controllers
{
    public class AchievementsController : ApiController
    {
        private readonly AchievementsRepository _repository = new AchievementsRepository();

        // GET api/<controller>
        public async Task<IEnumerable<AchievementDto>> Get()
        {
            return await _repository.GetAchievements();
        }
    }
}