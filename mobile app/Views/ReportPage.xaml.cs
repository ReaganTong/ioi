using Mapsui;
using Mapsui.Tiling;
using Mapsui.UI.Maui;
using Mapsui.Projections;
using Mapsui.Layers; // 👈 FIX: Required for PointFeature
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

        // 1. Initialize the Map
        MyMap.Map?.Layers.Add(OpenStreetMap.CreateTileLayer());

        // 2. Center the map on UTS (Sibu, Sarawak)
        // UTS Coordinates: 2.3423° N, 111.8327° E

        // FIX: FromLonLat now returns a tuple (double x, double y). 
        // We must convert it to an MPoint object manually.
        var coords = SphericalMercator.FromLonLat(111.8327, 2.3423);
        var utsPoint = new MPoint(coords.x, coords.y); // 👈 Created MPoint explicitly

        // FIX: 'Home' is removed. Use Navigator.
        MyMap.Map.Navigator.CenterOn(utsPoint);
        MyMap.Map.Navigator.ZoomTo(3000); // Zoom level

        // 3. Listen for Taps
        MyMap.MapClicked += OnMapClicked;
    }

    private void OnMapClicked(object? sender, MapClickedEventArgs e)
    {
        // e.Point gives us the Latitude/Longitude directly in Mapsui.Maui v5
        var lat = e.Point.Latitude;
        var lon = e.Point.Longitude;

        if (_viewModel != null)
        {
            _viewModel.Latitude = lat;
            _viewModel.Longitude = lon;
            _viewModel.LocationLabel = $"Pinned: {lat:F4}, {lon:F4}";

            // Draw the Pin
            // Convert GPS (Lat/Lon) back to Map Coordinates (Mercator)
            var mercatorCoords = SphericalMercator.FromLonLat(lon, lat);
            var mercatorPoint = new MPoint(mercatorCoords.x, mercatorCoords.y); // 👈 Convert tuple to MPoint

            // Create the Pin Layer
            var pinLayer = new MemoryLayer
            {
                Name = "PinLayer",
                Features = new[] { new PointFeature(mercatorPoint) } // 👈 Now valid because we passed MPoint
            };

            // Clear old pins
            var oldLayer = MyMap.Map.Layers.FirstOrDefault(l => l.Name == "PinLayer");
            if (oldLayer != null) MyMap.Map.Layers.Remove(oldLayer);

            MyMap.Map.Layers.Add(pinLayer);

            // Refresh map to show the pin
            MyMap.Refresh();
        }
    }
}