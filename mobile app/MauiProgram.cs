using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm;
using mobile_app.ViewModels;
using mobile_app.Views;
using Plugin.LocalNotification;

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

#if ANDROID
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("EnhancedWebView", (handler, view) =>
        {
            if (handler.PlatformView != null)
            {
                handler.PlatformView.Settings.JavaScriptEnabled = true;
                handler.PlatformView.Settings.DomStorageEnabled = true;
                handler.PlatformView.Settings.AllowFileAccess = true;
                handler.PlatformView.Settings.MixedContentMode = Android.Webkit.MixedContentHandling.AlwaysAllow;
                handler.PlatformView.Settings.MediaPlaybackRequiresUserGesture = false;

                handler.PlatformView.Touch += (sender, e) =>
                {
                    if (e.Event.Action == MotionEventActions.Down)
                    {
                        handler.PlatformView.Parent?.RequestDisallowInterceptTouchEvent(true);
                    }
                    else if (e.Event.Action == MotionEventActions.Up || e.Event.Action == MotionEventActions.Cancel)
                    {
                        handler.PlatformView.Parent?.RequestDisallowInterceptTouchEvent(false);
                    }
                    e.Handled = false;
                };
            }
        });
#endif

        // Register ViewModels
        builder.Services.AddSingleton<QuizViewModel>();
        builder.Services.AddSingleton<LessonViewModel>();
        builder.Services.AddSingleton<HelpViewModel>();
        builder.Services.AddSingleton<ReportViewModel>();
        builder.Services.AddSingleton<ProfileViewModel>(); // NEW

        // Register Pages
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
        builder.Services.AddTransient<ProfilePage>(); // NEW

        builder.Services.AddSingleton<Services.LocalDbService>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}