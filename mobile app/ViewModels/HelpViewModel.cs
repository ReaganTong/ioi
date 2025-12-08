using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using mobile_app.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using Microsoft.Maui.Media; // Required for Email
using System.Linq; // Required for .Where and .Any()

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
                Action = "Contact"
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

        try
        {
            if (resource.Action == "Call")
            {
                // Opens the dialer and pre-fills the number, waiting for the user to call.
                // This replaces the old PhoneDialer.Default.IsSupported check that was causing the "Not Supported" alert.
                PhoneDialer.Default.Open(resource.ContactInfo);
            }
            else if (resource.Action == "Contact")
            {
                // FIX: Changed IsSupported to IsComposeSupported to resolve CS1061 error.
                if (Email.Default.IsComposeSupported)
                {
                    // Assuming emails are separated by '/'
                    var emails = resource.ContactInfo.Split(new[] { '/', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                                     .Where(e => e.Contains('@'))
                                                     .ToList();

                    if (emails.Any())
                    {
                        var message = new EmailMessage
                        {
                            To = emails,
                            Subject = "Counselling Request from Anti-Bully App User"
                        };
                        await Email.Default.ComposeAsync(message);
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "No valid email address found.", "OK");
                    }
                }
                else
                {
                    // Fallback for unsupported email client
                    await Shell.Current.DisplayAlert("Not Supported", $"Please manually contact: {resource.ContactInfo}", "OK");
                }
            }
        }
        catch (Exception)
        {
            // This catches exceptions thrown when an API is called on an unsupported platform (e.g., desktop/emulator).
            // We now use Launcher.OpenAsync as a URI fallback.
            try
            {
                if (resource.Action == "Call")
                {
                    // Robust URI fallback for 'Call' (tel: scheme)
                    await Launcher.Default.OpenAsync($"tel:{resource.ContactInfo}");
                }
                else if (resource.Action == "Contact")
                {
                    // Robust URI fallback for 'Contact' (mailto: scheme)
                    await Launcher.Default.OpenAsync($"mailto:{resource.ContactInfo}");
                }
            }
            catch
            {
                // Final fallback: show the contact info for manual entry.
                await Shell.Current.DisplayAlert("Action Failed",
                    $"Could not perform action. Please manually use: {resource.ContactInfo}", "OK");
            }
        }
    }
}