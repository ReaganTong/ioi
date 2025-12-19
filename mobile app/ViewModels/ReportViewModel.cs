using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using mobile_app.Models;
using mobile_app.Services;
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

    public ObservableCollection<MediaAttachment> Attachments { get; }

    [ObservableProperty]
    private string locationLabel = "No location set";

    [ObservableProperty]
    private double latitude;

    [ObservableProperty]
    private double longitude;

    [ObservableProperty]
    private bool isRefreshing;

    // --- MEDIA LOGIC ---

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
                    previewSource = await _thumbnailService.GetThumbnailAsync(result.FullPath);
                    if (previewSource == null) previewSource = ImageSource.FromFile("dotnet_bot.png");
                }
                else
                {
                    previewSource = ImageSource.FromStream(async (ct) => await result.OpenReadAsync());
                }

                Attachments.Add(new MediaAttachment
                {
                    File = result,
                    IsVideo = isVideo,
                    PreviewSource = previewSource
                });
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
            if (attachment?.File == null) return;
            await Launcher.Default.OpenAsync(new OpenFileRequest
            {
                Title = "Preview",
                File = new ReadOnlyFile(attachment.File.FullPath)
            });
        }
        catch { }
    }

    [RelayCommand]
    private void RemoveAttachment(MediaAttachment item)
    {
        if (Attachments.Contains(item)) Attachments.Remove(item);
    }

    // --- SUBMISSION LOGIC (FIXED) ---

    [RelayCommand]
    private async Task SubmitReport()
    {
        if (string.IsNullOrEmpty(Description))
        {
            await Shell.Current.DisplayAlert("Required", "Please describe the incident.", "OK");
            return;
        }

        IsRefreshing = true;
        try
        {
            // 1. GET CURRENT USER EMAIL
            var currentUser = _supabase.Auth.CurrentUser;
            string studentEmail = currentUser?.Email ?? "Anonymous"; // If not logged in (shouldn't happen), mark Anonymous

            List<string> uploadedUrls = new List<string>();

            // 2. Upload Attachments
            foreach (var item in Attachments)
            {
                var fileName = $"{Guid.NewGuid()}{(item.IsVideo ? ".mp4" : ".jpg")}";
                using var stream = await item.File.OpenReadAsync();
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await stream.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                await _supabase.Storage.From("evidence").Upload(fileBytes, fileName);
                var url = _supabase.Storage.From("evidence").GetPublicUrl(fileName);
                uploadedUrls.Add(url);
            }

            string finalUrlString = string.Join(",", uploadedUrls);

            // 3. Create Report Object
            var newReport = new ReportModel
            {
                Description = this.Description,
                Location = $"{Latitude},{Longitude}",
                StudentId = studentEmail, // <--- FIXED: Now uses real email!
                ImageUrl = finalUrlString,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending"
            };

            // 4. Save to Database
            await _supabase.From<ReportModel>().Insert(newReport);

            await Shell.Current.DisplayAlert("Success", "Report sent successfully!", "OK");

            // Reset UI
            Description = string.Empty;
            Attachments.Clear();
            Latitude = 0;
            Longitude = 0;
            LocationLabel = "No location set";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"DEBUG: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "Could not submit report. Check internet connection.", "OK");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    // --- LOCATION LOGIC ---

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