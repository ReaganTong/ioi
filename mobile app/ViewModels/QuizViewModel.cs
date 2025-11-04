using System.Collections.ObjectModel;
using mobile_app.Models;

namespace mobile_app.ViewModels;

public class QuizViewModel
{
    public ObservableCollection<Quiz> Quizzes { get; set; }

    public QuizViewModel()
    {
        Quizzes = new ObservableCollection<Quiz>
        {
            new Quiz { Title = "Cyberbullying", Description = "Test your awareness on online bullying.", Icon = "cyber.png", Color = "#FF6B6B" },
            new Quiz { Title = "Verbal Bullying", Description = "How words can harm.", Icon = "verbal.png", Color = "#4ECDC4" }
        };
    }
}
