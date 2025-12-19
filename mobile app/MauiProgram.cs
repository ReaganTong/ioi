using Microsoft.Extensions.Logging;
using mobile_app.Views;
using mobile_app.ViewModels;
using Plugin.LocalNotification;
using mobile_app.Services;
using Supabase;

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

#if DEBUG
        builder.Logging.AddDebug();
#endif
#if ANDROID
        builder.Services.AddSingleton<IVideoThumbnailService, mobile_app.Platforms.Android.VideoThumbnailService>();
#endif

        // --- SUPABASE SETUP ---
        var supabaseUrl = "https://yacthiglkcipbltixron.supabase.co";
        var supabaseKey = "sb_publishable_thHRqeO7TmSOhuWj9TgB4A_cAw4BwXB";

        var options = new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true,
            SessionHandler = new mobile_app.Services.MauiSessionHandler() // <--- CONNECT THE HANDLER HERE
        };

        builder.Services.AddSingleton(provider => new Supabase.Client(supabaseUrl, supabaseKey, options));

        // --- REGISTRATIONS ---
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ReportViewModel>();
        builder.Services.AddTransient<HelpViewModel>();
        builder.Services.AddTransient<LessonViewModel>();
        builder.Services.AddTransient<QuizViewModel>();
        builder.Services.AddTransient<AdminViewModel>();
        builder.Services.AddTransient<NewsViewModel>();
        builder.Services.AddTransient<NewsPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<ReportPage>();
        builder.Services.AddTransient<HelpPage>();
        builder.Services.AddTransient<LessonsPage>();
        builder.Services.AddTransient<QuizPlayPage>();
        builder.Services.AddTransient<AdminPage>();

        return builder.Build();
    }
}