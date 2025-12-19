using System.Diagnostics; // Needed for Debug.WriteLine
using Newtonsoft.Json;
using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;

namespace mobile_app.Services;

public class MauiSessionHandler : IGotrueSessionPersistence<Session>
{
    const string Key = "supabase.session";

    public void SaveSession(Session session)
    {
        try
        {
            // Save the session to secure storage
            string json = JsonConvert.SerializeObject(session);
            Preferences.Default.Set(Key, json);
            Debug.WriteLine($"[MauiSessionHandler] SAVED Session. Length: {json.Length}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MauiSessionHandler] SAVE FAILED: {ex.Message}");
        }
    }

    public Session LoadSession()
    {
        try
        {
            // Try to get the string from storage
            string json = Preferences.Default.Get(Key, string.Empty);

            if (string.IsNullOrEmpty(json))
            {
                Debug.WriteLine("[MauiSessionHandler] LOAD: No saved session found in Preferences.");
                return null;
            }

            Debug.WriteLine($"[MauiSessionHandler] LOAD: Found session string ({json.Length} chars). Deserializing...");

            var session = JsonConvert.DeserializeObject<Session>(json);

            if (session == null)
                Debug.WriteLine("[MauiSessionHandler] LOAD: Deserialization returned null.");
            else
                Debug.WriteLine($"[MauiSessionHandler] LOAD SUCCESS: User {session.User?.Email}");

            return session;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MauiSessionHandler] LOAD ERROR: {ex.Message}");
            return null;
        }
    }

    public void DestroySession()
    {
        Debug.WriteLine("[MauiSessionHandler] Destroying Session.");
        Preferences.Default.Remove(Key);
    }
}