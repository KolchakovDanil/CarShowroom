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
    /// Логика взаимодействия для Zakaz.xaml
    /// </summary>
    public partial class Zakaz : Window
    {
        public Zakaz()
        {
            InitializeComponent();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа разработана для упрощения работы персонала на предприяти \"Автосалон\".");
        }

        private void menuBack_Click(object sender, RoutedEventArgs e)
        {
            Prodavec prod = new Prodavec();
            prod.Show();
            this.Close();
        }
    }
}
