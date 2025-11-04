using mobile_app.ViewModels;
using Microsoft.Maui.Controls;

namespace mobile_app.Views;

public partial class LessonsPage : ContentPage
{
    // Inject the ViewModel
    public LessonsPage(LessonViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}