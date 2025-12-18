using mobile_app.ViewModels;

namespace mobile_app.Views;

public partial class HelpPage : ContentPage
{
    // The error often happens if this part is wrong
    public HelpPage(HelpViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm; // Connect the UI to the Logic
    }
}