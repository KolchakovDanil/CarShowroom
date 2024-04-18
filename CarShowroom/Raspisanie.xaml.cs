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
    /// Логика взаимодействия для Raspisanie.xaml
    /// </summary>
    public partial class Raspisanie : Window
    {
        public void getDostup()
        {
            dostup = 1;
        }
        public Raspisanie()
        {
            InitializeComponent();
        }
        int dostup = 0;
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь можно мониторить расписание работы персонала и редактировать его за Администратора.");
        }

        private void menuBack_Click(object sender, RoutedEventArgs e)
        {
            if (dostup == 1)
            {
                Admin ad = new Admin();
                ad.Show();
                this.Close();
            }
            else if (dostup == 0)
            {
                Prodavec prod = new Prodavec();
                prod.Show();
                this.Close();
            }
        }
    }
}
