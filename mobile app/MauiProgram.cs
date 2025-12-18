using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm;
using mobile_app.ViewModels;
using mobile_app.Views;
using Plugin.LocalNotification;
using Supabase; // Add this

#if ANDROID
using Android.Views;
#endif

namespace mobile_app;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // FIX: Initialize Supabase
        var url = "YOUR_SUPABASE_URL";
        var key = "YOUR_SUPABASE_ANON_KEY";

        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true
        };

        var supabase = new Client(url, key, options);
        builder.Services.AddSingleton(provider => supabase);

        // ... (Keep your existing Android WebView handler code here) ...

        // Register ViewModels
        builder.Services.AddSingleton<ReportViewModel>();
        builder.Services.AddSingleton<ProfileViewModel>();
        // ... (Keep other services) ...

        return builder.Build();
    }
}