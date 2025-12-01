using SQLite;

namespace mobile_app.Models;

public class ReportModel
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Description { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string EvidencePath { get; set; } // Store file path, not the image itself
    public DateTime DateReported { get; set; }
}