using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace mobile_app.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    private string username;

    [ObservableProperty]
    private string password;

    [RelayCommand]
    async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter valid credentials", "OK");
            return;
        }

        // MOCK LOGIN LOGIC (Replace with Backend API later)
        // For now, we just let them in.

        // Navigate to the main AppShell (The tabs)
        Application.Current.MainPage = new AppShell();
    }
}