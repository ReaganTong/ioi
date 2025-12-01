using System.Collections.ObjectModel;
using mobile_app.Models;

namespace mobile_app.ViewModels;

public class QuizViewModel
{
    // FIX 1: Ensure this property exists directly inside the class
    public ObservableCollection<Quiz> Quizzes { get; set; }

    public QuizViewModel()
    {
        Quizzes = new ObservableCollection<Quiz>
        {
            new Quiz
            {
                Title = "Cyberbullying Awareness",
                Description = "Test your knowledge on digital safety.",
                Icon = "cyber.png",
                Color = "#FF6B6B",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "What should you do if you receive a mean message?",
                        OptionA = "Delete it immediately",
                        OptionB = "Reply with a mean message",
                        OptionC = "Save evidence and tell an adult",
                        CorrectOption = 3
                    },
                    new Question
                    {
                        Text = "Is excluding someone from an online group bullying?",
                        OptionA = "Yes, it is exclusion",
                        OptionB = "No, it's just a preference",
                        OptionC = "Only if you tell them",
                        CorrectOption = 1
                    }
                }
            },
            new Quiz
            {
                Title = "Verbal Bullying",
                Description = "Identify verbal harassment.",
                Icon = "verbal.png",
                Color = "#4ECDC4",
                Questions = new List<Question>
                {
                    new Question
                    {
                        Text = "Which of these is verbal bullying?",
                        OptionA = "Giving a compliment",
                        OptionB = "Name-calling and teasing",
                        OptionC = "Asking for help",
                        CorrectOption = 2
                    }
                }
            }
        };
    }
}