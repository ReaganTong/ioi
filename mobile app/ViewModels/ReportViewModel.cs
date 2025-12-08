using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel;
using System.Threading.Tasks;

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

    private FileResult? _photoFile;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // NEW: Command to get device GPS location
    [RelayCommand]
    private async Task GetCurrentLocation()
    {
        try
        {
            LocationLabel = "Getting location...";

            // Check permissions first (omitted for brevity, handled by OS usually)
            var location = await Geolocation.Default.GetLastKnownLocationAsync();

            if (location == null)
            {
                location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
                {
                    DesiredAccuracy = GeolocationAccuracy.Medium,
                    Timeout = TimeSpan.FromSeconds(30)
                });
            }

            if (location != null)
            {
                Latitude = location.Latitude;
                Longitude = location.Longitude;
                LocationLabel = $"📍 Location set: {Latitude:F4}, {Longitude:F4}";
            }
            else
            {
                LocationLabel = "Unable to get location";
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
            PhoneDialer.Default.Open(SecurityContact);
        }
        catch (Exception)
        {
            try
            {
                await Launcher.Default.OpenAsync($"tel:{SecurityContact}");
            }
            catch
            {
                await Shell.Current.DisplayAlert("Action Failed",
                    $"Could not open dialer. Please manually call UTS Security: {SecurityContact}", "OK");
            }
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
                else
                    await Shell.Current.DisplayAlert("Error", "Camera not supported", "OK");
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
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Could not pick photo: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task SubmitReport()
    {
        if (string.IsNullOrEmpty(Description))
        {
            await Shell.Current.DisplayAlert("Error", "Please describe the incident.", "OK");
            return;
        }

        // Logic to send data to backend...
        await Shell.Current.DisplayAlert("Report Sent", $"Incident reported at {LocationLabel}", "OK");

        Description = string.Empty;
        EvidenceImage = null;
        LocationLabel = "No location set";
    }
}