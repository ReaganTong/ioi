using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Media;

namespace mobile_app.ViewModels;

public partial class ProfileViewModel : ObservableObject
{
    // --- Personal Info ---
    [ObservableProperty]
    private string userName = "Student User";

    [ObservableProperty]
    private string bio = "Computer Science Student | Tech Enthusiast"; // New Field

    [ObservableProperty]
    private string userEmail = "student@uts.edu.my";

    [ObservableProperty]
    private string phoneNumber = "+60 12-345 6789"; // New Field

    [ObservableProperty]
    private string studentId = "12345678";

    [ObservableProperty]
    private ImageSource profileImage = "dotnet_bot.png";

    // --- State Management ---
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotEditing))] // Notify the opposite property too
    private bool isEditing;

    public bool IsNotEditing => !IsEditing; // Helper for XAML visibility

    // --- Stats (Read Only) ---
    [ObservableProperty]
    private int quizzesCompleted = 5;

    [ObservableProperty]
    private int reportsSubmitted = 2;

    // --- Commands ---

    [RelayCommand]
    private void ToggleEditMode()
    {
        // Toggle the state
        IsEditing = !IsEditing;

        // If we just saved (switched to false), show a toast/alert
        if (!IsEditing)
        {
            // Here you would save to database in a real app
            Shell.Current.DisplayAlert("Success", "Profile updated successfully!", "OK");
        }
    }

    [RelayCommand]
    private async Task ChangePhoto()
    {
        if (!IsEditing) return; // Only allow changing photo in Edit Mode

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
        bool answer = await Shell.Current.DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (answer)
        {
            await Shell.Current.GoToAsync("//Login");
        }
    }
}