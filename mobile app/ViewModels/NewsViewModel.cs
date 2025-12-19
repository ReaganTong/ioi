using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;
using Supabase.Realtime;
using Supabase.Realtime.PostgresChanges;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace mobile_app.ViewModels;

public partial class NewsViewModel : ObservableObject
{
    // FIX: Use Supabase.Client explicitly to avoid ambiguity with Realtime.Client
    private readonly Supabase.Client _supabase;
    public ObservableCollection<NewsModel> NewsList { get; set; } = new();

    public NewsViewModel(Supabase.Client supabase)
    {
        _supabase = supabase;

        // Load initial data
        Task.Run(LoadNewsAsync);

        // Start listening
        SubscribeToRealtimeUpdates();
    }

    public async Task LoadNewsAsync()
    {
        try
        {
            var result = await _supabase
                .From<NewsModel>()
                .Order("created_at", Supabase.Postgrest.Constants.Ordering.Descending)
                .Get();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                NewsList.Clear();
                foreach (var item in result.Models)
                {
                    NewsList.Add(item);
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading news: {ex.Message}");
        }
    }

    private async void SubscribeToRealtimeUpdates()
    {
        try
        {
            var channel = _supabase.Realtime.Channel("news-updates");

            var changesOptions = new PostgresChangesOptions(
                "public",
                "news",
                PostgresChangesOptions.ListenType.All
            );

            channel.Register(changesOptions);

            // Single consolidated handler
            channel.AddPostgresChangeHandler(PostgresChangesOptions.ListenType.All, async (sender, change) =>
            {
                Debug.WriteLine($"Realtime Change Detected: {change.Event}");

                // Ensure UI updates on the Main Thread
                MainThread.BeginInvokeOnMainThread(async () => {
                    await LoadNewsAsync();
                    await ShowSystemNotification("New Campus Alert!", "Check the awareness feed for updates.");
                });
            });

            await channel.Subscribe();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Realtime Subscription Error: {ex.Message}");
        }
    }

    private async Task ShowSystemNotification(string title, string desc)
    {
        // Ensure the app has permission from the user
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        var request = new NotificationRequest
        {
            NotificationId = 101,
            Title = title,
            Description = desc,
            BadgeNumber = 1,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(1) // Immediate delivery
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await LoadNewsAsync();
    }
}