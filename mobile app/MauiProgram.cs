using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm;
using mobile_app.ViewModels;
using mobile_app.Views;
using Plugin.LocalNotification;

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

        // FIX: Configure Android WebView to allow NASA Worldview to load
#if ANDROID
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("EnhancedWebView", (handler, view) =>
        {
            if (handler.PlatformView != null)
            {
                // Enable JavaScript and DOM Storage (Critical for modern maps)
                handler.PlatformView.Settings.JavaScriptEnabled = true;
                handler.PlatformView.Settings.DomStorageEnabled = true;
                handler.PlatformView.Settings.AllowFileAccess = true;
                
                // Allow "Mixed Content" (HTTP resources on an HTTPS site) to prevent blank maps
                handler.PlatformView.Settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
                
                // Allow WebGL to render properly
                handler.PlatformView.Settings.MediaPlaybackRequiresUserGesture = false;
            }
        });
#endif

        // Register Services
        builder.Services.AddSingleton<QuizViewModel>();
        builder.Services.AddSingleton<LessonViewModel>();
        builder.Services.AddSingleton<HelpViewModel>();
        builder.Services.AddSingleton<ReportViewModel>();

        builder.Services.AddTransient<Views.QuizPlayPage>();
        builder.Services.AddTransient<NewsViewModel>();
        builder.Services.AddTransient<Views.NewsPage>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<QuizPage>();
        builder.Services.AddTransient<MorePage>();
        builder.Services.AddTransient<LessonsPage>();
        builder.Services.AddTransient<HelpPage>();
        builder.Services.AddTransient<LessonDetailPage>();
        builder.Services.AddTransient<ReportPage>();

        builder.Services.AddSingleton<Services.LocalDbService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}