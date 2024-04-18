using System;
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
using System.Windows.Shapes;

namespace CarShowroom
{
    /// <summary>
    /// Логика взаимодействия для Prodavec.xaml
    /// </summary>
    public partial class Prodavec : Window
    {
        public Prodavec()
        {
            InitializeComponent();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы вошли в доступ \"Продавец\", здесь вы оформляете заказ для клиента. Также есть возможность просмотра вашего графика работ. " +
                "Будте внимательны при заполнении данных клиента и выбранного автомобиля!!!");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mn = new MainWindow();
            mn.Show();
            this.Close();
        }

        private void btnZakaz_Click(object sender, RoutedEventArgs e)
        {
            Zakaz zakaz = new Zakaz();
            zakaz.Show();
            this.Close();
        }

        private void btnRaspisanie_Click(object sender, RoutedEventArgs e)
        {
            Raspisanie raspisanie = new Raspisanie();
            raspisanie.Show();
        }
    }
}
