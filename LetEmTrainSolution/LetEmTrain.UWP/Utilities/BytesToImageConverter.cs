using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace LetEmTrain.UWP.Utilities
{
    public class BytesToImageConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is byte[]))
                return new BitmapImage(new Uri("ms-appx:///Assets/Placeholder.png"));

            using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
            {
                using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes((byte[])value);
                    writer.StoreAsync().GetResults();
                }

                var image = new BitmapImage();
                image.SetSource(ms);
                return image;
            }


        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null || !(value is BitmapImage))
                return (null);
            return (null);
        }
    }
}
