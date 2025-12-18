using System.Globalization;

namespace mobile_app.ViewModels;

public class BoolToObjectConverter : IValueConverter
{
    public object TrueObject { get; set; }
    public object FalseObject { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // Handles "TrueText|FalseText" parameter format
            if (parameter is string paramString && paramString.Contains('|'))
            {
                var parts = paramString.Split('|');
                return boolValue ? parts[0] : parts[1];
            }
            return boolValue ? TrueObject : FalseObject;
        }
        return FalseObject;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}