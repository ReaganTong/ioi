using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Supabase;

namespace mobile_app.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly Client _supabase;

    public LoginViewModel(Client supabase)
    {
        _supabase = supabase;
    }

    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private bool isRememberMe = true;

    // FIX: Added this command so tapping the label toggles the box
    [RelayCommand]
    private void ToggleRememberMe()
    {
        IsRememberMe = !IsRememberMe;
    }

    [RelayCommand]
    async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Please enter valid credentials", "OK");
            return;
        }

        try
        {
            // Login
            var session = await _supabase.Auth.SignIn(Email, Password);

            if (session != null)
            {
                // Save Preference
                Preferences.Default.Set("RememberMe", IsRememberMe);

                // Go to App
                Application.Current.MainPage = new AppShell();
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Login Failed", ex.Message, "OK");
        }
    }

    [RelayCommand]
    async Task SignUp()
    {
        if (!Email.EndsWith("@student.uts.edu.my") && !Email.EndsWith("@uts.edu.my"))
        {
            await Application.Current.MainPage.DisplayAlert("Restricted", "Use your student email.", "OK");
            return;
        }
        try
        {
            await _supabase.Auth.SignUp(Email, Password);
            await Application.Current.MainPage.DisplayAlert("Success", "Verification link sent!", "OK");
        }
        catch (Exception ex) { await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK"); }
    }
}