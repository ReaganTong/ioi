using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; // Required for RelayCommand
using mobile_app.Models;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace mobile_app.ViewModels;

public partial class QuizViewModel : ObservableObject // changed to partial and inherit from ObservableObject
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

    // This is the method you asked to add
    [RelayCommand]
    private async Task PlayQuiz(Quiz quiz)
    {
        if (quiz == null) return;

        // Navigate to the playing page and pass the Quiz Title
        await Shell.Current.GoToAsync($"QuizPlay?Title={quiz.Title}");
    }
}