using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;

namespace mobile_app.Views;

// QueryProperty to receive the lesson title from the LessonsPage
[QueryProperty(nameof(Title), "Title")]
public partial class LessonDetailPage : ContentPage
{
    public string Title { get; set; }

    public LessonDetailPage()
    {
        InitializeComponent();
    }

    // Handler for the "Get Help Now" button
    private async void OnGetHelpClicked(object sender, EventArgs e)
    {
        // Navigates to the main Help tab
        await Shell.Current.GoToAsync("//Help");
    }

    // Handler for the "Mark as Complete" button
    private async void OnMarkCompleteClicked(object sender, EventArgs e)
    {
        // Simple alert and navigation back to the lessons tab
        await Shell.Current.DisplayAlert("Lesson Complete", $"You have finished the lesson on {Title ?? "bullying"}. Keep up the great work!", "Awesome!");
        await Shell.Current.GoToAsync("//Lessons");
    }

    // This method is called when the page is navigated to, allowing UI updates
    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Update the page title based on the passed parameter (for UI feedback)
        this.Title = $"Lesson: {this.Title ?? "Details"}";
    }
}