using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ShoeStoreWpfApp
{
    public class ByteArrayToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // проверка, что переданное значение - это массив байтов
            if (value is byte[] photoData && photoData.Length > 0)
            {
                try
                {
                    // Проверяем заголовки изображений
                    bool isRawImage = (photoData.Length > 2 && photoData[0] == 0xFF && photoData[1] == 0xD8) ||
                                      (photoData.Length > 3 && photoData[0] == 0x89 && photoData[1] == 0x50);

                    if (isRawImage)
                    {
                        // Если это само изображение, создаем BitmapImage из массива байтов
                        using (var stream = new MemoryStream(photoData))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream;
                            bitmap.EndInit();
                            bitmap.Freeze();
                            return bitmap; // Возвращаем объект ImageSource
                        }
                    }
                    else
                    {
                        // Если это путь, декодируем имя файла
                        string decodedFileName = Encoding.UTF8.GetString(photoData).Trim();
                        string relativePath = System.IO.Path.Combine(/*"Images",*/ decodedFileName);
                        string localPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                        if (File.Exists(localPath))
                        {
                            var bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.UriSource = new Uri(localPath);
                            bitmap.EndInit();
                            bitmap.Freeze();
                            return bitmap; // Возвращаем загруженное изображение
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Опционально: логировать исключение
                }
            }

            // Возвращаем заглушку, если нет данных или возникла ошибка
            return new BitmapImage(new Uri("pack://application:,,,/Assets/picture.png", UriKind.Absolute));
        }

        // Реализация ConvertBack (можно оставить пустым, если не нужна)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
