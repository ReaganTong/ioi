using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace mobile_app.Models;

[Table("reports")]
public class ReportModel : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("location")]
    public string Location { get; set; }

    [Column("student_id")]
    public string StudentId { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    // NEW: To track if issue is fixed
    [Column("status")]
    public string Status { get; set; } = "Pending";
}