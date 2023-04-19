// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;
using SkiaSharp;

namespace Microcharts
{
    /// <summary>
    /// ![chart](../images/RadialGauge.png)
    ///
    /// Radial gauge chart.
    /// </summary>
    public class RadialGaugeChart : SimpleChart
    {
        #region Properties

        /// <summary>
        /// Gets or sets the size of each gauge. If negative, then its will be calculated from the available space.
        /// </summary>
        /// <value>The size of the line.</value>
        public float LineSize { get; set; } = -1;

        /// <summary>
        /// Gets or sets the gauge background area alpha.
        /// </summary>
        /// <value>The line area alpha.</value>
        public byte LineAreaAlpha { get; set; } = 52;

        /// <summary>
        /// Gets or sets the start angle.
        /// </summary>
        /// <value>The start angle.</value>
        public float StartAngle { get; set; } = -90;

        /// <summary>
        /// Gets or sets if the legend is all right
        /// </summary>
        /// <value>The placement of the legend.</value>
        public bool ForceLegendRight { get; set; } = false;

        private float AbsoluteMinimum => Entries?.Where(x => x.Value.HasValue).Select(x => x.Value.Value).Concat(new[] { MaxValue, MinValue, InternalMinValue ?? 0 }).Min(x => Math.Abs(x)) ?? 0;

        private float AbsoluteMaximum => Entries?.Where(x => x.Value.HasValue).Select(x => x.Value.Value).Concat(new[] { MaxValue, MinValue, InternalMinValue ?? 0 }).Max(x => Math.Abs(x)) ?? 0;

        /// <inheritdoc />
        protected override float ValueRange => AbsoluteMaximum - AbsoluteMinimum;

        #endregion

        #region Methods

        public void DrawGaugeArea(SKCanvas canvas, ChartEntry entry, float radius, int cx, int cy, float strokeWidth)
        {
            using (var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = strokeWidth,
                Color = entry.Color.WithAlpha(LineAreaAlpha),
                IsAntialias = true,
            })
            {
                canvas.DrawCircle(cx, cy, radius, paint);
            }
        }

        public void DrawGauge(SKCanvas canvas, ChartEntry entry, float radius, int cx, int cy, float strokeWidth)
        {
            var valToPaint = entry.Value;
            var maxValue = InternalMaxValue.HasValue ? InternalMaxValue.Value : ValueRange;
            if (maxValue <= 0)
                maxValue = Math.Abs(valToPaint.Value);

            int count = 0;

            while (valToPaint > 0 && count < 4)
            {
                var color = ChangeColorBrightness(entry.Color, RadialGaugeDarkenValue * count * -0.01f);

                using (var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = strokeWidth,
                    StrokeCap = SKStrokeCap.Butt,
                    Color = color,
                    IsAntialias = true,
                })
                {
                    using (SKPath path = new SKPath())
                    {
                        var sweepAngle = AnimationProgress * 360 * Math.Abs(valToPaint.Value) / maxValue;
                        path.AddArc(SKRect.Create(cx - radius, cy - radius, 2 * radius, 2 * radius), StartAngle, sweepAngle);
                        canvas.DrawPath(path, paint);
                    }
                }

                valToPaint -= maxValue;
                count++;
            }
        }

        /// <summary>
        /// Creates color with corrected brightness.
        /// </summary>
        /// <param name="color">Color to correct.</param>
        /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
        /// Negative values produce darker colors.</param>
        /// <returns>
        /// Corrected <see cref="SKColor"/> structure.
        /// </returns>
        public static SKColor ChangeColorBrightness(SKColor color, float correctionFactor)
        {
            float red = color.Red;
            float green = color.Green;
            float blue = color.Blue;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return new SKColor((byte)red, (byte)green, (byte)blue, color.Alpha);
        }

        public override void DrawContent(SKCanvas canvas, int width, int height)
        {
            if (Entries != null)
            {
                var sumValue = Entries.Where(x => x.Value.HasValue).Sum(x => Math.Abs(x.Value.Value));
                var radius = (Math.Min(width, height) - (2 * Margin)) / 2;
                var cx = Convert.ToInt32(radius);
                var cy = height / 2;
                var lineWidth = (LineSize < 0) ? (radius / ((Entries.Count() + 1) * 2)) : LineSize;
                var radiusSpace = lineWidth * 2;

                for (int i = 0; i < Entries.Count(); i++)
                {
                    var entry = Entries.ElementAt(i);

                    //Skip the ring if it has a null value
                    if (!entry.Value.HasValue) continue;

                    var entryRadius = (i + 1) * radiusSpace;
                    DrawGaugeArea(canvas, entry, entryRadius, cx, cy, lineWidth);
                    DrawGauge(canvas, entry, entryRadius, cx, cy, lineWidth);
                }

                //Make sure captions draw on top of chart
                DrawCaption(canvas, width, height); 
            }
        }

        private void DrawCaption(SKCanvas canvas, int width, int height)
        {
            if (ForceLegendRight)
            {
                DrawCaptionElements(canvas, width, height, Entries.Reverse().ToList(), false, false, true);
            }
            else
            {
                var rightValues = Entries.Take(Entries.Count() / 2).ToList();
                var leftValues = Entries.Skip(rightValues.Count()).ToList();

                leftValues.Reverse();

                DrawCaptionElements(canvas, width, height, rightValues, false, false);
                DrawCaptionElements(canvas, width, height, leftValues, true, false);
            }
        }

        #endregion
    }
}
