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

        // =========================================================
        // 1. REGISTER SUPABASE
        // =========================================================
        var supabaseUrl = "https://your-project-id.supabase.co";
        var supabaseKey = "your-public-anon-key";

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
        builder.Services.AddTransient<QuizViewModel>(); // <--- Added this

        // =========================================================
        // 3. REGISTER PAGES (VIEWS)
        // =========================================================
        builder.Services.AddTransient<ReportPage>();
        builder.Services.AddTransient<HelpPage>();
        builder.Services.AddTransient<LessonsPage>();
        builder.Services.AddTransient<QuizPlayPage>();  // <--- Added this

        return builder.Build();
    }
}