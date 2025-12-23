using Microsoft.Win32;
using ShoeStoreLibrary.Contexts;
using ShoeStoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ShoeStoreWpfApp.Views
{
    /// <summary>
    /// Логика взаимодействия для ProductUpdateWindow.xaml
    /// </summary>
    public partial class ProductUpdateWindow : Window
    {
        public event Action ProductUpdated;

        private readonly ShoeStoreContext _context;
        private Product _product; // Товар для редактирования

        public ProductUpdateWindow(Product? product = null)
        {
            InitializeComponent();
            _context = new ShoeStoreContext();
            // Заполнение полей на основе выбранного товара для редактирования
            if (product != null)
            {
                _product = product;

                // Заполнение текстовых полей
                ProductIdTextBox.Text = _product.ProductId;
                ProductDescriptionTextBox.Text = _product.ProductDescription;
                PriceTextBox.Text = _product.Price.ToString();
                StockQuantityTextBox.Text = _product.StockQuantity.ToString();
                ActiveDiscountTextBox.Text = _product.ActiveDiscount?.ToString() ?? string.Empty;

                ProductNameComboBox.SelectedValuePath = "ProductNameId"; // Идентификатор наименования
                ManufacturerComboBox.SelectedValuePath = "ManufacturerId"; // Идентификатор производителя
                SupplierComboBox.SelectedValuePath = "SupplierId"; // Идентификатор поставщика
                CategoryComboBox.SelectedValuePath = "CategoryId"; // Идентификатор категории
                UnitOfMeasureComboBox.SelectedValuePath = "UnitOfMeasureId"; // Идентификатор единицы измерения

                // Установка выбранных значений
                ProductNameComboBox.SelectedValue = _product.ProductNameId;
                ManufacturerComboBox.SelectedValue = _product.ManufacturerId;
                SupplierComboBox.SelectedValue = _product.SupplierId;
                CategoryComboBox.SelectedValue = _product.CategoryId;
                UnitOfMeasureComboBox.SelectedValue = _product.UnitOfMeasureId; ;

                // Проверяем, есть ли изображение в поле Photo
                if (_product.Photo != null && _product.Photo.Length > 0)
                {
                    try
                    {
                        // Проверяем, являются ли первые байты заголовком изображения
                        bool isRawImage = (_product.Photo.Length > 2 && _product.Photo[0] == 0xFF && _product.Photo[1] == 0xD8) ||
                                          (_product.Photo.Length > 3 && _product.Photo[0] == 0x89 && _product.Photo[1] == 0x50);

                        if (isRawImage)
                        {
                            // Если это само изображение — отображаем его
                            DisplayImageFromBytes(_product.Photo);
                        }
                        else
                        {
                            // Если это не заголовок картинки, значит это путь (текст в байтах)
                            string decodedFileName = Encoding.UTF8.GetString(_product.Photo);
                            string relativePath = System.IO.Path.Combine(decodedFileName);
                            string localPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

                            // Проверяем, существует ли файл по этому пути
                            if (File.Exists(localPath))
                            {
                                ProductImage.Source = new BitmapImage(new Uri(localPath));
                            }
                            else
                            {
                                // Если файл не найден, загружаем заглушку
                                ProductImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/picture.png"));
                            }
                        }
                    }
                    catch
                    {
                        // В случае ошибки загружаем заглушку
                        ProductImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/picture.png"));
                    }
                }
                else
                {
                    // Загружаем изображение-заглушку
                    ProductImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/picture.png"));
                }
            }

            LoadComboBoxes(); // Заполнение ComboBox производителями, поставщиками и т.д.
        }

        private void DisplayImageFromBytes(byte[] imageData)
        {
            if (imageData != null && imageData.Length > 0)
            {
                using (var stream = new MemoryStream(imageData))
                {
                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    ProductImage.Source = bitmapImage;
                }
            }
        }

        // Метод для загрузки данных в ComboBox
        private void LoadComboBoxes()
        {

            ProductNameComboBox.ItemsSource = _context.ProductNames.ToList(); // Пример для наименований
            ManufacturerComboBox.ItemsSource = _context.Manufacturers.ToList();
            SupplierComboBox.ItemsSource = _context.Suppliers.ToList();
            CategoryComboBox.ItemsSource = _context.Categories.ToList();
            UnitOfMeasureComboBox.ItemsSource = _context.UnitOfMeasures.ToList();

            // Настройка отображаемых свойств для ComboBox
            ProductNameComboBox.DisplayMemberPath = "ProductName";
            ManufacturerComboBox.DisplayMemberPath = "ManufacturerName";
            SupplierComboBox.DisplayMemberPath = "SupplierName";
            CategoryComboBox.DisplayMemberPath = "CategoryName";
            UnitOfMeasureComboBox.DisplayMemberPath = "UnitName";
        }
        private string _tempImagePath;

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите фото товара"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Получаем путь к файлу
                _tempImagePath = openFileDialog.FileName; // Сохраняем временный путь к изображению

                // Отображаем изображение в интерфейсе
                ProductImage.Source = new BitmapImage(new Uri(_tempImagePath));
            }
        }


        // Метод для сохранения товара
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем идентификатор товара
                string productId = ProductIdTextBox.Text;
                var existingProduct = _context.Products.SingleOrDefault(p => p.ProductId == productId);

                // Проверка, существует ли товар
                if (existingProduct != null)
                {
                    // Обновление полей товара
                    existingProduct.ProductDescription = ProductDescriptionTextBox.Text;
                    existingProduct.Price = decimal.Parse(PriceTextBox.Text);
                    existingProduct.StockQuantity = int.Parse(StockQuantityTextBox.Text);
                    // Проверка скидки
                    if (string.IsNullOrEmpty(ActiveDiscountTextBox.Text))
                    {
                        existingProduct.ActiveDiscount = null; 
                    }
                    else if (int.TryParse(ActiveDiscountTextBox.Text, out int discount) && discount >= 0 && discount <= 100)
                    {
                        existingProduct.ActiveDiscount = discount; 
                    }
                    else
                    {
                        MessageBox.Show("Скидка должна быть в диапазоне от 0 до 100.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return; 
                    }
                    existingProduct.ProductNameId = (int)(ProductNameComboBox.SelectedValue ?? 0);
                    existingProduct.ManufacturerId = (int)(ManufacturerComboBox.SelectedValue ?? 0);
                    existingProduct.SupplierId = (int)(SupplierComboBox.SelectedValue ?? 0);
                    existingProduct.CategoryId = (int)(CategoryComboBox.SelectedValue ?? 0);
                    existingProduct.UnitOfMeasureId = (int)(UnitOfMeasureComboBox.SelectedValue ?? 0);

                    // Сохраняем изображение в папку приложения и ОТНОСИТЕЛЬНЫЙ путь в массив байтов
                    if (!string.IsNullOrEmpty(_tempImagePath))
                    {
                        // 1. Создаем папку, если её нет
                        string imagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                        if (!System.IO.Directory.Exists(imagesFolder))
                            System.IO.Directory.CreateDirectory(imagesFolder);

                        // 2. Формируем имя файла и полный путь для физического копирования
                        string fileName = $"{productId}.png";
                        string destinationFilePath = System.IO.Path.Combine(imagesFolder, fileName);

                        // 3. Копируем файл физически
                        System.IO.File.Copy(_tempImagePath, destinationFilePath, true);

                        // 4. ФОРМИРУЕМ ОТНОСИТЕЛЬНЫЙ ПУТЬ (только папка и имя файла)
                        string relativePath = System.IO.Path.Combine("Images", fileName);

                        // 5. Сохраняем именно ОТНОСИТЕЛЬНЫЙ путь в байтах
                        _product.Photo = Encoding.UTF8.GetBytes(relativePath);
                    }

                    // Сохранение изменений в базе данных
                    _context.SaveChanges();

                    // Вызываем событие после успешного сохранения
                    ProductUpdated?.Invoke();

                    MessageBox.Show("Данные успешно обновлены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Товар не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

    }
}
