using mobile_app.ViewModels;
using System.Web; // Needed for parsing URL query

namespace mobile_app.Views;

public partial class ReportPage : ContentPage
{
    private ReportViewModel _viewModel;

    public ReportPage(ReportViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        // 1. Load the Map
        LoadLocalMap();

        // 2. Listen for "Get My Location" updates
        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
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

    // Runs when ViewModel Latitude changes (bridge from C# to JS)
    private async void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ReportViewModel.Latitude))
        {
            if (_viewModel.Latitude != 0 && _viewModel.Longitude != 0)
            {
                // Send coordinates to the Map
                await NasaWebView.EvaluateJavaScriptAsync($"setLocation({_viewModel.Latitude}, {_viewModel.Longitude})");
            }
        }
    }

    // Runs when Map is Tapped (bridge from JS to C#)
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