using mobile_app.ViewModels;
using Microsoft.Maui.Controls;

namespace mobile_app.Views;

public partial class LessonsPage : ContentPage
{
    public LessonsPage(LessonViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    // ✅ DELETED: OnSelectionChanged is removed. 
    // The XAML TapGestureRecognizer now handles navigation.
}