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
            new HelpResource
            {
                Name = "Talian Kasih (KPWKM)",
                Description = "National hotline for child, women, and domestic abuse victims in Malaysia.",
                ContactInfo = "15999",
                Icon = "📞",
                AccentColor = "#F44336",
                Action = "Call"
            },
            new HelpResource
            {
                Name = "Befrienders KL",
                Description = "24-hour emotional support and suicide prevention hotline.",
                ContactInfo = "03-76272929",
                Icon = "🤝",
                AccentColor = "#2196F3",
                Action = "Call"
            },
            new HelpResource
            {
                Name = "MCMC (Cyber999)",
                Description = "Report cyberbullying, internet fraud, and cyber security incidents to CyberSecurity Malaysia.",
                ContactInfo = "https://www.cyber999.my/",
                Icon = "🛡️",
                AccentColor = "#FF9800",
                Action = "Web"
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