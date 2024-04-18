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
    /// Логика взаимодействия для Admin.xaml
    /// </summary>
    public partial class Admin : Window
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы вошли в доступ \"Администратор\", здесь возможны такие операции как: добавление, редактирование и удаление данных. " +
                "Также есть возможность просмотра графика работ персонала. " +
                "Вы осуществляете управление базой данных используя данное приложение!");
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mn = new MainWindow();
            mn.Show();
            this.Close();
        }
        private void btnRedZakaz_Click(object sender, RoutedEventArgs e)
        {
            RedZakaz red = new RedZakaz();
            red.Show();
            this.Close();
        }
        private void btnRedSotrud_Click(object sender, RoutedEventArgs e)
        {
            RedSotrudniki red = new RedSotrudniki();
            red.Show();
            this.Close();
        }
        private void btnRedAuto_Click(object sender, RoutedEventArgs e)
        {
            RedAuto red = new RedAuto();
            red.Show();
            this.Close();
        }
        private void btnRedRaspisanie_Click(object sender, RoutedEventArgs e)
        {
            Raspisanie red = new Raspisanie();
            red.getDostup();
            red.Show();
            this.Close();
        }

        private void btnWatchSotrud_Click(object sender, RoutedEventArgs e)
        {
            WatchSotrudniki watch = new WatchSotrudniki();
            watch.Show();
        }
        private void btnWatchZakaz_Click(object sender, RoutedEventArgs e)
        {
            WatchZakaz watch = new WatchZakaz();
            watch.Show();
        }
    }
}
