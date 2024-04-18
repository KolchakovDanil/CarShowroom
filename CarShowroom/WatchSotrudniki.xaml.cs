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
    /// Логика взаимодействия для WatchSotrudniki.xaml
    /// </summary>
    public partial class WatchSotrudniki : Window
    {
        public WatchSotrudniki()
        {
            InitializeComponent();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа разработана для упрощения работы персонала на предприяти \"Автосалон\".");
        }
    }
}
