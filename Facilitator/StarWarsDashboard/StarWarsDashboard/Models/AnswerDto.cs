using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StarWarsDashboard.Models
{
    public class AnswerDto
    {
        public string TeamName { get; set; }

        public string QuestionId { get; set; }

        public string Text { get; set; }
    }
}