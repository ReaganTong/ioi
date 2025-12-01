using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection; // ADDED
using mobile_app.Views; // ADDED
using mobile_app.ViewModels; // ADDED
using CommunityToolkit.Mvvm; // ADDED (ensure csproj is updated)

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

        // 🌟 Dependency Injection Registration
        // ViewModels (Singleton lifespan for data across the app)
        builder.Services.AddSingleton<QuizViewModel>();
        builder.Services.AddSingleton<LessonViewModel>();
        builder.Services.AddSingleton<HelpViewModel>();

        // Pages/Views (Transient lifespan, especially for detailed views)
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<QuizPage>();
        builder.Services.AddTransient<MorePage>();
        builder.Services.AddTransient<LessonsPage>();
        builder.Services.AddTransient<HelpPage>();
        builder.Services.AddTransient<LessonDetailPage>();

        // Add this line inside the CreateMauiApp method
        builder.Services.AddTransient<ReportPage>();
        builder.Services.AddSingleton<ReportViewModel>();
        builder.UseMauiMaps(); // <--- CRITICAL for the map to work

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}