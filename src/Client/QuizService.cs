using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client
{
    public class QuizService
    {
        private readonly string _teamName = "Red 1";
        private readonly HttpClient _client;

        public QuizService()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(ConfigurationManager.AppSettings["QuizApi.BaseAddress"]);
        }

        public async Task<List<QuestionDto>> GetQuestions(int mission)
        {
            var json = await _client.GetStringAsync(mission.ToString());

            var quiz = JsonConvert.DeserializeObject<QuizDto>(json);

            return quiz.Questions;
        }

        public async Task<AnswerFeedbackDto> Answer(int mission, string questionId, string answerText)
        {
            var answer = new AnswerDto
            {
                TeamName = _teamName,
                QuestionId = questionId,
                Text = answerText
            };

            var requestJson = JsonConvert.SerializeObject(answer);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            
            var response = await _client.PostAsync(mission.ToString(), content);
            var responseJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AnswerFeedbackDto>(responseJson);
        }
    }
}
