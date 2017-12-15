using System.Collections.Generic;

namespace StarWarsDashboard.Models
{
    public class Question
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public string CorrectAnswer { get; set; }

        public List<QuestionHint> Hints { get; set; }
    }
}