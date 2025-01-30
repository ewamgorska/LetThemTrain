using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LetEmTrain.UWP.Helpers
{
    public class PaneWidthToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is double paneWidth)
            {
                return new Thickness(paneWidth, 0, 0, 0);
            }
            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
