using mobile_app.Models;
using mobile_app.ViewModels;

namespace mobile_app.Views;

[QueryProperty(nameof(LessonTitle), "Title")]
public partial class LessonDetailPage : ContentPage
{
    private readonly LessonViewModel _viewModel;
    public string? LessonTitle { get; set; }

    // Inject the ViewModel to access the list of lessons
    public LessonDetailPage(LessonViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!string.IsNullOrEmpty(LessonTitle))
        {
            // Find the lesson that matches the Title passed from the previous page
            var selectedLesson = _viewModel.Lessons.FirstOrDefault(l => l.Title == LessonTitle);

            // Set the BindingContext to this specific lesson so the XAML can display its data
            if (selectedLesson != null)
            {
                BindingContext = selectedLesson;
            }
        }
    }

    private async void OnGetHelpClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Help");
    }

    private async void OnMarkCompleteClicked(object sender, EventArgs e)
    {
        await Shell.Current.DisplayAlert("Great Job!", "You've completed this lesson.", "OK");
        await Shell.Current.GoToAsync("//Lessons");
    }
}