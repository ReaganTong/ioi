// mobile app/Views/LessonDetailPage.xaml.cs
using Microsoft.Maui.Controls;

namespace mobile_app.Views;

// FIX: Added 'partial' keyword
[QueryProperty(nameof(LessonTitle), "Title")]
public partial class LessonDetailPage : ContentPage
{
    public string? LessonTitle { get; set; }

    public LessonDetailPage()
    {
        InitializeComponent();
    }

    // Handler for the "Get Help Now" button (Navigate to Help Tab)
    private async void OnGetHelpClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//Help");
    }

    // Handler for the "Mark as Complete" button
    private async void OnMarkCompleteClicked(object sender, EventArgs e)
    {
        await Shell.Current.DisplayAlert("Lesson Complete", $"You have finished the lesson on {LessonTitle ?? "bullying"}. Keep up the great work!", "Awesome!");
        await Shell.Current.GoToAsync("//Lessons");
    }

    // Update the actual page Title property in OnAppearing for UI consistency
    protected override void OnAppearing()
    {
        base.OnAppearing();
        this.Title = $"Lesson: {this.LessonTitle ?? "Details"}";
    }
}