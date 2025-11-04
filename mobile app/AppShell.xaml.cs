namespace mobile_app;

// Must have the 'partial' keyword to link to the XAML (already fixed in previous step)
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // FIX: Register deep-link routes using C# to fix the XAML errors
        // Register LessonDetailPage for navigation from LessonsPage
        Routing.RegisterRoute("LessonDetail", typeof(Views.LessonDetailPage));

        // Optionally, register other views that might be navigated to later, if needed
        // Routing.RegisterRoute("QuizDetail", typeof(Views.QuizDetailPage));
    }
}