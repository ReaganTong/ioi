using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using System.Collections.ObjectModel;
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

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ImageSource? evidenceImage;

    [ObservableProperty]
    private string locationLabel = "No location set";

    // NASA Worldview URL
    [ObservableProperty]
    private string mapUrl = "https://worldview.earthdata.nasa.gov/";

    private FileResult? _photoFile;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

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

            // 'Best' accuracy tries to use GPS/Network to get a fix
            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(20));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location != null)
            {
                Latitude = location.Latitude;
                Longitude = location.Longitude;
                LocationLabel = $"📍 {Latitude:F4}, {Longitude:F4}";

                // Update NASA Map to zoom into the user's location using a bounding box
                double offset = 0.05; // Roughly 5km area
                string bbox = $"{Longitude - offset},{Latitude - offset},{Longitude + offset},{Latitude + offset}";
                MapUrl = $"https://worldview.earthdata.nasa.gov/?v={bbox}";
            }
            else
            {
                LocationLabel = "Could not find location";
            }
        }
        catch (FeatureNotEnabledException)
        {
            LocationLabel = "Location is off";

            bool openSettings = await Shell.Current.DisplayAlert(
                "Location Disabled",
                "Your phone's location is turned off. Please enable it in Settings.",
                "Open Settings", "Cancel");

            if (openSettings)
            {
#if ANDROID
                // Opens the specific GPS toggle screen on Android
                var intent = new Intent(Settings.ActionLocationSourceSettings);
                intent.AddFlags(ActivityFlags.NewTask);
                Microsoft.Maui.ApplicationModel.Platform.CurrentActivity.StartActivity(intent);
#else
                // iOS fallback
                AppInfo.Current.ShowSettingsUI();
#endif
            }
        }
        catch (Exception ex)
        {
            LocationLabel = $"Error: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task CallSecurity()
    {
        try
        {
            string cleanNumber = SecurityContact.Replace(" ", "");
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open(cleanNumber);
            else
                await Launcher.Default.OpenAsync($"tel:{cleanNumber}");
        }
        catch
        {
            await Shell.Current.DisplayAlert("Failed", $"Please dial {SecurityContact}", "OK");
        }
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        try
        {
            string action = await Shell.Current.DisplayActionSheet("Add Evidence", "Cancel", null, "Take Photo", "Choose from Gallery");
            if (action == "Cancel") return;

            FileResult? photo = null;
            if (action == "Take Photo")
            {
                if (MediaPicker.Default.IsCaptureSupported)
                    photo = await MediaPicker.Default.CapturePhotoAsync();
            }
            else if (action == "Choose from Gallery")
            {
                photo = await MediaPicker.Default.PickPhotoAsync();
            }

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
        await Shell.Current.DisplayAlert("Report Sent", "Incident reported to UTS Security.", "OK");
        Description = string.Empty;
        EvidenceImage = null;
        LocationLabel = "No location set";
        MapUrl = "https://worldview.earthdata.nasa.gov/";
    }
}