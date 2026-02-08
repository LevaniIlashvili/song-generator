using SkiaSharp;

namespace SongGenerator.Services;

public static class ImageGenerator
{
    public static byte[] GenerateCover(long seed, string title, string artist)
    {
        int internalSeed = (int)(seed ^ (seed >> 32));
        var rnd = new Random(internalSeed);

        int width = 400;
        int height = 400;

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        var canvas = surface.Canvas;

        var baseColor = new SKColor((byte)rnd.Next(50, 150), (byte)rnd.Next(50, 150), (byte)rnd.Next(50, 150));
        canvas.Clear(baseColor);

        using (var paint = new SKPaint { IsAntialias = true })
        {
            for (int i = 0; i < 15; i++)
            {
                paint.Color = new SKColor((byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256), 60);

                if (rnd.Next(2) == 0)
                    canvas.DrawCircle(rnd.Next(width), rnd.Next(height), rnd.Next(20, 100), paint);
                else
                    canvas.DrawRect(new SKRect(rnd.Next(width), rnd.Next(height), rnd.Next(width), rnd.Next(height)), paint);
            }
        }

        DrawTextWithShadow(canvas, title, width / 2f, height / 2 - 20, 32);
        DrawTextWithShadow(canvas, artist, width / 2f, height / 2 + 30, 22);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    private static void DrawTextWithShadow(SKCanvas canvas, string text, float x, float y, int size)
    {
        string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "verdenai.ttf");

        if (!File.Exists(fontPath))
        {
            Console.WriteLine($"--- FONT NOT FOUND AT: {fontPath}");
            fontPath = Path.Combine(Directory.GetCurrentDirectory(), "Fonts", "verdenai.ttf");
            Console.WriteLine($"--- TRYING ALTERNATE PATH: {fontPath} (Exists: {File.Exists(fontPath)})");
        }

        using var typeface = File.Exists(fontPath)
            ? SKTypeface.FromFile(fontPath)
            : SKTypeface.Default;

        using var font = new SKFont(typeface, size);
        using var paint = new SKPaint { IsAntialias = true };

        if (text.Length > 20) text = text.Substring(0, 17) + "...";

        paint.Color = SKColors.Black.WithAlpha(180);
        canvas.DrawText(text, x + 2, y + 2, SKTextAlign.Center, font, paint);

        paint.Color = SKColors.White;
        canvas.DrawText(text, x, y, SKTextAlign.Center, font, paint);
    }
}