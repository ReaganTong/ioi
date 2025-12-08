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
            // .UseSkiaSharp() <-- REMOVED this line because you uninstalled the package
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
        builder.Services.AddTransient<Views.QuizPlayPage>();

        // Register News Service
        builder.Services.AddTransient<NewsViewModel>();
        builder.Services.AddTransient<Views.NewsPage>();

        // Register Views
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