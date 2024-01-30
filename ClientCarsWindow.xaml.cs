using CarShop.Data;
using CarShop.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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
    /// Interaction logic for ClientCarsWindow.xaml
    /// </summary>
    public partial class ClientCarsWindow : Window
    {
        DataGrid dg;
        Client? currentClient;

        public ClientCarsWindow(int clientID)
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

            DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
            buttonColumn.Header = "Actions";
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory panelFactory = new FrameworkElementFactory(typeof(DockPanel));
            buttonTemplate.VisualTree = panelFactory;
            FrameworkElementFactory buttonAFactory = new FrameworkElementFactory(typeof(Button));
            buttonAFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(RemoveCar));
            buttonAFactory.SetValue(ContentProperty, "Remove");
            panelFactory.AppendChild(buttonAFactory);
            buttonColumn.CellTemplate = buttonTemplate;
            dg.Columns.Add(buttonColumn);

            using (var db = new ShopContext())
            {
                currentClient = db.Clients.FirstOrDefault(x => x.Id == clientID);
                if (currentClient != null)
                {
                    var cars = db.Cars.Where(x => x.ClientID == currentClient.Id).ToList();

                    foreach (var car in cars)
                    {
                        dg.Items.Add(car);
                    }
                }
            }
        }

        public void RemoveCar(object sender, EventArgs e)
        {
            Car car = (Car)dg.SelectedItem;
            if (car != null)
            {
                /*using(var db = new ShopContext())
                {
                    car.ClientID = 5;
                    db.SaveChanges();
                }*/
                ////////////////////////
                string connectionString = @"Data Source=SSD_1;Initial Catalog=CarShop;Integrated Security=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string queryString = "UPDATE Cars SET ClientID = @clientID WHERE ID = @carID";
                    SqlCommand command = new(queryString, connection);
                    command.Parameters.AddWithValue("@clientID", -1);
                    command.Parameters.AddWithValue("@carID", car.Id);
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                dg.Items.Remove(car);
            }
        }

        public void ClientAddVehicle_onClick(object sender, EventArgs e)
        {
            if (CarID_Input.Text != "")
            {
                int caridInput;
                if (int.TryParse(CarID_Input.Text, out caridInput))
                {
                    if (currentClient != null)
                    {
                        using (var db = new ShopContext())
                        {
                            var car = db.Cars.FirstOrDefault(x => x.Id == caridInput && x.ClientID == -1);
                            if (car != null && currentClient != null)
                            {
                                car.ClientID = currentClient.Id;
                                db.SaveChanges();
                                dg.Items.Add(car);
                            }
                        }
                    }
                }
            }
        }
    }
}
