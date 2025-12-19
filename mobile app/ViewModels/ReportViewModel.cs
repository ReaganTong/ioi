using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using mobile_app.Models;
using mobile_app.Services; // Required for IVideoThumbnailService
using Supabase;
using System.Collections.ObjectModel;
using System.Diagnostics;

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

    [ObservableProperty]
    private bool isRefreshing;

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
            // This will pop up a window with the EXACT error details
            await Shell.Current.DisplayAlert("Diagnostic Error",
                $"Message: {ex.Message}\n\nInner: {ex.InnerException?.Message}",
                "OK");

            // Also check your "Output" window in Visual Studio for this line:
            System.Diagnostics.Debug.WriteLine($"SUPABASE_ERROR: {ex}");
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

        IsRefreshing = true; // Show loading state if you have one
        try
        {
            List<string> uploadedUrls = new List<string>();

            foreach (var item in Attachments)
            {
                // Use the file path directly for better performance
                var fileName = $"{Guid.NewGuid()}{(item.IsVideo ? ".mp4" : ".jpg")}";

                // Open the file stream
                using var stream = await item.File.OpenReadAsync();

                // For Supabase C#, it's safer to convert to byte[] only if necessary, 
                // but let's add a timeout check here.
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                // UPLOAD
                // Ensure the bucket "evidence" exists in Supabase Storage!
                await _supabase.Storage.From("evidence").Upload(fileBytes, fileName);

                // GET URL
                var url = _supabase.Storage.From("evidence").GetPublicUrl(fileName);
                uploadedUrls.Add(url);
            }

            string finalUrlString = string.Join(",", uploadedUrls);

            var newReport = new ReportModel
            {
                Description = this.Description,
                Location = $"{Latitude},{Longitude}",
                StudentId = "12345678", // Replace with real Student ID if available
                ImageUrl = finalUrlString,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending" // Make sure your DB column allows this
            };

            // SAVE TO TABLE
            await _supabase.From<ReportModel>().Insert(newReport);

            await Shell.Current.DisplayAlert("Success", "Report sent successfully!", "OK");

            // RESET UI
            Description = string.Empty;
            Attachments.Clear();
            Latitude = 0;
            Longitude = 0;
            LocationLabel = "No location set";
        }
        catch (Exception ex)
        {
            // This will now give you more detail (e.g. if the bucket is missing)
            Debug.WriteLine($"DEBUG: {ex.Message}");
            await Shell.Current.DisplayAlert("Connection Error",
                "Make sure you have internet and the 'evidence' storage bucket is created in Supabase.", "OK");
        }
        finally
        {
            IsRefreshing = false;
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