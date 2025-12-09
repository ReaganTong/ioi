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

        // FIX: Advanced WebView Settings for Android
        // 1. Enable DOM Storage (Critical for maps)
        // 2. Enable Mixed Content (Critical for loading all map tiles)
#if ANDROID
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("EnhancedWebView", (handler, view) =>
        {
            if (handler.PlatformView != null)
            {
                handler.PlatformView.Settings.JavaScriptEnabled = true;
                handler.PlatformView.Settings.DomStorageEnabled = true;
                handler.PlatformView.Settings.AllowFileAccess = true;
                
                // FIXED: The correct enum is 'MixedContentHandling', not 'MixedContentMode'
                handler.PlatformView.Settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
                
                // Ensures 3D content (WebGL) renders correctly
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