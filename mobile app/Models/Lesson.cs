namespace mobile_app.Models;

public class Lesson
{
    public string? Title { get; set; }
    public string? ShortDescription { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public string? Route { get; set; }

    // NEW: Content for the detailed page
    public string? LongDefinition { get; set; }
    public List<string> Examples { get; set; } = new();
    public List<string> ActionSteps { get; set; } = new();
}