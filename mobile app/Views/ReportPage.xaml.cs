using mobile_app.ViewModels;

namespace mobile_app.Views;

public partial class ReportPage : ContentPage
{
    public ReportPage(ReportViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}