using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Data;

namespace AsyncTaskManagerWpf.Converters
{
    class HighlightConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value == null || parameter == null )
                return value;

            string text = value.ToString();
            string keyword = parameter.ToString();

            if ( string.IsNullOrWhiteSpace( keyword ) )
                return text;

            if ( text.ToLower().Contains( keyword.ToLower() ) ) {
                return text.Replace( keyword, $"[{keyword}]" );
            }

            return text;
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            return value;
        }
    }
}
