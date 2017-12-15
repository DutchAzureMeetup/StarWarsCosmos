using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public abstract class MissionBase
    {
        private readonly int _missionNumber;

        protected readonly QuizService _quizService = new QuizService();

        protected MissionBase(int missionNumber)
        {
            _missionNumber = missionNumber;
        }

        public async Task Quiz()
        {
            var questions = await _quizService.GetQuestions(_missionNumber).ConfigureAwait(false);

            foreach (var question in questions)
            {
                await AskQuestion(question).ConfigureAwait(false);
            }
        }

        private async Task AskQuestion(QuestionDto question)
        {
            AnswerFeedbackDto feedback = null;

            do
            {
                Console.WriteLine("Question:");
                Console.WriteLine(question.Text);
                Console.Write("Answer: ");

                var answer = Console.ReadLine();

                feedback = await _quizService.Answer(_missionNumber, question.Id, answer);

                Console.WriteLine();
                Console.WriteLine(feedback.IsCorrect ? "Correct!" : "There's room for improvement ;-)");
                Console.WriteLine();

                if (!feedback.IsCorrect && !string.IsNullOrEmpty(feedback.Hint))
                {
                    Console.WriteLine(feedback.Hint);
                    Console.WriteLine();
                }
            }
            while (!feedback.IsCorrect);
        }
    }
}
