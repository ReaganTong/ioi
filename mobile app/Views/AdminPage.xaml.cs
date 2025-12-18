using mobile_app.ViewModels;

namespace mobile_app.Views;

// FIX: Inherit from TabbedPage, NOT ContentPage
public partial class AdminPage : TabbedPage
{
    public AdminPage(AdminViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}