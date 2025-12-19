using Microsoft.Extensions.Logging;
using mobile_app.Views;
using mobile_app.ViewModels;
using Supabase;

namespace mobile_app;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // FIX: This MUST be inside #if ANDROID. 
        // If you put it in #if DEBUG, iOS and Windows try to read it and crash.
#if ANDROID
        builder.Services.AddSingleton<mobile_app.Services.IVideoThumbnailService, mobile_app.Platforms.Android.VideoThumbnailService>();
#endif

        // =========================================================
        // 1. REGISTER SUPABASE
        // =========================================================
        var supabaseUrl = "https://yacthiglkcipbltixron.supabase.co"; // Don't forget to put your real keys back!
        var supabaseKey = "sb_publishable_thHRqeO7TmSOhuWj9TgB4A_cAw4BwXB";

        var options = new Supabase.SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = true,
        };

        builder.Services.AddSingleton(provider =>
    new Supabase.Client(supabaseUrl, supabaseKey, options));

        // =========================================================
        // 2. REGISTER VIEWMODELS
        // =========================================================
        builder.Services.AddTransient<ReportViewModel>();
        builder.Services.AddTransient<HelpViewModel>();
        builder.Services.AddTransient<LessonViewModel>();
        builder.Services.AddTransient<QuizViewModel>();
        builder.Services.AddTransient<AdminViewModel>();
       

        // =========================================================
        // 3. REGISTER PAGES (VIEWS)
        // =========================================================
        builder.Services.AddTransient<ReportPage>();
        builder.Services.AddTransient<HelpPage>();
        builder.Services.AddTransient<LessonsPage>();
        builder.Services.AddTransient<QuizPlayPage>();
        builder.Services.AddTransient<AdminPage>();

        return builder.Build();
    }
}