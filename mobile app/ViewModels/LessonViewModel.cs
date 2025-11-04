using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace mobile_app.ViewModels;

public partial class LessonViewModel : ObservableObject
{
    public ObservableCollection<Lesson> Lessons { get; set; }

    public LessonViewModel()
    {
        Lessons = new ObservableCollection<Lesson>
        {
            new Lesson { Title = "Physical Bullying", ShortDescription = "Harming someone's body or possessions.", Icon = "👊", Color = "#FF6B6B", Route = "LessonDetail" },
            new Lesson { Title = "Verbal Bullying", ShortDescription = "Using words, threats, or insults.", Icon = "🗣️", Color = "#4ECDC4", Route = "LessonDetail" },
            new Lesson { Title = "Cyber Bullying", ShortDescription = "Bullying through digital devices or the internet.", Icon = "📱", Color = "#FFC652", Route = "LessonDetail" },
            new Lesson { Title = "Emotional Bullying", ShortDescription = "Harm to social standing or reputation.", Icon = "💔", Color = "#4A90E2", Route = "LessonDetail" },
        };
    }

    [RelayCommand]
    private async Task GoToDetails(Lesson lesson)
    {
        // Passes the Title as a query parameter
        await Shell.Current.GoToAsync($"{lesson.Route}?Title={lesson.Title}");
    }
}