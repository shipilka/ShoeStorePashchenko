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
    /// Логика взаимодействия для ProductEditWindow.xaml
    /// </summary>
    public partial class ProductEditWindow : Window
    {
        public event Action ProductUpdated; // Событие для уведомления об обновлении спи

        private readonly ShoeStoreContext _context;
        private Product _product; // Товар для редактирования

        public ProductEditWindow(Product? product = null)
        {
            InitializeComponent();
            _context = new ShoeStoreContext();
            // Заполнение полей на основе выбранного товара для редактирования
            if (product != null)
            {
                _product = product;

                // Заполнение ComboBox для наименования продукта
                ProductNameComboBox.SelectedValue = product.ProductNameId;

                // Заполнение текстовых полей
                ProductDescriptionTextBox.Text = product.ProductDescription;
                PriceTextBox.Text = product.Price.ToString();
                StockQuantityTextBox.Text = product.StockQuantity.ToString();

                // Заполнение ComboBox для производителя
                ManufacturerComboBox.SelectedValue = product.ManufacturerId;

                // Заполнение ComboBox для поставщика
                SupplierComboBox.SelectedValue = product.SupplierId;

                // Заполнение ComboBox для категории
                CategoryComboBox.SelectedValue = product.CategoryId;

                // Заполнение ComboBox для единицы измерения
                UnitOfMeasureComboBox.SelectedValue = product.UnitOfMeasureId;

                // Заполнение поля для активной скидки
                if (product.ActiveDiscount.HasValue)
                {
                    ActiveDiscountTextBox.Text = product.ActiveDiscount.Value.ToString();
                }
                else
                {
                    ActiveDiscountTextBox.Text = string.Empty; // Или устанавливаем пустую строку
                }

                // Отображение фото
                if (_product.Photo != null && _product.Photo.Length > 0)
                {
                    string imagePath = Encoding.UTF8.GetString(_product.Photo);
                    ProductImage.Source = new BitmapImage(new Uri(imagePath));
                }
                else
                {
                    //изображение по умолчанию
                    ProductImage.Source = new BitmapImage(new Uri("pack://application:,,,/Assets/picture.png")); // Заглушка
                }
            }

            LoadComboBoxes(); // Заполнение ComboBox производителями, поставщиками и т.д.
        }

        // Метод для загрузки данных в ComboBox
        private void LoadComboBoxes()
        {
            
            ProductNameComboBox.ItemsSource = _context.ProductNames.ToList();
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
                string productId = ProductIdTextBox.Text.Trim();

                // Проверка идентификатора продукта
                if (string.IsNullOrWhiteSpace(productId))
                {
                    MessageBox.Show("Идентификатор продукта должен быть заполнен.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var existingProduct = _context.Products.SingleOrDefault(p => p.ProductId == productId);

                if (existingProduct != null)
                {
                    var result = MessageBox.Show("Данный товар уже существует, хотите заменить?", "Подтверждение", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                _product = existingProduct ?? new Product();

                // Основные поля продукта
                _product.ProductId = productId;
                _product.ProductDescription = ProductDescriptionTextBox.Text.Trim();
                _product.Price = decimal.Parse(PriceTextBox.Text);
                _product.StockQuantity = int.Parse(StockQuantityTextBox.Text);
                // Проверка значения скидки
                if (!string.IsNullOrEmpty(ActiveDiscountTextBox.Text))
                {
                    if (int.TryParse(ActiveDiscountTextBox.Text, out int discountValue))
                    {
                        if (discountValue < 0 || discountValue > 100)
                        {
                            MessageBox.Show("Скидка должна быть в диапазоне от 0 до 100.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        _product.ActiveDiscount = discountValue;
                    }
                    else
                    {
                        MessageBox.Show("Введите корректное значение для скидки.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    _product.ActiveDiscount = null; // Если поле пустое, сохраняем как null
                }

                // Получение значений из комбобоксов
                var selectedProductName = ProductNameComboBox.SelectedItem as ProductNames;
                if (selectedProductName != null)
                {
                    _product.ProductNameId = selectedProductName.ProductNameId;
                }

                var selectedManufacturer = ManufacturerComboBox.SelectedItem as Manufacturer;
                if (selectedManufacturer != null)
                {
                    _product.ManufacturerId = selectedManufacturer.ManufacturerId;
                }

                var selectedSupplier = SupplierComboBox.SelectedItem as Supplier;
                if (selectedSupplier != null)
                {
                    _product.SupplierId = selectedSupplier.SupplierId;
                }

                var selectedCategory = CategoryComboBox.SelectedItem as Category;
                if (selectedCategory != null)
                {
                    _product.CategoryId = selectedCategory.CategoryId;
                }

                var selectedUnitOfMeasure = UnitOfMeasureComboBox.SelectedItem as UnitOfMeasure;
                if (selectedUnitOfMeasure != null)
                {
                    _product.UnitOfMeasureId = selectedUnitOfMeasure.UnitOfMeasureId;
                }

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

                // Сохранение продукта
                if (existingProduct == null)
                {
                    _context.Products.Add(_product);
                }

                _context.SaveChanges();
                // Если сохранение прошло успешно, вызываем событие
                ProductUpdated?.Invoke();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении товара: {ex.Message}\n{ex.InnerException?.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        

    }
}
