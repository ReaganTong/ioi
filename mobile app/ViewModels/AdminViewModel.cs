using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;
using Supabase;
using System.Collections.ObjectModel;

namespace mobile_app.ViewModels;

public partial class AdminViewModel : ObservableObject
{
    private readonly Client _supabase;

    public AdminViewModel(Client supabase)
    {
        _supabase = supabase;
        Reports = new ObservableCollection<ReportModel>();
        LoadReports();
    }

    // --- REPORT SECTION ---
    public ObservableCollection<ReportModel> Reports { get; }

    [ObservableProperty]
    private bool isRefreshing;

    [RelayCommand]
    private async Task LoadReports()
    {
        IsRefreshing = true;
        try
        {
            var response = await _supabase.From<ReportModel>()
                                          .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                                          .Get();

            Reports.Clear();
            foreach (var report in response.Models)
            {
                Reports.Add(report);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Failed to load reports: " + ex.Message, "OK");
        }
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task MarkResolved(ReportModel report)
    {
        try
        {
            report.Status = "Resolved";
            await _supabase.From<ReportModel>().Update(report);
            await LoadReports(); // Refresh list
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Update failed: " + ex.Message, "OK");
        }
    }

    // --- NEWS UPLOAD SECTION ---
    [ObservableProperty]
    private string newsTitle;

    [ObservableProperty]
    private string newsDescription;

    [RelayCommand]
    private async Task UploadNews()
    {
        if (string.IsNullOrWhiteSpace(NewsTitle) || string.IsNullOrWhiteSpace(NewsDescription))
        {
            await Shell.Current.DisplayAlert("Error", "Please fill in all fields", "OK");
            return;
        }

        try
        {
            var news = new NewsModel
            {
                Title = NewsTitle,
                Description = NewsDescription,
                CreatedAt = DateTime.UtcNow
            };

            await _supabase.From<NewsModel>().Insert(news);

            await Shell.Current.DisplayAlert("Success", "News Posted!", "OK");
            NewsTitle = string.Empty;
            NewsDescription = string.Empty;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", "Upload failed: " + ex.Message, "OK");
        }
    }
}