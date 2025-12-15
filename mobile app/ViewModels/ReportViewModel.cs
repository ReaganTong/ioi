using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using Microsoft.Maui.ApplicationModel;
using System.Threading.Tasks;

#if ANDROID
using Android.Content;
using Android.Provider;
#endif

namespace mobile_app.ViewModels;

public partial class ReportViewModel : ObservableObject
{
    private const string SecurityContact = "082-260991";

    // NEW: Define an event to force the map to move
    public event Action<double, double>? RequestSetLocation;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ImageSource? evidenceImage;

    [ObservableProperty]
    private string locationLabel = "No location set";

    [ObservableProperty]
    private double latitude;

    [ObservableProperty]
    private double longitude;

    private FileResult? _photoFile;

    [RelayCommand]
    private async Task GetCurrentLocation()
    {
        try
        {
            LocationLabel = "Getting location...";

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    LocationLabel = "Permission denied";
                    return;
                }
            }

            // 1. Try Last Known Location (Fast)
            var location = await Geolocation.Default.GetLastKnownLocationAsync();
            if (location != null)
            {
                UpdateLocation(location);
            }

            // 2. Get Fresh Location (Accurate)
            var request = new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromSeconds(10));
            var freshLocation = await Geolocation.Default.GetLocationAsync(request);

            if (freshLocation != null)
            {
                UpdateLocation(freshLocation);
            }
            else if (location == null)
            {
                LocationLabel = "Could not find location";
            }
        }
        catch (FeatureNotEnabledException)
        {
            LocationLabel = "Location is off";
            bool openSettings = await Shell.Current.DisplayAlert("Location Disabled", "Please turn on location services.", "Open Settings", "Cancel");
            if (openSettings)
            {
#if ANDROID
                var intent = new Intent(Settings.ActionLocationSourceSettings);
                intent.AddFlags(ActivityFlags.NewTask);
                Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(intent);
#else
                AppInfo.Current.ShowSettingsUI();
#endif
            }
        }
        catch (Exception ex)
        {
            LocationLabel = $"Error: {ex.Message}";
        }
    }

    // Helper to update data AND fire the map event
    private void UpdateLocation(Location loc)
    {
        Latitude = loc.Latitude;
        Longitude = loc.Longitude;
        LocationLabel = $"📍 {Latitude:F4}, {Longitude:F4}";

        // FIX: Directly tell the View to move the map
        RequestSetLocation?.Invoke(Latitude, Longitude);
    }

    [RelayCommand]
    private async Task CallSecurity()
    {
        try
        {
            string cleanNumber = SecurityContact.Replace(" ", "");
            if (PhoneDialer.Default.IsSupported) PhoneDialer.Default.Open(cleanNumber);
            else await Launcher.Default.OpenAsync($"tel:{cleanNumber}");
        }
        catch { await Shell.Current.DisplayAlert("Failed", $"Dial {SecurityContact}", "OK"); }
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        try
        {
            string action = await Shell.Current.DisplayActionSheet("Add Evidence", "Cancel", null, "Take Photo", "Choose from Gallery");
            if (action == "Cancel") return;

            FileResult? photo = null;
            if (action == "Take Photo" && MediaPicker.Default.IsCaptureSupported)
                photo = await MediaPicker.Default.CapturePhotoAsync();
            else if (action == "Choose from Gallery")
                photo = await MediaPicker.Default.PickPhotoAsync();

            if (photo != null)
            {
                _photoFile = photo;
                var stream = await photo.OpenReadAsync();
                EvidenceImage = ImageSource.FromStream(() => stream);
            }
        }
        catch { }
    }

    [RelayCommand]
    private async Task SubmitReport()
    {
        if (string.IsNullOrEmpty(Description))
        {
            await Shell.Current.DisplayAlert("Required", "Please describe the incident.", "OK");
            return;
        }
        await Shell.Current.DisplayAlert("Report Sent", "Incident reported.", "OK");
        Description = string.Empty;
        EvidenceImage = null;
        LocationLabel = "No location set";
    }
}