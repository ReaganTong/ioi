namespace mobile_app.Models;

public class Question
{
    public string Text { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public int CorrectOption { get; set; } // 1, 2, or 3
}