// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microcharts
{
    using SkiaSharp;

    /// <summary>
    /// A data entry for a chart.
    /// </summary>
    public class ChartEntry
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Microcharts.ChartEntry"/> class.
        /// </summary>
        /// <param name="value">The entry value.</param>
        public ChartEntry(float? value)
        {
            this.Value = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public float? Value { get; }

        /// <summary>
        /// Gets or sets the caption label.
        /// </summary>
        /// <value>The label.</value>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the label associated to the value.
        /// </summary>
        /// <value>The value label.</value>
        public string ValueLabel { get; set; }

        /// <summary>
        /// Gets or sets the color of the fill.
        /// </summary>
        /// <value>The color of the fill.</value>
        public SKColor Color { get; set; } = SKColors.Black;

        /// <summary>
        /// Gets or sets the color of the rest part
        /// </summary>
        /// <value>The color of the rest part.</value>
        public SKColor OtherColor { get; set; } = SKColor.Empty;

        /// <summary>
        /// Gets or sets the color of the text (for the caption label).
        /// </summary>
        /// <value>The color of the text.</value>
        public SKColor TextColor { get; set; } = SKColors.Gray;

        /// <summary>
        /// Gets or sets the color of the value label
        /// </summary>
        /// <value>The color of the value label.</value>
        public SKColor ValueLabelColor { get; set; } = SKColors.Black;

        /// <summary>
        /// Gets or sets max value for entry
        /// </summary>
        /// <value>Max value for entry to be able to display separate max values for different entries in the same chart</value>
        public float EntryMaxValue { get; set; } = -1;
        #endregion
    }
}
