namespace mobile_app;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // ✅ This ensures AppShell loads as your root
        return new Window(new Views.LoginPage());
    }
}
