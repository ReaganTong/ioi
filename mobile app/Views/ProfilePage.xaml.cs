using mobile_app.ViewModels;

namespace mobile_app.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // CRITICAL FIX: Refresh data every time the page appears
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProfileViewModel vm)
        {
            await vm.RefreshProfile();
        }
    }
}