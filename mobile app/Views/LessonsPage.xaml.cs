// mobile app/Views/LessonsPage.xaml.cs
using mobile_app.ViewModels;
using Microsoft.Maui.Controls;

namespace mobile_app.Views;

// FIX: Added 'partial' keyword
public partial class LessonsPage : ContentPage
{
    // Inject the ViewModel
    public LessonsPage(LessonViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}