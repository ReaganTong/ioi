using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm;
using mobile_app.ViewModels;
using mobile_app.Views;
using Plugin.LocalNotification;

#if ANDROID
using Android.Views; // Required for touch events
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

        // FIX: Advanced WebView Settings + Touch Handling
#if ANDROID
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("EnhancedWebView", (handler, view) =>
        {
            if (handler.PlatformView != null)
            {
                // 1. Enable Settings for Map
                handler.PlatformView.Settings.JavaScriptEnabled = true;
                handler.PlatformView.Settings.DomStorageEnabled = true;
                handler.PlatformView.Settings.AllowFileAccess = true;
                handler.PlatformView.Settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
                handler.PlatformView.Settings.MediaPlaybackRequiresUserGesture = false;

                // 2. FIX: Stop the Page from Scrolling when touching the Map
                handler.PlatformView.Touch += (sender, e) =>
                {
                    // If the user puts their finger DOWN on the map...
                    if (e.Event.Action == MotionEventActions.Down)
                    {
                        // Tell the parent (ScrollView) to STOP intercepting touches
                        handler.PlatformView.Parent?.RequestDisallowInterceptTouchEvent(true);
                    }
                    // If the user LIFTS their finger or cancels...
                    else if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel)
                    {
                        // Allow the page to scroll again
                        handler.PlatformView.Parent?.RequestDisallowInterceptTouchEvent(false);
                    }

                    // IMPORTANT: Return 'false' so the Map still gets the touch event (to pan/zoom)
                    e.Handled = false;
                };
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