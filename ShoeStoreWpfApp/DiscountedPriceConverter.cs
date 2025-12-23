using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShoeStoreWpfApp
{
    public class DiscountedPriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, что пришли оба значения и они не null
            if (values.Length >= 2 && values[0] != null && values[1] != null)
            {
                // Извлекаем цену и скидку из массива
                int price = System.Convert.ToInt32(values[0]);
                int discount = System.Convert.ToInt32(values[1]);

                if (discount > 0)
                {
                    // Считаем: приводим к double, чтобы не потерять точность при делении на 100
                    double finalPrice = price * (1.0 - (double)discount / 100.0);

                    // Возвращаем строку с округлением до 2 знаков
                    return string.Format("{0:N2}", finalPrice);
                }
            }
            return ""; // Если скидки нет или данные неверны
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

