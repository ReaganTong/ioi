using mobile_app.Views;
using Supabase.Gotrue; // Needed for Session
using Newtonsoft.Json; // Needed to read the file manually
using System.Diagnostics;

namespace mobile_app;

public partial class App : Application
{
    private readonly Supabase.Client _supabase;
    private readonly IServiceProvider _provider;

    public App(IServiceProvider provider)
    {
        InitializeComponent();
        _provider = provider;
        _supabase = provider.GetRequiredService<Supabase.Client>();

        MainPage = new ContentPage
        {
            BackgroundColor = Colors.White,
            Content = new ActivityIndicator { IsRunning = true, Color = Colors.Blue }
        };
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await Task.Delay(100);

        try
        {
            // 1. MANUALLY LOAD SESSION FROM STORAGE
            // We use the same key "supabase.session" defined in MauiSessionHandler
            string json = Preferences.Default.Get("supabase.session", string.Empty);

            if (!string.IsNullOrEmpty(json))
            {
                Debug.WriteLine("[App] Found saved session! Restoring...");

                // Deserialize
                var session = JsonConvert.DeserializeObject<Session>(json);

                if (session != null && !string.IsNullOrEmpty(session.AccessToken))
                {
                    // FORCE SUPABASE TO USE THIS SESSION
                    await _supabase.Auth.SetSession(session.AccessToken, session.RefreshToken);
                    Debug.WriteLine("[App] Session restored successfully.");
                }
            }

            // 2. NOW check if we have a user (SetSession should have populated this)
            var currentUser = _supabase.Auth.CurrentUser;
            bool isRemembered = Preferences.Default.Get("RememberMe", false);

            // 3. Navigate
            if (currentUser != null && isRemembered)
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = _provider.GetRequiredService<LoginPage>();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[App] Login Error: {ex.Message}");
            MainPage = _provider.GetRequiredService<LoginPage>();
        }
    }
}