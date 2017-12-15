using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using StarWarsDashboard.Models;
using StarWarsDashboard.Repositories;
using Swashbuckle.Swagger.Annotations;

namespace StarWarsDashboard.Controllers
{
    public class QuizController : ApiController
    {
        private readonly AchievementsRepository _repository = new AchievementsRepository();

        [SwaggerOperation("GetById")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.NotFound)]
        public QuizDto Get(int id)
        {
            var quiz = Quiz.Missions[id - 1];

            return new QuizDto
            {
                Id = quiz.Id,
                Questions = quiz.Questions
                    .Select(q => new QuestionDto
                    {
                        Id = q.Id,
                        Text = q.Text
                    })
                    .ToList()
            };
        }

        [SwaggerOperation("Create")]
        [SwaggerResponse(HttpStatusCode.Created)]
        public AnswerFeedbackDto Post(int id, [FromBody]AnswerDto answer)
        {
            var result = new AnswerFeedbackDto();
            var quiz = Quiz.Missions[id - 1];

            var question = quiz.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
            if (question != null)
            {
                if (string.Equals(answer.Text.Trim(), question.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
                {
                    _repository.AddAchievement(answer.TeamName, answer.QuestionId);

                    result.IsCorrect = true;
                }
                else
                {
                    var hint = question.Hints.FirstOrDefault(
                        h => string.Equals(answer.Text.Trim(), h.GivenAnswer, StringComparison.OrdinalIgnoreCase));

                    if (hint != null)
                    {
                        result.Hint = hint.Text;
                    }
                }
            }

            return result;
        }
    }
}
