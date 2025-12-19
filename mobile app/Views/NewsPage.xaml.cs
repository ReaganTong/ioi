using mobile_app.ViewModels;

namespace mobile_app.Views;

public partial class NewsPage : ContentPage
{
    public NewsPage(NewsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        // Refresh news every time the student opens this tab
        if (BindingContext is NewsViewModel vm)
        {
            await vm.LoadNewsAsync();
        }
    }
}