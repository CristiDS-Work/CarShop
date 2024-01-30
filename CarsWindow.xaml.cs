using CarShop.Data;
using CarShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CarShop
{
    /// <summary>
    /// Interaction logic for CarsWindow.xaml
    /// </summary>
    public partial class CarsWindow : Window
    {
        DataGrid dg;
        int editItem_ID = -1;
        int editItem_Index = -1;

        public CarsWindow()
        {
            InitializeComponent();

            dg = new DataGrid();
            dg.IsReadOnly = true;

            MainGrid.Children.Add(dg);

            var column = new DataGridTextColumn();
            column.Header = "Id";
            column.Binding = new Binding("Id");
            dg.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Header = "Brand";
            column.Binding = new Binding("Brand");
            dg.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Header = "Model";
            column.Binding = new Binding("Model");
            dg.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Header = "ProductionYear";
            column.Binding = new Binding("ProductionYear");
            dg.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Header = "Price";
            column.Binding = new Binding("Price");
            dg.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Header = "ClientID";
            column.Binding = new Binding("ClientID");
            dg.Columns.Add(column);

            DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
            buttonColumn.Header = "Actions";
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory panelFactory = new FrameworkElementFactory(typeof(DockPanel));
            buttonTemplate.VisualTree = panelFactory;
            FrameworkElementFactory buttonAFactory = new FrameworkElementFactory(typeof(Button));
            buttonAFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Change));
            buttonAFactory.SetValue(ContentProperty, "Change");
            FrameworkElementFactory buttonBFactory = new FrameworkElementFactory(typeof(Button));
            buttonBFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Delete));
            buttonBFactory.SetValue(ContentProperty, "Delete");
            panelFactory.AppendChild(buttonAFactory);
            panelFactory.AppendChild(buttonBFactory);
            buttonColumn.CellTemplate = buttonTemplate;
            dg.Columns.Add(buttonColumn);

            var db = new ShopContext();
            var cars = db.Cars;
            foreach (var car in cars)
            {
                dg.Items.Add(car);
            }
        }

        public void EditCar_onClick(object sender, RoutedEventArgs e)
        {
            if (Brand_Edit.Text != "" && Model_Edit.Text != "" && Year_Edit.Text != "" && Price_Edit.Text != "")
            {
                using (var db = new ShopContext())
                {
                    var car = db.Cars.FirstOrDefault(x => x.Id == editItem_ID);
                    if (car != null)
                    {
                        car.Brand = Brand_Edit.Text;
                        car.Model = Model_Edit.Text;
                        int yearEdit;
                        int priceEdit;
                        if (int.TryParse(Year_Edit.Text, out yearEdit) && int.TryParse(Price_Edit.Text, out priceEdit))
                        {
                            car.ProductionYear = yearEdit;
                            car.Price = priceEdit;
                            dg.Items[editItem_Index] = car;
                            db.SaveChanges();
                            editItem_Index = -1;
                            editItem_ID = -1;
                            EditCar_Title.Content = "Edit car (ID: N/A)";
                            EditCar_Success.Content = $"Car (ID: {car.Id}) EDITTED!";
                            EditCar_Success.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            EditCar_Success.Content = "Year & Price MUST be numerical!";
                            EditCar_Success.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
            else
            {
                EditCar_Success.Content = "Fill in all the fields!";
                EditCar_Success.Visibility = Visibility.Visible;
            }
        }

        public void Change(object sender, RoutedEventArgs e)
        {
            Car car = (Car)dg.SelectedItem;
            EditCar_Title.Content = $"Edit car (ID: {car.Id})";
            editItem_ID = car.Id;
            Brand_Edit.Text = car.Brand;
            Model_Edit.Text = car.Model;
            Year_Edit.Text = car.ProductionYear.ToString();
            Price_Edit.Text = car.Price.ToString();
            editItem_Index = dg.SelectedIndex;
        }

        public void Delete(object sender, RoutedEventArgs e)
        {
            Car car = (Car)dg.SelectedItem;
            dg.Items.Remove(car);

            using (var db = new ShopContext())
            {
                db.Cars.Where(x => x.Id == car.Id).ExecuteDelete();
            }
        }

        public void AddCar_onClick(object sender, RoutedEventArgs e)
        {
            if (Brand_Input.Text != "" && Model_Input.Text != "" && Year_Input.Text != "" && Price_Input.Text != "")
            {
                using (var db = new ShopContext())
                {
                    var cars = db.Set<Car>();
                    int yearInput;
                    int priceInput;
                    if (int.TryParse(Year_Input.Text, out yearInput) && int.TryParse(Price_Input.Text, out priceInput))
                    {
                        Car newCar = new Car { Brand = Brand_Input.Text, Model = Model_Input.Text, ProductionYear = yearInput, Price = priceInput, ClientID = -1 };
                        cars.Add(newCar);

                        db.SaveChanges();

                        dg.Items.Add(newCar);

                        AddCar_Success.Content = $"Car (ID: {newCar.Id}) ADDED!";
                        AddCar_Success.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        AddCar_Success.Content = "Year & Price MUST be numerical!";
                        AddCar_Success.Visibility = Visibility.Visible;
                    }
                }
            }
            else
            {
                AddCar_Success.Content = "Fill in all the fields!";
                AddCar_Success.Visibility = Visibility.Visible;
            }
        }
    }
}
