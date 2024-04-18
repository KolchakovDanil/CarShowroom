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
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace CarShowroom
{
    /// <summary>
    /// Логика взаимодействия для RedZakaz.xaml
    /// </summary>
    public partial class RedZakaz : Window
    {
        public RedZakaz()
        {
            InitializeComponent();
        }
        private void insur(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^а-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void inname(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^а-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void inpatr(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^а-яА-Я]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void inphone(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void menuBack_Click(object sender, RoutedEventArgs e)
        {
            Admin admin = new Admin();
            admin.Show();
            this.Close();
        }
        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа разработана для упрощения работы персонала на предприяти \"Автосалон\".");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void combID_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        //ВЫВОД ИЗОБРАЖЕНИЯ
        private void LoadImage(int carId)
        {
            string connectionString = ClassSQL.GetConnSQL();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT C_IMG FROM Cars WHERE C_ID = @CarId", connection);
                    command.Parameters.AddWithValue("@CarId", carId);
                    string imageUrl = command.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        string basePath = AppDomain.CurrentDomain.BaseDirectory;
                        string imagePath = System.IO.Path.Combine(basePath, imageUrl);
                        imgCar.Source = new BitmapImage(new Uri(imagePath));
                    }
                    else
                    {
                        MessageBox.Show("Изображение не найдено для выбранной книги.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки изображения: " + ex.Message);
                }
            }
        }
    }
}
