using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace TimelinePulse.Controls
{
    public class CircularImage : SKCanvasView
    {
        SKPath CircularPath = SKPath.ParseSvgPathData("M -1,0 A 1,1 0 1 1 1,0 M -1,0 A 1,1 0 1 0 1,0");

        SKBitmap _resourceBitmap;

        public static readonly BindableProperty EmbeddedImageNameProperty =
            BindableProperty.Create(nameof(EmbeddedImageName), typeof(string), typeof(CircularImage), "", propertyChanged: OnPropertyChanged);

        public string EmbeddedImageName
        {
            get { return (string)GetValue(EmbeddedImageNameProperty); }
            set { SetValue(EmbeddedImageNameProperty, value); }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(EmbeddedImageName))
            {
                string resourceID = EmbeddedImageName;
                Assembly assembly = GetType().GetTypeInfo().Assembly;
                using (Stream stream = assembly.GetManifestResourceStream(resourceID))
                {
                    if (stream != null)
                        _resourceBitmap = SKBitmap.Decode(stream);
                }

                _resourceBitmap = _resourceBitmap?.Resize(new SKImageInfo(90, 90), SKFilterQuality.Medium);
            }
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
        {
            base.OnPaintSurface(args);

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            CircularPath.GetBounds(out SKRect bounds);
            canvas.Translate(info.Width / 2, info.Height / 2);
            canvas.Scale(0.98f * info.Height / bounds.Height);
            canvas.Translate(-bounds.MidX, -bounds.MidY);
            canvas.ClipPath(CircularPath);
            canvas.ResetMatrix();

            if (_resourceBitmap != null)
            {
                canvas.DrawBitmap(_resourceBitmap, info.Rect);
            }
        }

        private static void OnPropertyChanged(BindableObject bindable, object oldVal, object newVal)
        {
            var pulse = bindable as PulseButton;

            pulse?.InvalidateSurface();
        }
    }
}