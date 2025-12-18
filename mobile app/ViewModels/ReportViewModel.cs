using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;
using mobile_app.Services; // Required for IVideoThumbnailService
using Supabase;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.ApplicationModel;
using System.Collections.ObjectModel;

namespace mobile_app.ViewModels;

public partial class ReportViewModel : ObservableObject
{
    private readonly Client _supabase;
    private readonly IVideoThumbnailService _thumbnailService;

    // EVENT: For the Map in the View to listen to
    public event Action<double, double>? RequestSetLocation;

    public ReportViewModel(Client supabase, IVideoThumbnailService thumbnailService)
    {
        _supabase = supabase;
        _thumbnailService = thumbnailService;
        Attachments = new ObservableCollection<MediaAttachment>();
    }

    [ObservableProperty]
    private string description = string.Empty;

    // List to hold multiple photos/videos
    public ObservableCollection<MediaAttachment> Attachments { get; }

    [ObservableProperty]
    private string locationLabel = "No location set";

    [ObservableProperty]
    private double latitude;

    [ObservableProperty]
    private double longitude;

    // --- MEDIA LOGIC (Photos & Videos) ---

    [RelayCommand]
    private async Task PickMedia()
    {
        try
        {
            string action = await Shell.Current.DisplayActionSheet("Attach Media", "Cancel", null, "Pick Photo", "Pick Video", "Take Photo", "Take Video");

            FileResult? result = null;
            bool isVideo = false;

            if (action == "Pick Photo")
                result = await MediaPicker.Default.PickPhotoAsync();
            else if (action == "Pick Video")
            {
                result = await MediaPicker.Default.PickVideoAsync();
                isVideo = true;
            }
            else if (action == "Take Photo" && MediaPicker.Default.IsCaptureSupported)
                result = await MediaPicker.Default.CapturePhotoAsync();
            else if (action == "Take Video" && MediaPicker.Default.IsCaptureSupported)
            {
                result = await MediaPicker.Default.CaptureVideoAsync();
                isVideo = true;
            }

            if (result != null)
            {
                ImageSource? previewSource = null;

                if (isVideo)
                {
                    // 1. Try to generate a real thumbnail
                    previewSource = await _thumbnailService.GetThumbnailAsync(result.FullPath);

                    // 2. Fallback
                    if (previewSource == null) previewSource = ImageSource.FromFile("dotnet_bot.png");
                }
                else
                {
                    // It's a photo, just show it
                    previewSource = ImageSource.FromStream(async (ct) => await result.OpenReadAsync());
                }

                var attachment = new MediaAttachment
                {
                    File = result,
                    IsVideo = isVideo,
                    PreviewSource = previewSource
                };

                Attachments.Add(attachment);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task PreviewAttachment(MediaAttachment attachment)
    {
        try
        {
            if (attachment == null || attachment.File == null) return;
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                Title = "Preview Evidence",
                File = new ReadOnlyFile(attachment.File.FullPath)
            });
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Could not open file: " + ex.Message, "OK");
        }
    }

    [RelayCommand]
    private void RemoveAttachment(MediaAttachment item)
    {
        if (Attachments.Contains(item)) Attachments.Remove(item);
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
            List<string> uploadedUrls = new List<string>();

            foreach (var item in Attachments)
            {
                using var stream = await item.File.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                string ext = item.IsVideo ? ".mp4" : ".jpg";
                var fileName = $"{Guid.NewGuid()}{ext}";

                await _supabase.Storage.From("evidence").Upload(fileBytes, fileName);
                var url = _supabase.Storage.From("evidence").GetPublicUrl(fileName);
                uploadedUrls.Add(url);
            }

            string finalUrlString = string.Join(",", uploadedUrls);

            var newReport = new ReportModel
            {
                Description = this.Description,
                Location = $"{Latitude},{Longitude}",
                StudentId = "12345678",
                ImageUrl = finalUrlString,
                CreatedAt = DateTime.UtcNow
            };

            await _supabase.From<ReportModel>().Insert(newReport);
            await Shell.Current.DisplayAlert("Success", "Report sent!", "OK");

            Description = string.Empty;
            Attachments.Clear();
            LocationLabel = "No location set";
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to send: {ex.Message}", "OK");
        }
    }

    // --- LOCATION & SECURITY ---

    [RelayCommand]
    private async Task GetCurrentLocation()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) return;
            }

            var location = await Geolocation.Default.GetLastKnownLocationAsync();
            if (location != null) UpdateLocation(location);

            var request = new GeolocationRequest(GeolocationAccuracy.Default, TimeSpan.FromSeconds(10));
            var freshLocation = await Geolocation.Default.GetLocationAsync(request);

            if (freshLocation != null) UpdateLocation(freshLocation);
        }
        catch { }
    }

    private void UpdateLocation(Location loc)
    {
        Latitude = loc.Latitude;
        Longitude = loc.Longitude;
        LocationLabel = $"📍 {Latitude:F4}, {Longitude:F4}";
        RequestSetLocation?.Invoke(Latitude, Longitude);
    }

    [RelayCommand]
    private async Task CallSecurity()
    {
        if (PhoneDialer.Default.IsSupported) PhoneDialer.Default.Open("082-260991");
    }
}