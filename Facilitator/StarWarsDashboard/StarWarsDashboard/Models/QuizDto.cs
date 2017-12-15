using System.Collections.Generic;

namespace StarWarsDashboard.Models
{
    public class QuizDto
    {
        public string Id { get; set; }

        public List<QuestionDto> Questions { get; set; }
    }
}