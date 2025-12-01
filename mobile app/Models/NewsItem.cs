namespace mobile_app.Models;

public class NewsItem
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Date { get; set; }
    public string Color { get; set; } = "#FFFFFF"; // Default white
}