﻿using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void Clients_onClick(object sender, RoutedEventArgs e)
        {
            ClientsWindow clientsWindow = new ClientsWindow();
            //Visibility = Visibility.Hidden;
            clientsWindow.Show();
        }
        public void Cars_onClick(object sender, RoutedEventArgs e)
        {
            CarsWindow carsWindow = new CarsWindow();
            //Visibility = Visibility.Hidden;
            carsWindow.Show();
        }
    }
}
