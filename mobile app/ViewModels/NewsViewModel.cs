using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;
using Plugin.LocalNotification;
using System.Collections.ObjectModel;

namespace mobile_app.ViewModels;

public partial class NewsViewModel : ObservableObject
{
    public ObservableCollection<NewsItem> NewsList { get; set; } = new();

    public NewsViewModel()
    {
        // Initial Data
        NewsList.Add(new NewsItem { Title = "Welcome", Description = "Stay tuned for updates.", Date = DateTime.Now.ToString("MMM dd"), Color = "#E0E0E0" });
    }

    [RelayCommand]
    private async Task SimulateNotification()
    {
        // 1. Create fake "new" data
        var newItem = new NewsItem
        {
            Title = "🔴 BULLYING ALERT",
            Description = "Reported incident at Block A. Security deployed.",
            Date = DateTime.Now.ToString("hh:mm tt"),
            Color = "#FFCDD2" // Red tint for alert
        };

        // 2. Add to UI
        NewsList.Insert(0, newItem);

        // 3. Fire System Notification (Requirement E6)
        var request = new NotificationRequest
        {
            NotificationId = 100,
            Title = newItem.Title,
            Description = newItem.Description,
            BadgeNumber = 1,
            Schedule = { NotifyTime = DateTime.Now.AddSeconds(1) }
        };
        await LocalNotificationCenter.Current.Show(request);
    }
}