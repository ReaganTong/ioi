using mobile_app.ViewModels;
using mobile_app.Models; // ✅ Needed to recognize 'Lesson'
using Microsoft.Maui.Controls;

namespace mobile_app.Views;

public partial class LessonsPage : ContentPage
{
    public LessonsPage(LessonViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    // ✅ This method runs automatically when a user taps an item
    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // 1. Check if an item was actually selected
        if (e.CurrentSelection.FirstOrDefault() is Lesson selectedLesson)
        {
            // 2. Trigger the navigation command in your ViewModel
            var viewModel = (LessonViewModel)BindingContext;
            await viewModel.GoToDetailsCommand.ExecuteAsync(selectedLesson);

            // 3. CRITICAL: Unselect the item immediately.
            // This resets the list so the user can tap the same item again later.
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}