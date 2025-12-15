using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;

namespace mobile_app.ViewModels;

public partial class LessonViewModel : ObservableObject
{
    // List 1: Lessons
    public ObservableCollection<Lesson> Lessons { get; set; }

    // List 2: Quizzes (Moved from QuizViewModel)
    public ObservableCollection<Quiz> Quizzes { get; set; }

    public LessonViewModel()
    {
        // --- Initialize Lessons ---
        Lessons = new ObservableCollection<Lesson>
        {
            new Lesson
            {
                Title = "Physical Bullying",
                ShortDescription = "Harming someone's body or possessions.",
                Icon = "👊",
                Color = "#FF6B6B",
                Route = "LessonDetail",
                LongDefinition = "Physical bullying involves hurting someone's body or possessions. It includes hitting, kicking, tripping, pinching, pushing, or damaging someone's property.",
                Examples = new List<string> { "• Hitting, kicking, or punching", "• Tripping or pushing someone", "• Breaking or stealing belongings" },
                ActionSteps = new List<string> { "1. Get away to a safe place immediately.", "2. Stay in a group.", "3. Tell a trusted adult or campus security." }
            },
            new Lesson
            {
                Title = "Cyberbullying",
                ShortDescription = "Bullying via digital devices.",
                Icon = "📱",
                Color = "#FFC652",
                Route = "LessonDetail",
                LongDefinition = "Cyberbullying takes place over digital devices. It includes sending, posting, or sharing negative, harmful, false, or mean content about someone else.",
                Examples = new List<string> { "• Sending mean texts", "• Posting embarrassing photos", "• Spreading rumors online" },
                ActionSteps = new List<string> { "1. Do not respond.", "2. Save the evidence.", "3. Block and report." }
            },
            new Lesson
            {
                Title = "Verbal Bullying",
                ShortDescription = "Using words to hurt others.",
                Icon = "🗣️",
                Color = "#4ECDC4",
                Route = "LessonDetail",
                LongDefinition = "Verbal bullying is saying or writing mean things. It includes teasing, name-calling, or threatening to cause harm.",
                Examples = new List<string> { "• Teasing or name-calling", "• Inappropriate sexual comments", "• Taunting" },
                ActionSteps = new List<string> { "1. Ignore and walk away.", "2. Tell them to stop firmly.", "3. Report to a counselor." }
            },
             new Lesson
            {
                Title = "Emotional Bullying",
                ShortDescription = "Harming social reputation.",
                Icon = "💔",
                Color = "#4A90E2",
                Route = "LessonDetail",
                LongDefinition = "Relational aggression involves hurting someone's reputation or relationships.",
                Examples = new List<string> { "• Leaving someone out", "• Spreading rumors", "• Public embarrassment" },
                ActionSteps = new List<string> { "1. Focus on true friends.", "2. Do activities you enjoy.", "3. Talk to a counselor." }
            }
        };

        // --- Initialize Quizzes ---
        Quizzes = new ObservableCollection<Quiz>
        {
            new Quiz
            {
                Title = "Cyberbullying Awareness",
                Description = "Test your knowledge on digital safety.",
                Icon = "cyber.png",
                Color = "#6C5CE7", // Different color to distinguish from lessons
                Questions = new List<Question>
                {
                    new Question { Text = "What should you do if you receive a mean message?", OptionA = "Delete it", OptionB = "Reply back", OptionC = "Save evidence & tell adult", CorrectOption = 3 },
                    new Question { Text = "Is excluding someone online bullying?", OptionA = "Yes", OptionB = "No", OptionC = "Maybe", CorrectOption = 1 }
                }
            },
            new Quiz
            {
                Title = "Verbal Harassment",
                Description = "Identify verbal bullying signs.",
                Icon = "verbal.png",
                Color = "#00CEC9",
                Questions = new List<Question>
                {
                    new Question { Text = "Which is verbal bullying?", OptionA = "Complimenting", OptionB = "Name-calling", OptionC = "Asking for help", CorrectOption = 2 }
                }
            }
        };
    }

    // Command to open Lesson Details
    [RelayCommand]
    private async Task GoToDetails(Lesson lesson)
    {
        if (lesson == null) return;
        var navigationParameter = new Dictionary<string, object> { { "Title", lesson.Title } };
        await Shell.Current.GoToAsync(lesson.Route, navigationParameter);
    }

    // Command to start a Quiz
    [RelayCommand]
    private async Task PlayQuiz(Quiz quiz)
    {
        if (quiz == null) return;
        await Shell.Current.GoToAsync($"QuizPlay?Title={quiz.Title}");
    }
}