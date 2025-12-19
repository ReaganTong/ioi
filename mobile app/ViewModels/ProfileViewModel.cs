using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Media;
using Supabase;
using mobile_app.Views;
using mobile_app.Models;

namespace mobile_app.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    private readonly Client _supabase;

    public ProfileViewModel(Client supabase)
    {
        _supabase = supabase;
        // We don't auto-load here anymore to prevent double-loading.
        // The View (ProfilePage.xaml.cs) will call RefreshProfile().
    }

    // --- Personal Info ---

    [ObservableProperty]
    private string userName = "Loading...";

    [ObservableProperty]
    private string bio = "Computer Science Student | Tech Enthusiast";

    [ObservableProperty]
    private string userEmail = "";

    [ObservableProperty]
    private string phoneNumber = "+60 12-345 6789";

    [ObservableProperty]
    private string studentId = "Loading...";

    [ObservableProperty]
    private ImageSource profileImage = "dotnet_bot.png";

    // --- State Management ---

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotEditing))]
    private bool isEditing;

    public bool IsNotEditing => !IsEditing;

    // --- Stats ---

    [ObservableProperty]
    private int reportsSubmitted = 0;

    // --- Logic ---

    // PUBLIC method so the Page can call it
    public async Task RefreshProfile()
    {
        var currentUser = _supabase.Auth.CurrentUser;

        if (currentUser != null)
        {
            UserEmail = currentUser.Email;

            // 1. Set Name and ID
            if (UserEmail.Contains("@"))
            {
                StudentId = UserEmail.Split('@')[0];
                UserName = $"Student {StudentId}";
            }

            try
            {
                // 2. Fetch Report Count (Real DB Call)
                var reportCount = await _supabase
                    .From<ReportModel>()
                    .Where(x => x.StudentId == UserEmail)
                    .Count(Supabase.Postgrest.Constants.CountType.Exact);

                ReportsSubmitted = reportCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading stats: {ex.Message}");
            }
        }
    }

    // --- Commands ---

    [RelayCommand]
    private void ToggleEditMode()
    {
        IsEditing = !IsEditing;
        if (!IsEditing)
        {
            Shell.Current.DisplayAlert("Success", "Profile updated locally!", "OK");
        }
    }

    [RelayCommand]
    private async Task ChangePhoto()
    {
        if (!IsEditing) return;

        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo != null)
            {
                var stream = await photo.OpenReadAsync();
                ProfileImage = ImageSource.FromStream(() => stream);
            }
        }
        catch
        {
            await Shell.Current.DisplayAlert("Error", "Could not pick photo", "OK");
        }
    }

    [RelayCommand]
    private async Task Logout()
    {
        bool answer = await Shell.Current.DisplayAlert("Logout", "Are you sure?", "Yes", "No");
        if (answer)
        {
            await _supabase.Auth.SignOut();
            Application.Current.MainPage = new LoginPage(new LoginViewModel(_supabase));
        }
    }
}