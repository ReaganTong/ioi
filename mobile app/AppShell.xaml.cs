namespace mobile_app;
using mobile_app.Views;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // FIX: Register deep-link routes using C#
        Routing.RegisterRoute("LessonDetail", typeof(Views.LessonDetailPage));
        Routing.RegisterRoute("QuizPlay", typeof(Views.QuizPlayPage));
        Routing.RegisterRoute("NewsPage", typeof(Views.NewsPage));
        Routing.RegisterRoute(nameof(QuizPlayPage), typeof(QuizPlayPage));
    }
}