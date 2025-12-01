namespace mobile_app;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // FIX: Register deep-link routes using C#
        Routing.RegisterRoute("LessonDetail", typeof(Views.LessonDetailPage));
        Routing.RegisterRoute("QuizPlay", typeof(Views.QuizPlayPage));
    }
}