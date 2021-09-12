using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AcousticTransferMatrices.Core.ValueConverters
{
    public class MeterToMillimeterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value.ToString(), out double result))
            {
                if (result == 0)
                    return double.PositiveInfinity;
                else
                    return result * 1000;
            }
            

            throw new Exception("Not supported format");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {         
            if (double.TryParse(value.ToString(), out double result))
                return result / 1000;
            else
                return 0;
        }
    }
}
