using Microsoft.Extensions.Logging;
using CommunityToolkit.Mvvm;
using SkiaSharp.Views.Maui.Controls.Hosting; // 👈 CRITICAL: Required for Mapsui v5
using mobile_app.ViewModels;
using mobile_app.Views;

namespace mobile_app;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseSkiaSharp() // 👈 FIX: Changed from .UseMapsui() to .UseSkiaSharp()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register Services
        builder.Services.AddSingleton<QuizViewModel>();
        builder.Services.AddSingleton<LessonViewModel>();
        builder.Services.AddSingleton<HelpViewModel>();
        builder.Services.AddSingleton<ReportViewModel>();

        // Register Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<QuizPage>();
        builder.Services.AddTransient<MorePage>();
        builder.Services.AddTransient<LessonsPage>();
        builder.Services.AddTransient<HelpPage>();
        builder.Services.AddTransient<LessonDetailPage>();
        builder.Services.AddTransient<ReportPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}