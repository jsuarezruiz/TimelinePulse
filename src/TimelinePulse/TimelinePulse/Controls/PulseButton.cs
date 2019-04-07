using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace TimelinePulse.Controls
{
    public class PulseButton : SKCanvasView
    {
        SKBitmap _resourceBitmap;
        Stopwatch _stopwatch;
        float _time;

        public PulseButton()
        {
            _stopwatch = new Stopwatch();

            Start();
        }

        public static readonly BindableProperty PulseColorProperty =
            BindableProperty.Create(nameof(PulseColor), typeof(Color), typeof(PulseButton), Color.Red, propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty EmbeddedImageNameProperty =
            BindableProperty.Create(nameof(EmbeddedImageName), typeof(string), typeof(PulseButton), "", propertyChanged: OnPropertyChanged);

        public static readonly BindableProperty PulseSpeedProperty =
            BindableProperty.Create(nameof(PulseSpeed), typeof(int), typeof(PulseButton), 10, propertyChanged: OnPropertyChanged);

        public Color PulseColor
        {
            get { return (Color)GetValue(PulseColorProperty); }
            set { SetValue(PulseColorProperty, value); }
        }

        public string EmbeddedImageName
        {
            get { return (string)GetValue(EmbeddedImageNameProperty); }
            set { SetValue(EmbeddedImageNameProperty, value); }
        }

        public int PulseSpeed
        {
            get { return (int)GetValue(PulseSpeedProperty); }
            set { SetValue(PulseSpeedProperty, value); }
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
            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke
            };

            byte R = (byte)(PulseColor.R * 255);
            byte G = (byte)(PulseColor.G * 255);
            byte B = (byte)(PulseColor.B * 255);

            canvas.Clear();

            SKPoint center = new SKPoint(info.Width / 2, info.Height / 2);
            float radius = info.Width / 2 * _time;

            paint.Color = new SKColor(R, G, B, (byte)(255 * (1 - _time)));
            paint.Style = SKPaintStyle.Fill;
            canvas.DrawCircle(center.X, center.Y, radius, paint);

            paint.Color = new SKColor(R, G, B);
            canvas.DrawCircle(center.X, center.Y, 85, paint);

            if (_resourceBitmap != null)
                canvas.DrawBitmap(_resourceBitmap, center.X - _resourceBitmap.Width / 2, center.Y - _resourceBitmap.Height / 2);
        }

        private static void OnPropertyChanged(BindableObject bindable, object oldVal, object newVal)
        {
            var pulse = bindable as PulseButton;

            pulse?.InvalidateSurface();
        }

        private void Start()
        {
            _stopwatch.Start();
            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                var speed = PulseSpeed * 250;
                _time = (float)(_stopwatch.Elapsed.TotalMilliseconds % speed / speed);

                InvalidateSurface();

                return true;
            });
        }
    }
}