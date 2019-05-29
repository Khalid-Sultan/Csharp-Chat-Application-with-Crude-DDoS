using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Server_Side.Converters
{
    public class BooleanToBrushConverter : IValueConverter
    {
        private readonly Brush bTrue = new SolidColorBrush(Color.FromArgb(255, 192, 255, 192));
        private readonly Brush bFalse = new SolidColorBrush(Color.FromArgb(255, 255, 192, 192));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                if ((bool)value)
                    return bTrue;
            return bFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Brush)value == bTrue;
        }
    }
    public class BooleanToClientStatusMessageConverter : IValueConverter
    {
        private const string strTrue = "Server is connected";
        private const string strFalse = "Server is not connected";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                if ((bool)value)
                    return strTrue;
            return strFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString() == strTrue;
        }
    }
    public class BooleanToConnectDisconnectConverter : IValueConverter
    {
        private const string strTrue = "Disconnect";
        private const string strFalse = "Connect";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                if ((bool)value)
                    return strTrue;
            return strFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString() == strTrue;
        }
    }
    public class BooleanToServerStatusMessageConverter : IValueConverter
    {
        private const string strTrue = "Server is active";
        private const string strFalse = "Server is stopped";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                if ((bool)value)
                    return strTrue;
            return strFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString() == strTrue;
        }
    }
    public class BooleanToStartStopConverter : IValueConverter
    {
        private const string strTrue = "Stop";
        private const string strFalse = "Start";

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
                if ((bool)value)
                    return strTrue;
            return strFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString() == strTrue;
        }
    }
}
