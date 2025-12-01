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
                ContactInfo = "084-367300", // Example UTS number
                Icon = "👮",
                AccentColor = "#F44336", // Red for emergency
                Action = "Call"
            },
            // 2. UTS Counselling
            new HelpResource
            {
                Name = "UTS Counselling Unit",
                Description = "Student Development & Services Department (SDSD).",
                ContactInfo = "counselling@uts.edu.my",
                Icon = "🧠",
                AccentColor = "#4CAF50", // Green for support
                Action = "Web" // Or Call
            },
            // 3. External Help
            new HelpResource
            {
                Name = "Befrienders KL",
                Description = "Emotional support & suicide prevention.",
                ContactInfo = "03-76272929",
                Icon = "🤝",
                AccentColor = "#2196F3",
                Action = "Call"
            }
        };
    }

    [RelayCommand]
    private static async Task PerformAction(HelpResource resource)
    {
        if (resource.Action == "Call")
        {
            if (PhoneDialer.Default.IsSupported)
            {
                PhoneDialer.Default.Open(resource.ContactInfo);
            }
            else
            {
                await Shell.Current.DisplayAlert("Feature Not Supported", $"Please manually call: {resource.ContactInfo}", "OK");
            }
        }
        else if (resource.Action == "Web")
        {
            if (await Launcher.Default.CanOpenAsync(resource.ContactInfo!))
            {
                await Launcher.Default.OpenAsync(resource.ContactInfo!);
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", $"Could not open the link: {resource.ContactInfo}", "OK");
            }
        }
    }
}