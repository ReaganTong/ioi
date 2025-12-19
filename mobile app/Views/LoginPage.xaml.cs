using mobile_app.ViewModels;

namespace mobile_app.Views;

public partial class LoginPage : ContentPage
{
    // FIX: Add this constructor so ProfileViewModel can navigate here
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}