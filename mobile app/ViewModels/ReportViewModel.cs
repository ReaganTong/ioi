using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;
using Supabase;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Media;

namespace mobile_app.ViewModels;

public partial class ReportViewModel : ObservableObject
{
    private readonly Client _supabase;

    // FIX 1: Restore the event so the Map can listen to it
    public event Action<double, double>? RequestSetLocation;

    public ReportViewModel(Client supabase)
    {
        _supabase = supabase;
    }

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ImageSource? evidenceImage;

    [ObservableProperty]
    private string locationLabel = "No location set";

    // FIX 2: Use ONLY ObservableProperty. Do NOT add public double Latitude { get; set; } manually.
    [ObservableProperty]
    private double latitude;

    [ObservableProperty]
    private double longitude;

    private FileResult? _photoFile;

    // --- LOCATION LOGIC ---

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
        catch (Exception ex)
        {
            LocationLabel = $"Error: {ex.Message}";
        }
    }

    private void UpdateLocation(Location loc)
    {
        Latitude = loc.Latitude;
        Longitude = loc.Longitude;
        LocationLabel = $"📍 {Latitude:F4}, {Longitude:F4}";

        // Fire event to move the map
        RequestSetLocation?.Invoke(Latitude, Longitude);
    }

    // --- SUBMISSION LOGIC ---

    [RelayCommand]
    private async Task SubmitReport()
    {
        if (string.IsNullOrEmpty(Description))
        {
            await Shell.Current.DisplayAlert("Required", "Please describe the incident.", "OK");
            return;
        }

        try
        {
            var newReport = new ReportModel
            {
                Description = this.Description,
                Location = $"{Latitude},{Longitude}",
                StudentId = "12345678" // Hardcoded for now, or get from Profile
            };

            // Insert into Supabase
            await _supabase.From<ReportModel>().Insert(newReport);

            await Shell.Current.DisplayAlert("Success", "Report sent to Supabase!", "OK");

            // Reset Form
            Description = string.Empty;
            EvidenceImage = null;
            LocationLabel = "No location set";
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to send: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        try
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();
                if (photo != null)
                {
                    _photoFile = photo;
                    var stream = await photo.OpenReadAsync();
                    EvidenceImage = ImageSource.FromStream(() => stream);
                }
            }
        }
        catch { }
    }

    [RelayCommand]
    private async Task CallSecurity()
    {
        try
        {
            if (PhoneDialer.Default.IsSupported)
                PhoneDialer.Default.Open("082-260991");
        }
        catch { }
    }
}