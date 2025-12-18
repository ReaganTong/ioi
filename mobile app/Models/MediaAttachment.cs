namespace mobile_app.Models;

public class MediaAttachment
{
    public ImageSource? PreviewSource { get; set; } // The image or a video icon
    public FileResult File { get; set; }            // The actual file to upload
    public bool IsVideo { get; set; }               // To know if it's a video
}