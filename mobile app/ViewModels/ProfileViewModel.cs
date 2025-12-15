using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace mobile_app.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    [ObservableProperty]
    private string userName = "Student User";

    [ObservableProperty]
    private string userEmail = "student@uts.edu.my";

    [ObservableProperty]
    private string studentId = "12345678";

    // Placeholder stats
    [ObservableProperty]
    private int quizzesCompleted = 3;

    [ObservableProperty]
    private int reportsSubmitted = 1;

    [RelayCommand]
    private async Task Logout()
    {
        bool answer = await Shell.Current.DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (answer)
        {
            // Navigate back to login page
            await Shell.Current.GoToAsync("//Login");
        }
    }
}