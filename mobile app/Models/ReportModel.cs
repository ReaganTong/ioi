using Supabase.Postgrest.Attributes; // FIX: Added 'Supabase.' prefix
using Supabase.Postgrest.Models;     // FIX: Added 'Supabase.' prefix

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

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("image_url")] // Use the exact column name from your Supabase Table
    public string? ImageUrl { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

}