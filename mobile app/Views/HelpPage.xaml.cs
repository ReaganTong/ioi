using mobile_app.ViewModels;
using Microsoft.Maui.Controls;

namespace mobile_app.Views;

public partial class HelpPage : ContentPage
{
    // Inject the ViewModel
    public HelpPage(HelpViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}