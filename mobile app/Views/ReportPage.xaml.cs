using Microsoft.Maui.Maps;

namespace mobile_app.Views;

public partial class ReportPage : ContentPage
{
    // Constructor
    public ReportPage(ViewModels.ReportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // This runs when the page appears
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // UTS Coordinates: 2.3423, 111.8327
        var utsLocation = new Location(2.3423, 111.8327);

        // Zoom into UTS (0.5km radius)
        var mapSpan = MapSpan.FromCenterAndRadius(utsLocation, Distance.FromKilometers(0.5));

        IncidentMap.MoveToRegion(mapSpan);
    }
}