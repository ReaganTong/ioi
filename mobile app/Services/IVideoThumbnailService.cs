namespace mobile_app.Services;

public interface IVideoThumbnailService
{
    Task<ImageSource> GetThumbnailAsync(string videoPath);
}