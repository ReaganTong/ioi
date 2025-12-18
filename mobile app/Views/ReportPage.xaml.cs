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

        if (_viewModel != null)
        {
            _viewModel.RequestSetLocation += (lat, lon) =>
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await NasaWebView.EvaluateJavaScriptAsync($"setLocation({lat}, {lon})");
                });
            };
        }
    }

    // --- KEYBOARD CLOSING LOGIC ---
    private void OnBackgroundClicked(object sender, TappedEventArgs e)
    {
        if (DescriptionEditor.IsFocused)
        {
            DescriptionEditor.Unfocus();
        }
    }
    // -----------------------------

    private async void LoadLocalMap()
    {
        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("NasaMap.html");
            using var reader = new StreamReader(stream);
            var htmlContent = await reader.ReadToEndAsync();
            NasaWebView.Source = new HtmlWebViewSource { Html = htmlContent };
        }
        catch { }
    }

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