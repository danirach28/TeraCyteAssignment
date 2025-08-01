using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TeraCyteAssignment.Models;

namespace TeraCyteAssignment.Converters
{
    public class HistogramBarsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Ensure we have the correct inputs.
            if (values.Length < 3 ||
                !(values[0] is int[] histogramData) ||
                !(values[1] is double canvasWidth) ||
                !(values[2] is double canvasHeight))
            {
                return null;
            }

            var bars = new List<HistogramBar>();
            if (histogramData == null || histogramData.Length == 0 || canvasWidth <= 0 || canvasHeight <= 0)
            {
                return bars; // Return an empty collection if there's nothing to draw.
            }

            // This is the same logic that was in the ViewModel, but now it uses live UI dimensions.
            double barWidth = canvasWidth / histogramData.Length;
            int maxCount = histogramData.Max();
            if (maxCount == 0) maxCount = 1; // Avoid division by zero.

            for (int i = 0; i < histogramData.Length; i++)
            {
                double barHeight = ((double)histogramData[i] / maxCount) * canvasHeight;
                double leftPosition = i * barWidth;
                bars.Add(new HistogramBar(leftPosition, barHeight, barWidth));
            }

            return bars;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // This is a one-way conversion, so we don't need to implement ConvertBack.
            throw new NotImplementedException();
        }
    }
}
