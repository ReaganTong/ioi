using Android.Media;
using Android.Graphics;
using mobile_app.Services;

namespace mobile_app.Platforms.Android;

public class VideoThumbnailService : IVideoThumbnailService
{
    public async Task<ImageSource> GetThumbnailAsync(string videoPath)
    {
        return await Task.Run(() =>
        {
            try
            {
                // FIX 1: Add 'using' to release memory immediately after use
                using var retriever = new MediaMetadataRetriever();

                retriever.SetDataSource(videoPath);

                // FIX 2: Use the 'Option' Enum, not the class name
                // In C#, it is 'Option.ClosestSync', not 'MediaMetadataRetriever.OptionClosestSync'
                var bitmap = retriever.GetFrameAtTime(0, Option.ClosestSync);

                if (bitmap != null)
                {
                    var stream = new MemoryStream();
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                    stream.Position = 0;

                    return ImageSource.FromStream(() => stream);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Thumbnail Error: {ex.Message}");
            }

            return null;
        });
    }
}