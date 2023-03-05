using ShitchenKink.Core.Services;

using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

using SImage = SixLabors.ImageSharp.Image;
using TPixel = SixLabors.ImageSharp.PixelFormats.Rgba32;

namespace ShitchenKink.Commands.Services;

public class IfunnyService
{
    private const string LogoFilename = "Assets/ifunny-logo.png";

    private readonly HttpService _http;

    private readonly Lazy<Task<Image<TPixel>>> _logoImageLazy
        = new(GetLogoImage, isThreadSafe: true);

    public IfunnyService(HttpService http)
    {
        _http = http;
    }

    public async Task<Stream> FromUrlAsync(string url, int jpegQuality)
    {
        await using var sourceStream = await _http.DownloadFileAsync(url);
        using var sourceImage = await SImage.LoadAsync<TPixel>(sourceStream);

        using var outputImage = await GenerateImageAsync(sourceImage);
        return await SaveToStreamAsync(outputImage, jpegQuality);
    }

    private async Task<SImage> GenerateImageAsync(SImage sourceImage)
    {
        var logoImage = await _logoImageLazy.Value;
        var outputImage = new Image<TPixel>(sourceImage.Width, sourceImage.Height + logoImage.Height);

        outputImage.Mutate(ctx =>
        {
            // Bottom right corner, below the source image
            var logoPosition = new Point(sourceImage.Width - logoImage.Width, sourceImage.Height);

            var graphicsOptions = new GraphicsOptions();
            ctx.DrawImage(sourceImage, Point.Empty, graphicsOptions);
            ctx.DrawImage(logoImage, logoPosition, graphicsOptions);

            // Draw padding on the left of the logo if necessary
            if (logoPosition.X > 0)
            {
                var logoBackground = logoImage[0, logoImage.Height / 2];
                var paddingRect = new RectangleF(0f, logoPosition.Y, logoPosition.X, logoImage.Height);

                ctx.Fill(logoBackground, paddingRect);
            }
        });

        return outputImage;
    }

    private static async Task<Stream> SaveToStreamAsync(SImage image, int jpegQuality)
    {
        var stream = new MemoryStream();
        await image.SaveAsJpegAsync(stream, new JpegEncoder
        {
            Quality = jpegQuality,
            SkipMetadata = true,
        });

        return stream;
    }

    private static Task<Image<TPixel>> GetLogoImage()
        => SImage.LoadAsync<TPixel>(LogoFilename);
}