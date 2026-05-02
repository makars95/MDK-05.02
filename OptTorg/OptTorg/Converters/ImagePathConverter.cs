using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace OptTorg.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string path && !string.IsNullOrEmpty(path))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();

                    if (path.StartsWith("pack://"))
                    {
                        bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    }
                    else if (Path.IsPathRooted(path))
                    {
                        bitmap.UriSource = new Uri(path, UriKind.Absolute);
                    }
                    else
                    {
                        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.TrimStart('/'));
                        if (File.Exists(fullPath))
                        {
                            bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                        }
                    }

                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка загрузки изображения: {ex.Message}");
                    return GetPlaceholderImage();
                }
            }
            return GetPlaceholderImage();
        }

        private BitmapImage GetPlaceholderImage()
        {
            try
            {
                // Создаем заглушку для отсутствующего изображения
                string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "no-image.png");
                if (File.Exists(placeholderPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(placeholderPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
            }
            catch { }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}