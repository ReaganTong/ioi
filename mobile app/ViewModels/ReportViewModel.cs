using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;
using System.Collections.ObjectModel;
using Microsoft.Maui.ApplicationModel; // For PhoneDialer
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Media;

namespace mobile_app.ViewModels;

public partial class ReportViewModel : ObservableObject
{
    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ImageSource? evidenceImage;

    [ObservableProperty]
    private string locationLabel = "Tap map to pin location";

    private FileResult? _photoFile;
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    // Command to Pick Photo (Requirement E4)
    [RelayCommand]
    private async Task CallSecurity()
    {
        if (PhoneDialer.Default.IsSupported)
        {
            // Example UTS Security Number
            PhoneDialer.Default.Open("084367300");
        }
        else
        {
            await Shell.Current.DisplayAlert("Error", "Phone dialer not supported.", "OK");
        }
    }

    // Command to Submit Report
    [RelayCommand]
    private async Task SubmitReport()
    {
        if (string.IsNullOrEmpty(Description))
        {
            await Shell.Current.DisplayAlert("Error", "Please describe the incident.", "OK");
            return;
        }

        // Logic to send data to your Backend would go here
        // For now, we simulate a submission
        await Shell.Current.DisplayAlert("Report Sent", $"Incident at {Latitude}, {Longitude} reported to UTS Security.", "OK");

        // Reset form
        Description = string.Empty;
        EvidenceImage = null;
        LocationLabel = "Tap map to pin location";
    }
}