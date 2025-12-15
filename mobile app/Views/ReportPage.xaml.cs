using mobile_app.ViewModels;
using System.Web;

namespace mobile_app.Views;

public partial class ReportPage : ContentPage
{
    private ReportViewModel _viewModel;

    public ReportPage(ReportViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        LoadLocalMap();

        // FIX: Subscribe to the "Force Move" event from the ViewModel
        // This ensures the map ALWAYS zooms when you click "Get Location", 
        // even if the location hasn't changed much.
        if (_viewModel != null)
        {
            _viewModel.RequestSetLocation += (lat, lon) =>
            {
                // We run this on the main thread to be safe
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await NasaWebView.EvaluateJavaScriptAsync($"setLocation({lat}, {lon})");
                });
            };
        }
    }

    private async void LoadLocalMap()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("NasaMap.html");
            using var reader = new StreamReader(stream);
            var htmlContent = await reader.ReadToEndAsync();
            NasaWebView.Source = new HtmlWebViewSource { Html = htmlContent };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Map Error: {ex.Message}");
        }
    }

    // 2. JavaScript -> C# (When you Tap the Map)
    private void OnMapNavigating(object sender, WebNavigatingEventArgs e)
    {
        if (e.Url.StartsWith("app://pin"))
        {
            e.Cancel = true;
            try
            {
                var uri = new Uri(e.Url);
                var query = HttpUtility.ParseQueryString(uri.Query);

                if (double.TryParse(query["lat"], out double lat) &&
                    double.TryParse(query["lon"], out double lon))
                {
                    if (_viewModel != null)
                    {
                        // Simply update the numbers for the report
                        // We DO NOT force the map to move here, preventing the "choppy" loop
                        _viewModel.Latitude = lat;
                        _viewModel.Longitude = lon;
                        _viewModel.LocationLabel = $"Pinned: {lat:F4}, {lon:F4}";
                    }
                }
            }
            catch { }
        }
    }
}