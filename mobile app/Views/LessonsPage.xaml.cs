using mobile_app.ViewModels;
using mobile_app.Models; // ✅ Important: Required to see 'Lesson' class
using Microsoft.Maui.Controls;

namespace mobile_app.Views;

public partial class LessonsPage : ContentPage
{
    public LessonsPage(LessonViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    // ✅ FIX: This method runs instantly when you tap an item
    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // 1. Get the item user tapped
        if (e.CurrentSelection.FirstOrDefault() is Lesson selectedLesson)
        {
            // 2. Trigger the navigation command
            var viewModel = (LessonViewModel)BindingContext;
            await viewModel.GoToDetailsCommand.ExecuteAsync(selectedLesson);

            // 3. Reset the selection so it can be clicked again immediately
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}