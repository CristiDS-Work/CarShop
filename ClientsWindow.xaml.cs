using CarShop.Data;
using CarShop.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
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
    /// Interaction logic for ClientsWindow.xaml
    /// </summary>
    public partial class ClientsWindow : Window
    {
        DataGrid dg;
        int editItem_ID = -1;
        int editItem_Index = -1;

        public ClientsWindow()
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
            column.Header = "FirstName";
            column.Binding = new Binding("FirstName");
            dg.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Header = "LastName";
            column.Binding = new Binding("LastName");
            dg.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Header = "Email";
            column.Binding = new Binding("Email");
            dg.Columns.Add(column);

            column = new DataGridTextColumn();
            column.Header = "PhoneNumber";
            column.Binding = new Binding("PhoneNumber");
            dg.Columns.Add(column);

            DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
            buttonColumn.Header = "Actions";
            DataTemplate buttonTemplate = new DataTemplate();
            FrameworkElementFactory panelFactory = new FrameworkElementFactory(typeof(DockPanel));
            buttonTemplate.VisualTree = panelFactory;
            FrameworkElementFactory button0Factory = new FrameworkElementFactory(typeof(Button));
            button0Factory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ShowVehicles));
            button0Factory.SetValue(ContentProperty, "Vehicles");
            FrameworkElementFactory buttonAFactory = new FrameworkElementFactory(typeof(Button));
            buttonAFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Change));
            buttonAFactory.SetValue(ContentProperty, "Change");
            FrameworkElementFactory buttonBFactory = new FrameworkElementFactory(typeof(Button));
            buttonBFactory.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(Delete));
            buttonBFactory.SetValue(ContentProperty, "Delete");
            panelFactory.AppendChild(button0Factory);
            panelFactory.AppendChild(buttonAFactory);
            panelFactory.AppendChild(buttonBFactory);
            buttonColumn.CellTemplate = buttonTemplate;
            dg.Columns.Add(buttonColumn);

            var db = new ShopContext();
            var clients = db.Clients;
            foreach (var client in clients)
            {
                dg.Items.Add(client);
            }
        }

        public void ShowVehicles(object sender, RoutedEventArgs e)
        {
            Client client = (Client)dg.SelectedItem;
            ClientCarsWindow carsWindow = new ClientCarsWindow(client.Id);
            //Visibility = Visibility.Hidden;
            carsWindow.Show();
        }

        public void EditClient_onClick(object sender, RoutedEventArgs e)
        {
            if (FirstName_Edit.Text != "" && LastName_Edit.Text != "" && Email_Edit.Text != "" && PhoneNumber_Edit.Text != "")
            {
                using (var db = new ShopContext())
                {
                    var client = db.Clients.FirstOrDefault(x => x.Id == editItem_ID);
                    if (client != null)
                    {
                        client.FirstName = FirstName_Edit.Text;
                        client.LastName = LastName_Edit.Text;
                        client.Email = Email_Edit.Text;
                        client.PhoneNumber = PhoneNumber_Edit.Text;
                        dg.Items[editItem_Index] = client;
                        //db.SaveChanges();
                        editItem_Index = -1;

                        EditClient_Title.Content = "Edit client (ID: N/A)";
                        EditClient_Success.Content = $"Client (ID: {client.Id}) EDITTED!";
                        EditClient_Success.Visibility = Visibility.Visible;
                    }
                }
                ////////////////////////
                string connectionString = @"Data Source=SSD_1;Initial Catalog=CarShop;Integrated Security=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string queryString = "UPDATE Clients SET FirstName = @cFirstName, LastName = @cLastName, Email = @cEmail, PhoneNumber = @cPhoneNumber WHERE ID = @cID";
                    SqlCommand command = new(queryString, connection);
                    command.Parameters.AddWithValue("@cFirstName", FirstName_Edit.Text);
                    command.Parameters.AddWithValue("@cLastName", LastName_Edit.Text);
                    command.Parameters.AddWithValue("@cEmail", Email_Edit.Text);
                    command.Parameters.AddWithValue("@cPhoneNumber", PhoneNumber_Edit.Text);
                    command.Parameters.AddWithValue("@cID", editItem_ID);
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
                editItem_ID = -1;
            }
            else
            {
                EditClient_Success.Content = "Fill in all the fields.";
                EditClient_Success.Visibility = Visibility.Visible;
            }
        }

        public void Change(object sender, RoutedEventArgs e)
        {
            Client client = (Client)dg.SelectedItem;
            EditClient_Title.Content = $"Edit client (ID: {client.Id})";
            editItem_ID = client.Id;
            FirstName_Edit.Text = client.FirstName;
            LastName_Edit.Text = client.LastName;
            Email_Edit.Text = client.Email;
            PhoneNumber_Edit.Text = client.PhoneNumber;
            editItem_Index = dg.SelectedIndex;
        }

        public void Delete(object sender, RoutedEventArgs e)
        {
            Client client = (Client)dg.SelectedItem;
            dg.Items.Remove(client);

            using (var db = new ShopContext())
            {
                db.Clients.Where(x => x.Id == client.Id).ExecuteDelete();
            }
        }

        public void AddClient_onClick(object sender, RoutedEventArgs e)
        {
            if (FirstName_Input.Text != "" && LastName_Input.Text != "" && Email_Input.Text != "" && PhoneNumber_Input.Text != "")
            {
                using (var db = new ShopContext())
                {
                    var clients = db.Set<Client>();
                    Client newClient = new Client { FirstName = FirstName_Input.Text, LastName = LastName_Input.Text, Email = Email_Input.Text, PhoneNumber = PhoneNumber_Input.Text };
                    clients.Add(newClient);

                    db.SaveChanges();

                    dg.Items.Add(newClient);

                    AddClient_Success.Content = $"Client (ID: {newClient.Id}) ADDED!";
                    AddClient_Success.Visibility = Visibility.Visible;
                }
            }
            else
            {
                AddClient_Success.Content = "Fill in all the fields.";
                AddClient_Success.Visibility = Visibility.Visible;
            }
        }
    }
}
