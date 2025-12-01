using Mapsui; // Required for Map object
using Mapsui.Projections;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using mobile_app.ViewModels;

namespace mobile_app.Views;

public partial class ReportPage : ContentPage
{
    private ReportViewModel _viewModel;

    public ReportPage(ReportViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        // 1. Initialize the Map to use OpenStreetMap
        MyMap.Map?.Layers.Add(OpenStreetMap.CreateTileLayer());

        // 2. Center the map on UTS (Sibu, Sarawak)
        // UTS Coords: 2.3423° N, 111.8327° E
        // Mapsui uses "Spherical Mercator" format, so we convert Lat/Lon to that.
        var utsPoint = SphericalMercator.FromLonLat(111.8327, 2.3423);
        MyMap.Map.Home = n => n.NavigateTo(utsPoint, 3000); // 3000 is zoom level

        // 3. Listen for Taps on the map
        MyMap.MapClicked += OnMapClicked;
    }

    private void OnMapClicked(object? sender, MapClickedEventArgs e)
    {
        // Get the point where user tapped
        var point = e.Point;

        // Convert back to GPS (Lat/Lon)
        var latLon = SphericalMercator.ToLonLat(point);

        // Update the ViewModel
        if (_viewModel != null)
        {
            _viewModel.Latitude = latLon.Y;
            _viewModel.Longitude = latLon.X;
            _viewModel.LocationLabel = $"Pinned: {latLon.Y:F4}, {latLon.X:F4}";

            // Add a visual Pin (Feature)
            var pinLayer = new Mapsui.Layers.MemoryLayer
            {
                Name = "PinLayer",
                Features = new[] { new PointFeature(point) }
            };

            // Clear old pins and add new one
            var oldLayer = MyMap.Map.Layers.FirstOrDefault(l => l.Name == "PinLayer");
            if (oldLayer != null) MyMap.Map.Layers.Remove(oldLayer);

            MyMap.Map.Layers.Add(pinLayer);
        }
    }
}