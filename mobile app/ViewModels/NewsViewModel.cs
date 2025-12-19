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
            // 1. Create the channel
            var channel = _supabase.Realtime.Channel("news-updates");

            // 2. Define the listener options (Schema: public, Table: news)
            // Note: The enum is now 'ListenType'
            var changesOptions = new PostgresChangesOptions(
                "public",
                "news",
                PostgresChangesOptions.ListenType.All
            );

            // 3. Attach the options to the channel
            channel.Register(changesOptions);

            // Inside SubscribeToRealtimeUpdates...
            channel.AddPostgresChangeHandler(PostgresChangesOptions.ListenType.All, async (sender, change) =>
            {
                Debug.WriteLine($"Realtime Change: {change.Event}");

                // Use 'await' to ensure the notification logic runs
                await LoadNewsAsync();
                await ShowSystemNotification("New Campus Alert!", "Check the awareness feed for updates.");
            });
            // 4. Add the event handler using the correct method
            // This replaces "channel.PostgresChanges += ..."
            channel.AddPostgresChangeHandler(PostgresChangesOptions.ListenType.All, (sender, change) =>
            {
                // 'change' contains the event data (Insert, Update, Delete)
                Debug.WriteLine($"Realtime Change: {change.Event}");

                // Execute reload on the UI thread or background as needed
                _ = LoadNewsAsync();

                ShowSystemNotification("New Campus Alert!", "Check the awareness feed for updates.");
            });

            // 5. Subscribe to start listening
            await channel.Subscribe();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Realtime Subscription Error: {ex.Message}");
        }
    }

    private async Task ShowSystemNotification(string title, string desc)
    {
        // 1. Check if we have permission
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            // 2. Request permission if we don't have it
            var granted = await LocalNotificationCenter.Current.RequestNotificationPermission();
            if (!granted) return; // Exit if user says no
        }

        var request = new NotificationRequest
        {
            NotificationId = 101,
            Title = title,
            Description = desc,
            BadgeNumber = 1,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Now.AddSeconds(1) // Tiny delay ensures system is ready
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