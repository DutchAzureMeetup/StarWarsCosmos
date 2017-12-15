using System.Collections.Generic;

namespace StarWarsDashboard.Models
{
    public class Quiz
    {
        public static readonly List<Quiz> Missions = new List<Quiz>
        {
            new Quiz
            {
                Id = "Mission1",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = "Mission1.1",
                        Text = "What’s the average size of the crew on cargo flights carrying a payload of less than or equal to 50 metric tons (round to nearest integer)?",
                        CorrectAnswer = "2"
                    },
                    new Question
                    {
                        Id = "Mission1.2",
                        Text = "How many non-cargo / fighter flights are there in the collection?",
                        CorrectAnswer = "560"
                    },
                    new Question
                    {
                        Id = "Mission1.3",
                        Text = "What are the coordinates of the planet where the new TIE Fighter prototypes are being tested? (Note that the new prototypes are equipped with an hyperdrive).",
                        CorrectAnswer = "998086,89;902999,24;823496,85"
                    }
                }
            },
            new Quiz
            {
                Id = "Mission2",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = "Mission2.1",
                        Text = "What is the name of the planet where the new TIE Fighter prototypes are being tested?",
                        CorrectAnswer = "Kessel"
                    }
                }                
            },
            new Quiz
            {
                Id = "Mission3",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Id = "Mission3.1",
                        Text = "What’s the name of the hidden facility? (Hint: there’s only one facility/port on the planet you’ve discovered in mission 2)",
                        CorrectAnswer = "Maw Installation"
                    },
                    new Question
                    {
                        Id = "Mission3.2",
                        Text = "What’s the main mineral resource being transported to the hidden facility?",
                        CorrectAnswer = "Thorilide"
                    },
                    new Question
                    {
                        Id = "Mission3.3",
                        Text = "Advice on an attack plan to disrupt manufacturing of the new TIE models. Which facility / space port should we target?",
                        CorrectAnswer = "Glogg Terminal",
                        Hints = new List<QuestionHint>
                        {
                            new QuestionHint
                            {
                                GivenAnswer = "mawinstallation.3",
                                Text = "Enter the name of the vertice, not an id."
                            },
                            new QuestionHint
                            {
                                GivenAnswer = "gorseport.3",
                                Text = "Enter the name of the vertice, not an id."
                            },
                            new QuestionHint
                            {
                                GivenAnswer = "gloggterminal.3",
                                Text = "Enter the name of the vertice, not an id."
                            },
                            new QuestionHint
                            {
                                GivenAnswer = "Maw Installation",
                                Text = "This installation was built from asteroids in the midst of the Maw Cluster near Kessel. Maw Cluster is a virtually unnavigable cluster of black holes. It's probably easier to attack the ports providing supplies for the installation."
                            },
                            new QuestionHint
                            {
                                GivenAnswer = "Gorse Port",
                                Text = "Initial scans show a large empirial force guarding the Gorse Port. Do you have an alternative? Note that Thorilide was initially found in crystalline form and is mined using highly explosive baradium bisulfate."
                            }
                        }
                    }
                }
            }
        };

        public string Id { get; set; }

        public List<Question> Questions { get; set; }
    }
}