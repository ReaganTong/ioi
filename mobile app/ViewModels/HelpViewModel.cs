using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace mobile_app.ViewModels;

public partial class HelpViewModel : ObservableObject
{
    public ObservableCollection<HelpResource> Hotlines { get; set; }

    public HelpViewModel()
    {
        Hotlines = new ObservableCollection<HelpResource>
        {
            // 1. UTS Security (Primary Contact)
            new HelpResource
            {
                Name = "UTS Campus Security",
                Description = "24/7 Emergency response within campus grounds.",
                ContactInfo = "082-260991", // Example UTS number
                Icon = "👮",
                AccentColor = "#F44336", // Red for emergency
                Action = "Call"
            },
            // 2. UTS Counselling
            new HelpResource
            {
                Name = "UTS Counselling Unit",
                Description = "Mdm Hamidah / Mr Kevin.",
                ContactInfo = "hamidahrapee@uts.edu.my / kevin@uts.edu.my",
                Icon = "🧠",
                AccentColor = "#4CAF50", // Green for support
                Action = "Contact" // Or Call
            },
            // 3. External Help
            new HelpResource
            {
                Name = "Befrienders Kuching",
                Description = "sam@befrienderskch.org.my",
                ContactInfo = "082-242 800",
                Icon = "🤝",
                AccentColor = "#2196F3",
                Action = "Call"
            }
        };
    }

    [RelayCommand]
    private static async Task PerformAction(HelpResource resource)
    {
        // Fix: Safety check for null contact info
        if (string.IsNullOrWhiteSpace(resource.ContactInfo))
        {
            await Shell.Current.DisplayAlert("Error", "No contact info available.", "OK");
            return;
        }

        if (resource.Action == "Call")
        {
            if (PhoneDialer.Default.IsSupported)
            {
                // Now safe to use because we checked for null above
                PhoneDialer.Default.Open(resource.ContactInfo);
            }
            else
            {
                await Shell.Current.DisplayAlert("Not Supported", $"Manual call: {resource.ContactInfo}", "OK");
            }
        }
    }
}